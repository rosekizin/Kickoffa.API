using Amazon.S3;
using Amazon.S3.Model;
using Kickoffa.API.Application.Configuration;
using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Application.MessageErrors;
using Kickoffa.API.Domain.Interfaces.ProcessResult;
using Kickoffa.API.Domain.ProcessResult;
using Kickoffa.API.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;

namespace Kickoffa.API.Application.Services
{
	/// <summary>
	/// Serviço para upload de arquivos para AWS S3
	/// </summary>
	public class FileUploadService : IFileUploadService
	{
		private readonly IAmazonS3 _s3Client;
		private readonly AwsS3Configuration _s3Config;
		private readonly ICurrentUserService _currentUserService;
		private readonly ILogger<FileUploadService> _logger;

		private readonly string[] _allowedImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg"];
		private readonly string[] _allowedImageContentTypes = ["image/jpeg", "image/png", "image/gif", "image/webp", "image/svg+xml"];

		public FileUploadService(
			IAmazonS3 s3Client,
			IOptions<AwsS3Configuration> s3Config,
			ICurrentUserService currentUserService,
			ILogger<FileUploadService> logger)
		{
			_s3Client = s3Client;
			_s3Config = s3Config.Value;
			_currentUserService = currentUserService;
			_logger = logger;
		}

		/// <inheritdoc/>
		public async Task<IResult<string>> UploadImageAsync(IFormFile file, string folder, CancellationToken cancellationToken)
		{
			var validationResult = ValidateImageFile(file);
			if (validationResult.IsFailure)
				return validationResult;

			// Gerar caminho da pasta com userId para imagens de briefing
			var userScopedFolder = GenerateUserScopedFolder(folder);

			return await UploadFileInternalAsync(file, userScopedFolder, cancellationToken);
		}

		/// <inheritdoc/>
		public async Task<IResult<string>> UploadFileAsync(IFormFile file, string folder, CancellationToken cancellationToken)
		{
			var validationResult = ValidateFile(file);
			if (validationResult.IsFailure)
				return validationResult;

			return await UploadFileInternalAsync(file, folder, cancellationToken);
		}

		/// <inheritdoc/>
		public async Task<IResult<bool>> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(fileUrl))
					return Result<bool>.Success(false);

				var fileKey = ExtractKeyFromUrl(fileUrl);
				if (string.IsNullOrWhiteSpace(fileKey))
					return Result<bool>.Success(false);

				var deleteRequest = new DeleteObjectRequest
				{
					BucketName = _s3Config.BucketName,
					Key = fileKey
				};

				var response = await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);

				_logger.LogInformation("Arquivo removido do S3: {FileKey}", fileKey);
				var success = response.HttpStatusCode == HttpStatusCode.NoContent;
				return Result<bool>.Success(success);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao remover arquivo do S3: {FileUrl}", fileUrl);
				return Result<bool>.Failure(FileUploadServiceErrors.S3UploadError(fileUrl, ex));
			}
		}

		/// <inheritdoc/>
		public async Task<IResult<string>> GeneratePresignedUrlAsync(string fileKey, int expirationMinutes = 60)
		{
			try
			{
				var request = new GetPreSignedUrlRequest
				{
					BucketName = _s3Config.BucketName,
					Key = fileKey,
					Verb = HttpVerb.GET,
					Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
				};

				var url = await _s3Client.GetPreSignedURLAsync(request);
				_logger.LogInformation("URL pré-assinada gerada para: {FileKey}", fileKey);

				return Result<string>.Success(url);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao gerar URL pré-assinada para: {FileKey}", fileKey);
				return Result<string>.Failure(FileUploadServiceErrors.PresignedUrlError(fileKey, ex));
			}
		}

		private async Task<IResult<string>> UploadFileInternalAsync(IFormFile file, string folder, CancellationToken cancellationToken)
		{
			try
			{
				var fileName = GenerateUniqueFileName(file.FileName);
				var fileKey = $"{folder.Trim('/')}/{fileName}";

				using var stream = file.OpenReadStream();

				var uploadRequest = new PutObjectRequest
				{
					BucketName = _s3Config.BucketName,
					Key = fileKey,
					InputStream = stream,
					ContentType = file.ContentType,
					ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
					CannedACL = S3CannedACL.Private // Arquivo privado - acesso apenas via backend
				};

				// Adicionar metadados
				uploadRequest.Metadata.Add("original-filename", file.FileName);
				uploadRequest.Metadata.Add("upload-date", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

				var response = await _s3Client.PutObjectAsync(uploadRequest, cancellationToken);

				if (response.HttpStatusCode == HttpStatusCode.OK)
				{
					var fileUrl = $"{_s3Config.BaseUrl.TrimEnd('/')}/{fileKey}";
					_logger.LogInformation("Arquivo enviado com sucesso para S3: {FileKey}", fileKey);
					return Result<string>.Success(fileUrl);
				}

				return Result<string>.Failure(FileUploadServiceErrors.UploadFailed(response.HttpStatusCode.ToString()));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao fazer upload do arquivo: {FileName}", file.FileName);
				return Result<string>.Failure(FileUploadServiceErrors.S3UploadError(file.FileName, ex));
			}
		}

		private Result<string> ValidateImageFile(IFormFile file)
		{
			var fileValidation = ValidateFile(file);
			if (fileValidation.IsFailure)
				return fileValidation;

			var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
			if (!_allowedImageExtensions.Contains(extension))
			{
				return Result<string>
					.Failure(FileUploadServiceErrors.InvalidImageExtension(extension, string.Join(", ", _allowedImageExtensions)));
			}

			if (!_allowedImageContentTypes.Contains(file.ContentType.ToLowerInvariant()))
			{
				return Result<string>
					.Failure(FileUploadServiceErrors.InvalidImageContentType(file.ContentType, string.Join(", ", _allowedImageContentTypes)));
			}

			return Result<string>.Success(string.Empty);
		}

		private static Result<string> ValidateFile(IFormFile file)
		{
			if (file == null)
				return Result<string>.Failure(FileUploadServiceErrors.FileNull());

			if (file.Length == 0)
				return Result<string>.Failure(FileUploadServiceErrors.FileEmpty());

			if (file.Length > 10 * 1024 * 1024) // 10MB
				return Result<string>.Failure(FileUploadServiceErrors.FileTooLarge(10));

			if (string.IsNullOrWhiteSpace(file.FileName))
				return Result<string>.Failure(FileUploadServiceErrors.FileNameRequired());

			return Result<string>.Success(string.Empty);
		}

		/// <summary>
		/// Gera caminho da pasta com escopo de usuário para organização por userId
		/// </summary>
		/// <param name="baseFolder">Pasta base (ex: "briefing/images")</param>
		/// <returns>Caminho da pasta com userId (ex: "briefing/images/userId/123")</returns>
		private string GenerateUserScopedFolder(string baseFolder)
		{
			// Se não há usuário autenticado, usar pasta genérica
			if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
			{
				_logger.LogWarning("Upload sem usuário autenticado, usando pasta genérica");
				return $"{baseFolder.Trim('/')}/anonymous";
			}

			var userId = _currentUserService.UserId.Value;
			var userScopedPath = $"{baseFolder.Trim('/')}/userId/{userId}";

			_logger.LogInformation("Pasta de upload gerada: {UserScopedPath} para usuário: {UserId}",
				userScopedPath, userId);

			return userScopedPath;
		}

		private static string GenerateUniqueFileName(string originalFileName)
		{
			var extension = Path.GetExtension(originalFileName);
			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
			var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
			var guid = Guid.NewGuid().ToString("N")[..8];

			var normalizedFileNameWithoutExtension = ReplaceMultipleSpacesWithSingleDash(fileNameWithoutExtension);

            return $"{normalizedFileNameWithoutExtension}_{timestamp}_{guid}{extension}";
		}

        public static string ReplaceMultipleSpacesWithSingleDash(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var result = new StringBuilder(input.Length);
            bool lastWasSpace = false;

            foreach (var c in input)
            {
                if (c == ' ')
                {
                    if (!lastWasSpace)
                    {
                        result.Append('-');
                        lastWasSpace = true;
                    }
                    // senão: ignora espaços consecutivos
                }
                else
                {
                    result.Append(c);
                    lastWasSpace = false;
                }
            }

            return result.ToString();
        }

        private string ExtractKeyFromUrl(string fileUrl)
		{
			try
			{
				var baseUrl = _s3Config.BaseUrl.TrimEnd('/');
				if (fileUrl.StartsWith(baseUrl))
				{
					return fileUrl.Substring(baseUrl.Length + 1);
				}

				// Tentar extrair da URL do S3 padrão
				var uri = new Uri(fileUrl);
				return uri.AbsolutePath.TrimStart('/');
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao extrair chave da URL: {FileUrl}", fileUrl);
				return string.Empty;
			}
		}
	}
}
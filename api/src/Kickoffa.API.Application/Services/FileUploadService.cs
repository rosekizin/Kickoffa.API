using Amazon.S3;
using Amazon.S3.Model;
using Kickoffa.API.Application.Configuration;
using Kickoffa.API.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace Kickoffa.API.Application.Services
{
	/// <summary>
	/// Serviço para upload de arquivos para AWS S3
	/// </summary>
	public class FileUploadService : IFileUploadService
	{
		private readonly IAmazonS3 _s3Client;
		private readonly AwsS3Configuration _s3Config;
		private readonly ILogger<FileUploadService> _logger;

		private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
		private readonly string[] _allowedImageContentTypes = { "image/jpeg", "image/png", "image/gif", "image/webp", "image/svg+xml" };

		public FileUploadService(
			IAmazonS3 s3Client,
			IOptions<AwsS3Configuration> s3Config,
			ILogger<FileUploadService> logger)
		{
			_s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
			_s3Config = s3Config?.Value ?? throw new ArgumentNullException(nameof(s3Config));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc/>
		public async Task<string> UploadImageAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
		{
			ValidateImageFile(file);
			return await UploadFileInternalAsync(file, folder, cancellationToken);
		}

		/// <inheritdoc/>
		public async Task<string> UploadFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
		{
			ValidateFile(file);
			return await UploadFileInternalAsync(file, folder, cancellationToken);
		}

		/// <inheritdoc/>
		public async Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(fileUrl))
					return false;

				var fileKey = ExtractKeyFromUrl(fileUrl);
				if (string.IsNullOrWhiteSpace(fileKey))
					return false;

				var deleteRequest = new DeleteObjectRequest
				{
					BucketName = _s3Config.BucketName,
					Key = fileKey
				};

				var response = await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
				
				_logger.LogInformation("Arquivo removido do S3: {FileKey}", fileKey);
				return response.HttpStatusCode == HttpStatusCode.NoContent;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao remover arquivo do S3: {FileUrl}", fileUrl);
				return false;
			}
		}

		/// <inheritdoc/>
		public async Task<string> GeneratePresignedUrlAsync(string fileKey, int expirationMinutes = 60)
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
				
				return url;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao gerar URL pré-assinada para: {FileKey}", fileKey);
				throw;
			}
		}

		private async Task<string> UploadFileInternalAsync(IFormFile file, string folder, CancellationToken cancellationToken)
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
					return fileUrl;
				}

				throw new InvalidOperationException($"Falha no upload. Status: {response.HttpStatusCode}");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao fazer upload do arquivo: {FileName}", file.FileName);
				throw;
			}
		}

		private void ValidateImageFile(IFormFile file)
		{
			ValidateFile(file);

			var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
			if (!_allowedImageExtensions.Contains(extension))
			{
				throw new ArgumentException($"Extensão de arquivo não permitida: {extension}. Extensões permitidas: {string.Join(", ", _allowedImageExtensions)}");
			}

			if (!_allowedImageContentTypes.Contains(file.ContentType.ToLowerInvariant()))
			{
				throw new ArgumentException($"Tipo de conteúdo não permitido: {file.ContentType}. Tipos permitidos: {string.Join(", ", _allowedImageContentTypes)}");
			}
		}

		private static void ValidateFile(IFormFile file)
		{
			if (file == null)
				throw new ArgumentNullException(nameof(file));

			if (file.Length == 0)
				throw new ArgumentException("Arquivo está vazio", nameof(file));

			if (file.Length > 10 * 1024 * 1024) // 10MB
				throw new ArgumentException("Arquivo muito grande. Tamanho máximo: 10MB", nameof(file));

			if (string.IsNullOrWhiteSpace(file.FileName))
				throw new ArgumentException("Nome do arquivo é obrigatório", nameof(file));
		}

		private static string GenerateUniqueFileName(string originalFileName)
		{
			var extension = Path.GetExtension(originalFileName);
			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
			var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
			var guid = Guid.NewGuid().ToString("N")[..8];
			
			return $"{fileNameWithoutExtension}_{timestamp}_{guid}{extension}";
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

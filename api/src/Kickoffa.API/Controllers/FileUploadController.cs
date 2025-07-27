using Kickoffa.API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kickoffa.API.Controllers
{
	/// <summary>
	/// Controller para upload de arquivos
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class FileUploadController : ControllerBase
	{
		private readonly IFileUploadService _fileUploadService;
		private readonly IImageProxyService _imageProxyService;
		private readonly ILogger<FileUploadController> _logger;

		public FileUploadController(
			IFileUploadService fileUploadService,
			IImageProxyService imageProxyService,
			ILogger<FileUploadController> logger)
		{
			_fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
			_imageProxyService = imageProxyService ?? throw new ArgumentNullException(nameof(imageProxyService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Upload de imagem para uso no TipTap (briefing)
		/// </summary>
		/// <param name="file">Arquivo de imagem</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>URL da imagem no S3</returns>
		[HttpPost("image/briefing")]
		[ProducesResponseType(typeof(FileUploadResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UploadBriefingImage(
			IFormFile file,
			CancellationToken cancellationToken = default)
		{
			try
			{
				if (file == null)
				{
					return BadRequest(new { message = "Nenhum arquivo foi enviado" });
				}

				_logger.LogInformation("Iniciando upload de imagem para briefing: {FileName}", file.FileName);

				// Upload para S3 (retorna URL direta do S3)
				var s3Url = await _fileUploadService.UploadImageAsync(file, "briefing/images", cancellationToken);

				// Extrair chave do arquivo da URL do S3
				var fileKey = _imageProxyService.ExtractFileKeyFromUrl(s3Url);
				if (string.IsNullOrEmpty(fileKey))
				{
					_logger.LogError("Não foi possível extrair chave do arquivo da URL: {S3Url}", s3Url);
					return StatusCode(StatusCodes.Status500InternalServerError,
						new { message = "Erro ao processar upload" });
				}

				// Gerar URL do proxy para acesso seguro
				var proxyUrl = _imageProxyService.GenerateProxyUrl(fileKey);

				var response = new FileUploadResponse
				{
					Url = proxyUrl, // Retornar URL do proxy ao invés da URL direta do S3
					FileName = file.FileName,
					ContentType = file.ContentType,
					Size = file.Length
				};

				_logger.LogInformation("Resposta do upload: {Response}", System.Text.Json.JsonSerializer.Serialize(response));

				_logger.LogInformation("Upload de imagem concluído com sucesso. S3: {S3Url}, Proxy: {ProxyUrl}",
					s3Url, proxyUrl);

				return Ok(response);
			}
			catch (ArgumentException ex)
			{
				_logger.LogWarning(ex, "Erro de validação no upload de imagem: {FileName}", file?.FileName);
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro interno no upload de imagem: {FileName}", file?.FileName);
				return StatusCode(StatusCodes.Status500InternalServerError, 
					new { message = "Erro interno do servidor ao fazer upload da imagem" });
			}
		}

		/// <summary>
		/// Upload de arquivo genérico
		/// </summary>
		/// <param name="file">Arquivo</param>
		/// <param name="folder">Pasta de destino</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>URL do arquivo no S3</returns>
		[HttpPost("file")]
		[ProducesResponseType(typeof(FileUploadResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UploadFile(
			IFormFile file,
			[FromQuery] string folder = "uploads",
			CancellationToken cancellationToken = default)
		{
			try
			{
				if (file == null)
				{
					return BadRequest(new { message = "Nenhum arquivo foi enviado" });
				}

				if (string.IsNullOrWhiteSpace(folder))
				{
					folder = "uploads";
				}

				_logger.LogInformation("Iniciando upload de arquivo: {FileName} para pasta: {Folder}", file.FileName, folder);

				// Upload para S3
				var s3Url = await _fileUploadService.UploadFileAsync(file, folder, cancellationToken);

				// Extrair chave do arquivo da URL do S3
				var fileKey = _imageProxyService.ExtractFileKeyFromUrl(s3Url);
				if (string.IsNullOrEmpty(fileKey))
				{
					_logger.LogError("Não foi possível extrair chave do arquivo da URL: {S3Url}", s3Url);
					return StatusCode(StatusCodes.Status500InternalServerError,
						new { message = "Erro ao processar upload" });
				}

				// Gerar URL do proxy para acesso seguro
				var proxyUrl = _imageProxyService.GenerateProxyUrl(fileKey);

				var response = new FileUploadResponse
				{
					Url = proxyUrl, // Retornar URL do proxy
					FileName = file.FileName,
					ContentType = file.ContentType,
					Size = file.Length
				};

				_logger.LogInformation("Upload de arquivo concluído com sucesso. S3: {S3Url}, Proxy: {ProxyUrl}",
					s3Url, proxyUrl);

				return Ok(response);
			}
			catch (ArgumentException ex)
			{
				_logger.LogWarning(ex, "Erro de validação no upload de arquivo: {FileName}", file?.FileName);
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro interno no upload de arquivo: {FileName}", file?.FileName);
				return StatusCode(StatusCodes.Status500InternalServerError, 
					new { message = "Erro interno do servidor ao fazer upload do arquivo" });
			}
		}

		/// <summary>
		/// Gera URL pré-assinada para uma imagem (para arquivos privados)
		/// </summary>
		/// <param name="fileKey">Chave do arquivo no S3</param>
		/// <param name="expirationMinutes">Tempo de expiração em minutos</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>URL pré-assinada</returns>
		[HttpGet("presigned-url")]
		[ProducesResponseType(typeof(PresignedUrlResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetPresignedUrl(
			[FromQuery] string fileKey,
			[FromQuery] int expirationMinutes = 60,
			CancellationToken cancellationToken = default)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(fileKey))
				{
					return BadRequest(new { message = "Chave do arquivo é obrigatória" });
				}

				var presignedUrl = await _fileUploadService.GeneratePresignedUrlAsync(fileKey, expirationMinutes);

				var response = new PresignedUrlResponse
				{
					Url = presignedUrl,
					ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
					FileKey = fileKey
				};

				return Ok(response);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao gerar URL pré-assinada para: {FileKey}", fileKey);
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = "Erro interno do servidor ao gerar URL pré-assinada" });
			}
		}

		/// <summary>
		/// Remove um arquivo do S3
		/// </summary>
		/// <param name="fileUrl">URL do arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Resultado da operação</returns>
		[HttpDelete]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteFile(
			[FromQuery] string fileUrl,
			CancellationToken cancellationToken = default)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(fileUrl))
				{
					return BadRequest(new { message = "URL do arquivo é obrigatória" });
				}

				_logger.LogInformation("Iniciando remoção de arquivo: {FileUrl}", fileUrl);

				var success = await _fileUploadService.DeleteFileAsync(fileUrl, cancellationToken);

				if (success)
				{
					_logger.LogInformation("Arquivo removido com sucesso: {FileUrl}", fileUrl);
					return Ok(new { message = "Arquivo removido com sucesso" });
				}

				_logger.LogWarning("Arquivo não encontrado ou não pôde ser removido: {FileUrl}", fileUrl);
				return NotFound(new { message = "Arquivo não encontrado ou não pôde ser removido" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro interno ao remover arquivo: {FileUrl}", fileUrl);
				return StatusCode(StatusCodes.Status500InternalServerError, 
					new { message = "Erro interno do servidor ao remover arquivo" });
			}
		}
	}

	/// <summary>
	/// Resposta do upload de arquivo
	/// </summary>
	public class FileUploadResponse
	{
		/// <summary>
		/// URL do arquivo no S3
		/// </summary>
		public string Url { get; set; } = string.Empty;

		/// <summary>
		/// Nome original do arquivo
		/// </summary>
		public string FileName { get; set; } = string.Empty;

		/// <summary>
		/// Tipo de conteúdo do arquivo
		/// </summary>
		public string ContentType { get; set; } = string.Empty;

		/// <summary>
		/// Tamanho do arquivo em bytes
		/// </summary>
		public long Size { get; set; }
	}

	/// <summary>
	/// Resposta da URL pré-assinada
	/// </summary>
	public class PresignedUrlResponse
	{
		/// <summary>
		/// URL pré-assinada
		/// </summary>
		public string Url { get; set; } = string.Empty;

		/// <summary>
		/// Data de expiração da URL
		/// </summary>
		public DateTime ExpiresAt { get; set; }

		/// <summary>
		/// Chave do arquivo no S3
		/// </summary>
		public string FileKey { get; set; } = string.Empty;
	}
}

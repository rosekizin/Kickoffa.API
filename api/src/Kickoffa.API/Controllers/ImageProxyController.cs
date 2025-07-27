using Kickoffa.API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Kickoffa.API.Controllers
{
	/// <summary>
	/// Controller para servir imagens privadas do S3 através de proxy autenticado
	/// </summary>
	[ApiController]
	[Route("api/images")]
	[Authorize]
	public class ImageProxyController : ControllerBase
	{
		private readonly IImageProxyService _imageProxyService;
		private readonly ILogger<ImageProxyController> _logger;

		public ImageProxyController(
			IImageProxyService imageProxyService,
			ILogger<ImageProxyController> logger)
		{
			_imageProxyService = imageProxyService ?? throw new ArgumentNullException(nameof(imageProxyService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Serve uma imagem privada do S3
		/// </summary>
		/// <param name="fileKey">Chave do arquivo no S3 (pode conter barras)</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Stream da imagem</returns>
		[HttpGet("{*fileKey}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetImage(
			string fileKey,
			CancellationToken cancellationToken = default)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(fileKey))
				{
					_logger.LogWarning("Chave do arquivo não fornecida");
					return BadRequest(new { message = "Chave do arquivo é obrigatória" });
				}

				_logger.LogInformation("Solicitação de imagem: {FileKey} por usuário: {User}", 
					fileKey, User.Identity?.Name ?? "Anônimo");

				// Verificar se o arquivo existe
				var exists = await _imageProxyService.FileExistsAsync(fileKey, cancellationToken);
				if (!exists)
				{
					_logger.LogWarning("Arquivo não encontrado: {FileKey}", fileKey);
					return NotFound(new { message = "Imagem não encontrada" });
				}

				// Verificar cache do navegador
				var ifNoneMatch = Request.Headers[HeaderNames.IfNoneMatch].FirstOrDefault();
				
				// Obter stream da imagem
				var imageResult = await _imageProxyService.GetImageStreamAsync(fileKey, cancellationToken);
				if (imageResult == null)
				{
					_logger.LogWarning("Não foi possível obter stream da imagem: {FileKey}", fileKey);
					return NotFound(new { message = "Imagem não encontrada" });
				}

				// Verificar ETag para cache
				if (!string.IsNullOrEmpty(ifNoneMatch) && 
					!string.IsNullOrEmpty(imageResult.ETag) && 
					ifNoneMatch.Trim('"') == imageResult.ETag)
				{
					_logger.LogInformation("Imagem não modificada (cache): {FileKey}", fileKey);
					return StatusCode(StatusCodes.Status304NotModified);
				}

				// Configurar headers de cache
				Response.Headers[HeaderNames.CacheControl] = "private, max-age=3600"; // Cache por 1 hora
				Response.Headers[HeaderNames.LastModified] = imageResult.LastModified.Value.ToString("R");

				if (!string.IsNullOrEmpty(imageResult.ETag))
				{
					Response.Headers[HeaderNames.ETag] = $"\"{imageResult.ETag}\"";
				}

				// Configurar headers de segurança
				Response.Headers["X-Content-Type-Options"] = "nosniff";
				Response.Headers["X-Frame-Options"] = "SAMEORIGIN"; // Permitir iframe no mesmo domínio
				Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
				Response.Headers["X-Robots-Tag"] = "noindex, nofollow"; // Não indexar imagens

				_logger.LogInformation("Servindo imagem: {FileKey}, Size: {Size} bytes, ContentType: {ContentType}", 
					fileKey, imageResult.ContentLength, imageResult.ContentType);

				// Retornar stream da imagem
				return File(
					imageResult.Stream, 
					imageResult.ContentType, 
					enableRangeProcessing: true
				);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro interno ao servir imagem: {FileKey}", fileKey);
				return StatusCode(StatusCodes.Status500InternalServerError, 
					new { message = "Erro interno do servidor" });
			}
		}

		/// <summary>
		/// Verifica se uma imagem existe (HEAD request)
		/// </summary>
		/// <param name="fileKey">Chave do arquivo no S3</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Status da existência do arquivo</returns>
		[HttpHead("{*fileKey}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> HeadImage(
			string fileKey,
			CancellationToken cancellationToken = default)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(fileKey))
				{
					return BadRequest();
				}

				var exists = await _imageProxyService.FileExistsAsync(fileKey, cancellationToken);
				
				if (exists)
				{
					Response.Headers[HeaderNames.CacheControl] = "private, max-age=3600";
					return Ok();
				}

				return NotFound();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao verificar existência da imagem: {FileKey}", fileKey);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}

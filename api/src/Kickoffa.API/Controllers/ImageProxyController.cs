using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.AspNet.Infrastructure.ErrorHandling;
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
		private readonly IActionResultErrorHandler _actionResultErrorHandler;
		private readonly ILogger<ImageProxyController> _logger;

		public ImageProxyController(
			IImageProxyService imageProxyService,
			IActionResultErrorHandler actionResultErrorHandler,
			ILogger<ImageProxyController> logger)
		{
			_imageProxyService = imageProxyService ?? throw new ArgumentNullException(nameof(imageProxyService));
			_actionResultErrorHandler = actionResultErrorHandler ?? throw new ArgumentNullException(nameof(actionResultErrorHandler));
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
			if (string.IsNullOrWhiteSpace(fileKey))
			{
				_logger.LogWarning("Chave do arquivo não fornecida");
				return BadRequest(new { message = "Chave do arquivo é obrigatória" });
			}

			_logger.LogInformation("Solicitação de imagem: {FileKey} por usuário: {User}",
				fileKey, User.Identity?.Name ?? "Anônimo");

			// Verificar se o arquivo existe
			var existsResult = await _imageProxyService.FileExistsAsync(fileKey, cancellationToken);
			if (existsResult.IsFailure)
			{
				return (ActionResult)_actionResultErrorHandler.GetActionResultFromError(existsResult.ErrorObject!);
			}

			if (!existsResult.Value)
			{
				_logger.LogWarning("Arquivo não encontrado: {FileKey}", fileKey);
				return NotFound(new { message = "Imagem não encontrada" });
			}

			// Verificar cache do navegador
			var ifNoneMatch = Request.Headers[HeaderNames.IfNoneMatch].FirstOrDefault();

			// Obter stream da imagem
			var imageResult = await _imageProxyService.GetImageStreamAsync(fileKey, cancellationToken);
			if (imageResult.IsFailure)
			{
				return (ActionResult)_actionResultErrorHandler.GetActionResultFromError(imageResult.ErrorObject!);
			}

			var image = imageResult.Value;

			// Verificar ETag para cache
			if (!string.IsNullOrEmpty(ifNoneMatch) &&
				!string.IsNullOrEmpty(image.ETag) &&
				ifNoneMatch.Trim('"') == image.ETag)
			{
				_logger.LogInformation("Imagem não modificada (cache): {FileKey}", fileKey);
				return StatusCode(StatusCodes.Status304NotModified);
			}

			// Configurar headers de cache
			Response.Headers[HeaderNames.CacheControl] = "private, max-age=3600"; // Cache por 1 hora
			Response.Headers[HeaderNames.LastModified] = image.LastModified.Value.ToString("R");

			if (!string.IsNullOrEmpty(image.ETag))
			{
				Response.Headers[HeaderNames.ETag] = $"\"{image.ETag}\"";
			}

			// Configurar headers de segurança
			Response.Headers["X-Content-Type-Options"] = "nosniff";
			Response.Headers["X-Frame-Options"] = "SAMEORIGIN"; // Permitir iframe no mesmo domínio
			Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
			Response.Headers["X-Robots-Tag"] = "noindex, nofollow"; // Não indexar imagens

			_logger.LogInformation("Servindo imagem: {FileKey}, Size: {Size} bytes, ContentType: {ContentType}",
				fileKey, image.ContentLength, image.ContentType);

			// Retornar stream da imagem
			return File(
				image.Stream,
				image.ContentType,
				enableRangeProcessing: true
			);
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
			if (string.IsNullOrWhiteSpace(fileKey))
			{
				return BadRequest();
			}

			var existsResult = await _imageProxyService.FileExistsAsync(fileKey, cancellationToken);
			if (existsResult.IsFailure)
			{
				return (ActionResult)_actionResultErrorHandler.GetActionResultFromError(existsResult.ErrorObject!);
			}

			if (existsResult.Value)
			{
				Response.Headers[HeaderNames.CacheControl] = "private, max-age=3600";
				return Ok();
			}

			return NotFound();
		}
	}
}

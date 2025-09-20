using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.AspNet.Infrastructure.ErrorHandling;
using Kickoffa.API.Contracts.FileUpload;
using Kickoffa.API.Domain.Interfaces.ProcessResult;
using Kickoffa.API.Domain.ProcessResult;
using Kickoffa.API.Helpers;
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
        private readonly IErrorFactory _errorFactory;
        private readonly IFileUploadService _fileUploadService;
        private readonly IImageProxyService _imageProxyService;
        private readonly ILogger<FileUploadController> _logger;
        private readonly IResultToActionResultConverter _resultToActionResultConverter;

        public FileUploadController(
            IErrorFactory errorFactory,
            IFileUploadService fileUploadService,
            IImageProxyService imageProxyService,
            IResultToActionResultConverter resultToActionResultConverter,
            ILogger<FileUploadController> logger)
        {
            _errorFactory = errorFactory;
            _fileUploadService = fileUploadService;
            _imageProxyService = imageProxyService;
            _resultToActionResultConverter = resultToActionResultConverter;
            _logger = logger;
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
        public async Task<IActionResult> UploadBriefingImage(IFormFile file, CancellationToken cancellationToken)
        {
            string? s3FileUrl = null;
            string? proxyUrl = null;

            if (file == null)
                return BadRequest(_errorFactory.CreateBadRequest("Nenhum arquivo foi enviado"));

            _logger.LogInformation("Iniciando upload de imagem para briefing: {FileName}", file.FileName);

            var result = await _fileUploadService
                .UploadImageAsync(file, "briefing/images", cancellationToken)
                .Bind(s3Url =>
                {
                    s3FileUrl = s3Url;
                    return _imageProxyService.ExtractFileKeyFromUrl(s3Url);
                })
                .Bind(fileKey =>
                {
                    proxyUrl = fileKey;
                    return _imageProxyService.GenerateProxyUrl(fileKey);
                })
                .Map(proxyUrl => new FileUploadResponse
                {
                    Url = proxyUrl,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Size = file.Length
                });

            _logger.LogInformation("Upload de imagem concluído com sucesso. S3: {S3Url}, Proxy: {ProxyUrl}",
                s3FileUrl, proxyUrl);

            return _resultToActionResultConverter.Convert(result);
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
            [FromQuery] string folder,
            CancellationToken cancellationToken)
        {
            string? s3FileUrl = null;
            string? proxyUrl = null;

            if (file == null)
                return BadRequest(_errorFactory.CreateBadRequest("Nenhum arquivo foi enviado"));

            if (string.IsNullOrWhiteSpace(folder))
                folder = "uploads";

            _logger.LogInformation("Iniciando upload de arquivo: {FileName} para pasta: {Folder}", file.FileName, folder);

            var result = await _fileUploadService
                .UploadFileAsync(file, folder, cancellationToken)
                .Bind(s3Url =>
                {
                    s3FileUrl = s3Url;
                    return _imageProxyService.ExtractFileKeyFromUrl(s3Url);
                })
                .Bind(fileKey => _imageProxyService.GenerateProxyUrl(fileKey))
                .Map(proxyUrlValue =>
                {
                    proxyUrl = proxyUrlValue;
                    return new FileUploadResponse
                    {
                        Url = proxyUrlValue,
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        Size = file.Length
                    };
                });

            _logger.LogInformation("Upload de arquivo concluído com sucesso. S3: {S3Url}, Proxy: {ProxyUrl}",
                s3FileUrl, proxyUrl);

            return _resultToActionResultConverter.Convert(result);
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
            [FromQuery] int expirationMinutes,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(fileKey))
                return BadRequest(_errorFactory.CreateBadRequest("Chave do arquivo é obrigatória"));

            var result = await _fileUploadService
                .GeneratePresignedUrlAsync(fileKey, expirationMinutes)
                .Map(url => new PresignedUrlResponse
                {
                    Url = url,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                    FileKey = fileKey
                });

            return _resultToActionResultConverter.Convert(result);
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
            if (string.IsNullOrWhiteSpace(fileUrl))
                return BadRequest(_errorFactory.CreateBadRequest("URL do arquivo é obrigatória"));

            _logger.LogInformation("Iniciando remoção de arquivo: {FileUrl}", fileUrl);

            var result = await _fileUploadService
                .DeleteFileAsync(fileUrl, cancellationToken)
                .Bind(success =>
                {
                    if (success)
                    {
                        _logger.LogInformation("Arquivo removido com sucesso: {FileUrl}", fileUrl);
                        return Result<object>.Success(new { message = "Arquivo removido com sucesso" });
                    }

                    _logger.LogWarning("Arquivo não encontrado ou não pôde ser removido: {FileUrl}", fileUrl);
                    return Result<object>.Failure("Arquivo não encontrado ou não pôde ser removido");
                });

            return _resultToActionResultConverter.Convert(result);
        }
    }
}
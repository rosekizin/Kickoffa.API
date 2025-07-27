using Amazon.S3;
using Amazon.S3.Model;
using Kickoffa.API.Application.Configuration;
using Kickoffa.API.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace Kickoffa.API.Application.Services
{
	/// <summary>
	/// Serviço para servir imagens privadas do S3 através de proxy autenticado
	/// </summary>
	public class ImageProxyService : IImageProxyService
	{
		private readonly IAmazonS3 _s3Client;
		private readonly AwsS3Configuration _s3Config;
		private readonly ILogger<ImageProxyService> _logger;

		public ImageProxyService(
			IAmazonS3 s3Client,
			IOptions<AwsS3Configuration> s3Config,
			ILogger<ImageProxyService> logger)
		{
			_s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
			_s3Config = s3Config?.Value ?? throw new ArgumentNullException(nameof(s3Config));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc/>
		public async Task<ImageStreamResult?> GetImageStreamAsync(string fileKey, CancellationToken cancellationToken = default)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(fileKey))
				{
					_logger.LogWarning("Chave do arquivo está vazia");
					return null;
				}

				_logger.LogInformation("Obtendo imagem do S3: {FileKey}", fileKey);

				var request = new GetObjectRequest
				{
					BucketName = _s3Config.BucketName,
					Key = fileKey
				};

				var response = await _s3Client.GetObjectAsync(request, cancellationToken);

				if (response.HttpStatusCode != HttpStatusCode.OK)
				{
					_logger.LogWarning("Arquivo não encontrado no S3: {FileKey}, Status: {Status}", 
						fileKey, response.HttpStatusCode);
					return null;
				}

				var result = new ImageStreamResult
				{
					Stream = response.ResponseStream,
					ContentType = response.Headers.ContentType ?? "application/octet-stream",
					ContentLength = response.Headers.ContentLength,
					LastModified = response.LastModified,
					ETag = response.ETag?.Trim('"')
				};

				_logger.LogInformation("Imagem obtida com sucesso: {FileKey}, Size: {Size} bytes", 
					fileKey, result.ContentLength);

				return result;
			}
			catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
			{
				_logger.LogWarning("Arquivo não encontrado no S3: {FileKey}", fileKey);
				return null;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao obter imagem do S3: {FileKey}", fileKey);
				throw;
			}
		}

		/// <inheritdoc/>
		public async Task<bool> FileExistsAsync(string fileKey, CancellationToken cancellationToken = default)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(fileKey))
					return false;

				var request = new GetObjectMetadataRequest
				{
					BucketName = _s3Config.BucketName,
					Key = fileKey
				};

				await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
				return true;
			}
			catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
			{
				return false;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao verificar existência do arquivo: {FileKey}", fileKey);
				return false;
			}
		}

		/// <inheritdoc/>
		public string? ExtractFileKeyFromUrl(string url)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(url))
					return null;

				// Se for uma URL do proxy, extrair a chave do parâmetro
				if (url.Contains("/api/images/"))
				{
					var uri = new Uri(url);
					var segments = uri.Segments;
					
					// Formato esperado: /api/images/{fileKey}
					if (segments.Length >= 3 && segments[1] == "api/" && segments[2] == "images/")
					{
						return string.Join("", segments.Skip(3)).TrimEnd('/');
					}
				}

				// Se for uma URL direta do S3, extrair da URL
				var baseUrl = _s3Config.BaseUrl?.TrimEnd('/');
				if (!string.IsNullOrEmpty(baseUrl) && url.StartsWith(baseUrl))
				{
					return url.Substring(baseUrl.Length + 1);
				}

				// Tentar extrair de URL padrão do S3
				var s3Uri = new Uri(url);
				return s3Uri.AbsolutePath.TrimStart('/');
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao extrair chave da URL: {Url}", url);
				return null;
			}
		}

		/// <inheritdoc/>
		public string GenerateProxyUrl(string fileKey)
		{
			if (string.IsNullOrWhiteSpace(fileKey))
				throw new ArgumentException("Chave do arquivo não pode estar vazia", nameof(fileKey));

			// Remover barras iniciais se houver
			fileKey = fileKey.TrimStart('/');

			// Gerar URL absoluta do proxy usando configuração
			var baseUrl = _s3Config.ApiBaseUrl?.TrimEnd('/') ?? "http://localhost:5084";
			var proxyUrl = $"{baseUrl}/api/images/{fileKey}";

			_logger.LogInformation("URL do proxy gerada: {ProxyUrl} para chave: {FileKey}", proxyUrl, fileKey);

			return proxyUrl;
		}
	}
}

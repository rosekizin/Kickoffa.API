using Kickoffa.API.Domain.Interfaces.ProcessResult;

namespace Kickoffa.API.Application.Interfaces
{
	/// <summary>
	/// Serviço para servir imagens privadas do S3 através de proxy autenticado
	/// </summary>
	public interface IImageProxyService
	{
		/// <summary>
		/// Obtém uma imagem do S3 como stream
		/// </summary>
		/// <param name="fileKey">Chave do arquivo no S3</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Result contendo stream da imagem e informações do arquivo ou erro</returns>
		Task<IResult<ImageStreamResult>> GetImageStreamAsync(string fileKey, CancellationToken cancellationToken);

		/// <summary>
		/// Verifica se um arquivo existe no S3
		/// </summary>
		/// <param name="fileKey">Chave do arquivo no S3</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Result contendo true se o arquivo existe ou erro</returns>
		Task<IResult<bool>> FileExistsAsync(string fileKey, CancellationToken cancellationToken);

		/// <summary>
		/// Extrai a chave do arquivo de uma URL
		/// </summary>
		/// <param name="url">URL da imagem</param>
		/// <returns>Result contendo a chave do arquivo ou erro se inválida</returns>
		IResult<string> ExtractFileKeyFromUrl(string url);

		/// <summary>
		/// Gera URL do proxy para uma chave de arquivo
		/// </summary>
		/// <param name="fileKey">Chave do arquivo no S3</param>
		/// <returns>Result contendo a URL do proxy ou erro</returns>
		IResult<string> GenerateProxyUrl(string fileKey);
	}

	/// <summary>
	/// Resultado do stream de imagem
	/// </summary>
	public class ImageStreamResult
	{
		/// <summary>
		/// Stream da imagem
		/// </summary>
		public Stream Stream { get; set; } = null!;

		/// <summary>
		/// Tipo de conteúdo da imagem
		/// </summary>
		public string ContentType { get; set; } = string.Empty;

		/// <summary>
		/// Tamanho do arquivo em bytes
		/// </summary>
		public long ContentLength { get; set; }

		/// <summary>
		/// Data da última modificação
		/// </summary>
		public DateTime? LastModified { get; set; }

		/// <summary>
		/// ETag para cache
		/// </summary>
		public string? ETag { get; set; }
	}
}
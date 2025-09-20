using Kickoffa.API.Domain.ProcessResult;
using System.Net;

namespace Kickoffa.API.Application.MessageErrors
{
	/// <summary>
	/// Classe que define os tipos de erro que podem ocorrer no serviço de proxy de imagens
	/// </summary>
	public static class ImageProxyServiceErrors
	{
		private const string FILE_KEY_EMPTY = "Chave do arquivo não pode estar vazia";
		private const string FILE_NOT_FOUND = "Arquivo não encontrado no S3";
		private const string INVALID_URL = "A URL de arquivo fornecida é inválida. Não pode ser vazia ou nula.";
		private const string EXTRACT_FILE_KEY_FROM_URL_ERROR = "Erro ao extratir chave de arquivo a partir da url";
		private const string S3_ACCESS_ERROR = "Erro ao acessar arquivo no S3";
		private const string STREAM_ERROR = "Erro ao obter stream do arquivo";
		private const string FILE_EXISTS_ERROR = "Erro ao verificar existência do arquivo";

		/// <summary>
		/// Erro quando a chave do arquivo está vazia
		/// </summary>
		public static Error FileKeyEmpty() => new(
			nameof(FILE_KEY_EMPTY),
			FILE_KEY_EMPTY,
			HttpStatusCode.BadRequest
		);

		/// <summary>
		/// Erro quando arquivo não é encontrado no S3
		/// </summary>
		/// <param name="fileKey">Chave do arquivo</param>
		public static Error FileNotFound(string fileKey) => new(
            nameof(FILE_NOT_FOUND),
			FILE_NOT_FOUND,
			HttpStatusCode.NotFound,
			new Dictionary<string, object>
			{
				{ "fileKey", fileKey }
			}
		);

		/// <summary>
		/// URL de arquivo fornecida inválida
		/// </summary>
		/// <param name="url">URL inválida</param>
		public static Error InvalidUrl(string url) => new(
            nameof(INVALID_URL),
            INVALID_URL,
			HttpStatusCode.BadRequest,
			new Dictionary<string, object>
			{
				{ "url", url }
			}
		);

		/// <summary>
		/// Erro quando ao extrair chave de arquivo da URL fornecida
		/// </summary>
		/// <param name="url">URL inválida</param>
		public static Error ExtractFileKeyFromUrlError(string url) => new(
            nameof(EXTRACT_FILE_KEY_FROM_URL_ERROR),
            EXTRACT_FILE_KEY_FROM_URL_ERROR,
			HttpStatusCode.InternalServerError,
			new Dictionary<string, object>
			{
				{ "url", url }
			}
		);

		/// <summary>
		/// Erro genérico de acesso ao S3
		/// </summary>
		/// <param name="fileKey">Chave do arquivo</param>
		/// <param name="innerException">Exceção interna</param>
		public static Error S3AccessError(string fileKey, Exception? innerException = null) => new(
            nameof(S3_ACCESS_ERROR),
			S3_ACCESS_ERROR,
			HttpStatusCode.InternalServerError,
			new Dictionary<string, object>
			{
				{ "fileKey", fileKey },
				{ "innerException", innerException?.Message ?? "N/A" }
			}
		);

		/// <summary>
		/// Erro ao obter stream do arquivo
		/// </summary>
		/// <param name="fileKey">Chave do arquivo</param>
		/// <param name="innerException">Exceção interna</param>
		public static Error StreamError(string fileKey, Exception? innerException = null) => new(
            nameof(STREAM_ERROR),
            STREAM_ERROR,
			HttpStatusCode.InternalServerError,
			new Dictionary<string, object>
			{
				{ "fileKey", fileKey },
				{ "innerException", innerException?.Message ?? "N/A" }
			}
		);

		/// <summary>
		/// Erro ao verificar existenncia de arquivo
		/// </summary>
		/// <param name="fileKey">Chave do arquivo</param>
		/// <param name="innerException">Exceção interna</param>
		public static Error FileExistsError(string fileKey, Exception? innerException = null) => new(
             nameof(FILE_EXISTS_ERROR),
            FILE_EXISTS_ERROR,
			HttpStatusCode.InternalServerError,
			new Dictionary<string, object>
			{
				{ "fileKey", fileKey },
				{ "innerException", innerException?.Message ?? "N/A" }
			}
		);
	}
}
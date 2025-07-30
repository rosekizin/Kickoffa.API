using Kickoffa.API.Domain.ProcessResult;
using System.Net;

namespace Kickoffa.API.Application.MessageErrors
{
	/// <summary>
	/// Classe que define os tipos de erro que podem ocorrer no upload de arquivos
	/// </summary>
	public static class FileUploadServiceErrors
	{
		private const string FILE_NULL = "Nenhum arquivo foi enviado";
		private const string FILE_EMPTY = "Arquivo está vazio";
		private const string FILE_TOO_LARGE = "Arquivo muito grande. Tamanho máximo: {0}MB";
		private const string FILE_NAME_REQUIRED = "Nome do arquivo é obrigatório";
		private const string INVALID_IMAGE_EXTENSION = "Extensão de arquivo não permitida: {0}. Extensões permitidas: {1}";
		private const string INVALID_IMAGE_CONTENT_TYPE = "Tipo de conteúdo não permitido: {0}. Tipos permitidos: {1}";
		private const string UPLOAD_FAILED = "Falha no upload. Status: {0}";
		private const string S3_UPLOAD_ERROR = "Erro ao fazer upload do arquivo para S3";
		private const string PRESIGNED_URL_ERROR = "Erro ao gerar URL pré-assinada";

		/// <summary>
		/// Erro quando nenhum arquivo é enviado
		/// </summary>
		public static Error FileNull() => new(
			nameof(FILE_NULL),
			FILE_NULL,
			HttpStatusCode.BadRequest
		);

		/// <summary>
		/// Erro quando arquivo está vazio
		/// </summary>
		public static Error FileEmpty() => new(
            nameof(FILE_EMPTY),
			FILE_EMPTY,
			HttpStatusCode.BadRequest
		);

		/// <summary>
		/// Erro quando arquivo é muito grande
		/// </summary>
		/// <param name="maxSizeMB">Tamanho máximo permitido em MB</param>
		public static Error FileTooLarge(int maxSizeMB) => new(
            nameof(FILE_TOO_LARGE),
			string.Format(FILE_TOO_LARGE, maxSizeMB),
			HttpStatusCode.BadRequest
		);

		/// <summary>
		/// Erro quando nome do arquivo é obrigatório
		/// </summary>
		public static Error FileNameRequired() => new(
            nameof(FILE_NAME_REQUIRED),
			FILE_NAME_REQUIRED,
			HttpStatusCode.BadRequest
		);

		/// <summary>
		/// Erro quando extensão de imagem não é permitida
		/// </summary>
		/// <param name="extension">Extensão do arquivo</param>
		/// <param name="allowedExtensions">Extensões permitidas</param>
		public static Error InvalidImageExtension(string extension, string allowedExtensions) => new(
            nameof(INVALID_IMAGE_EXTENSION),
			string.Format(INVALID_IMAGE_EXTENSION, extension, allowedExtensions),
			HttpStatusCode.BadRequest
		);

		/// <summary>
		/// Erro quando tipo de conteúdo de imagem não é permitido
		/// </summary>
		/// <param name="contentType">Tipo de conteúdo do arquivo</param>
		/// <param name="allowedContentTypes">Tipos de conteúdo permitidos</param>
		public static Error InvalidImageContentType(string contentType, string allowedContentTypes) => new(
            nameof(INVALID_IMAGE_CONTENT_TYPE),
			string.Format(INVALID_IMAGE_CONTENT_TYPE, contentType, allowedContentTypes),
			HttpStatusCode.BadRequest
		);

		/// <summary>
		/// Erro quando upload falha
		/// </summary>
		/// <param name="statusCode">Status code retornado pelo S3</param>
		public static Error UploadFailed(string statusCode) => new(
            nameof(UPLOAD_FAILED),
			string.Format(UPLOAD_FAILED, statusCode),
			HttpStatusCode.InternalServerError
		);

		/// <summary>
		/// Erro genérico de upload para S3
		/// </summary>
		/// <param name="fileName">Nome do arquivo</param>
		/// <param name="innerException">Exceção interna</param>
		public static Error S3UploadError(string fileName, Exception? innerException = null) => new(
            nameof(S3_UPLOAD_ERROR),
			S3_UPLOAD_ERROR,
			HttpStatusCode.InternalServerError,
			new Dictionary<string, object>
			{
				{ "fileName", fileName },
				{ "innerException", innerException?.Message ?? "N/A" }
			}
		);

		/// <summary>
		/// Erro ao gerar URL pré-assinada
		/// </summary>
		/// <param name="fileKey">Chave do arquivo</param>
		/// <param name="innerException">Exceção interna</param>
		public static Error PresignedUrlError(string fileKey, Exception? innerException = null) => new(
            nameof(PRESIGNED_URL_ERROR),
			PRESIGNED_URL_ERROR,
			HttpStatusCode.InternalServerError,
			new Dictionary<string, object>
			{
				{ "fileKey", fileKey },
				{ "innerException", innerException?.Message ?? "N/A" }
			}
		);
	}
}
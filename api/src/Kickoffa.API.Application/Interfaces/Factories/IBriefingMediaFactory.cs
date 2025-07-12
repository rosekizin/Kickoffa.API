using Kickoffa.API.Domain.Interfaces.Models;

namespace Kickoffa.API.Application.Interfaces.Factories
{
	/// <summary>
	/// Factory para criação de mídias de briefing
	/// </summary>
	public interface IBriefingMediaFactory
	{
		/// <summary>
		/// Cria uma nova mídia de briefing
		/// </summary>
		public IBriefingMedia CreateBriefingMedia(
			long sectionId,
			string fileName,
			string storagePath,
			string url,
			string contentType,
			long fileSize,
			int? width = null,
			int? height = null,
			string? altText = null);

		/// <summary>
		/// Cria uma mídia de imagem com dimensões
		/// </summary>
		IBriefingMedia CreateImageMedia(
			long sectionId,
			string fileName,
			string storagePath,
			string url,
			string contentType,
			long fileSize,
			int width,
			int height,
			string? altText = null);

		/// <summary>
		/// Cria uma mídia de documento (sem dimensões)
		/// </summary>
		IBriefingMedia CreateDocumentMedia(
			long sectionId,
			string fileName,
			string storagePath,
			string url,
			string contentType,
			long fileSize);

		/// <summary>
		/// Cria uma mídia de vídeo com dimensões
		/// </summary>
		IBriefingMedia CreateVideoMedia(
			long sectionId,
			string fileName,
			string storagePath,
			string url,
			string contentType,
			long fileSize,
			int? width = null,
			int? height = null);

		/// <summary>
		/// Cria múltiplas mídias a partir de uma lista de arquivos
		/// </summary>
		List<IBriefingMedia> CreateMultipleMedia(
			long sectionId,
			IEnumerable<(string fileName, string storagePath, string url, string contentType, long fileSize, int? width, int? height)> mediaData);

		/// <summary>
		/// Valida os dados de uma mídia antes da criação
		/// </summary>
		bool ValidateMediaData(
			string fileName,
			string storagePath,
			string url,
			string contentType,
			long fileSize,
			out string? errorMessage);

		/// <summary>
		/// Verifica se é um tipo de mídia de imagem
		/// </summary>
		bool IsImageContentType(string contentType);

		/// <summary>
		/// Verifica se é um tipo de mídia de vídeo
		/// </summary>
		bool IsVideoContentType(string contentType);

		/// <summary>
		/// Verifica se é um tipo de mídia de áudio
		/// </summary>
		bool IsAudioContentType(string contentType);

		/// <summary>
		/// Obtém a extensão do arquivo a partir do nome
		/// </summary>
		string GetFileExtension(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
				return string.Empty;

			return Path.GetExtension(fileName).ToLowerInvariant();
		}

		/// <summary>
		/// Calcula o tamanho formatado do arquivo
		/// </summary>
		string FormatFileSize(long fileSizeBytes);
	}
}
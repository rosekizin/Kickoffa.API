using Kickoffa.API.Domain.Models;

namespace Kickoffa.API.Domain.Factories
{
	/// <summary>
	/// Factory para criação de mídias de briefing
	/// </summary>
	public static class BriefingMediaFactory
	{
		/// <summary>
		/// Cria uma nova mídia de briefing
		/// </summary>
		public static BriefingMedia CreateBriefingMedia(
			long sectionId,
			string fileName,
			string storagePath,
			string url,
			string contentType,
			long fileSize,
			int? width = null,
			int? height = null,
			string? altText = null)
		{
			return new BriefingMedia(
				sectionId,
				fileName,
				storagePath,
				url,
				contentType,
				fileSize,
				width,
				height,
				altText
			);
		}

		/// <summary>
		/// Cria uma mídia de imagem com dimensões
		/// </summary>
		public static BriefingMedia CreateImageMedia(
			long sectionId,
			string fileName,
			string storagePath,
			string url,
			string contentType,
			long fileSize,
			int width,
			int height,
			string? altText = null)
		{
			return new BriefingMedia(
				sectionId,
				fileName,
				storagePath,
				url,
				contentType,
				fileSize,
				width,
				height,
				altText ?? GenerateDefaultAltText(fileName)
			);
		}

		/// <summary>
		/// Cria uma mídia de documento (sem dimensões)
		/// </summary>
		public static BriefingMedia CreateDocumentMedia(
			long sectionId,
			string fileName,
			string storagePath,
			string url,
			string contentType,
			long fileSize)
		{
			return new BriefingMedia(
				sectionId,
				fileName,
				storagePath,
				url,
				contentType,
				fileSize
			);
		}

		/// <summary>
		/// Cria uma mídia de vídeo com dimensões
		/// </summary>
		public static BriefingMedia CreateVideoMedia(
			long sectionId,
			string fileName,
			string storagePath,
			string url,
			string contentType,
			long fileSize,
			int? width = null,
			int? height = null)
		{
			return new BriefingMedia(
				sectionId,
				fileName,
				storagePath,
				url,
				contentType,
				fileSize,
				width,
				height,
				$"Vídeo: {Path.GetFileNameWithoutExtension(fileName)}"
			);
		}

		/// <summary>
		/// Cria múltiplas mídias a partir de uma lista de arquivos
		/// </summary>
		public static List<BriefingMedia> CreateMultipleMedia(
			long sectionId,
			IEnumerable<(string fileName, string storagePath, string url, string contentType, long fileSize, int? width, int? height)> mediaData)
		{
			var mediaList = new List<BriefingMedia>();

			foreach (var data in mediaData)
			{
				var media = CreateBriefingMedia(
					sectionId,
					data.fileName,
					data.storagePath,
					data.url,
					data.contentType,
					data.fileSize,
					data.width,
					data.height
				);

				mediaList.Add(media);
			}

			return mediaList;
		}

		/// <summary>
		/// Valida os dados de uma mídia antes da criação
		/// </summary>
		public static bool ValidateMediaData(
			string fileName,
			string storagePath,
			string url,
			string contentType,
			long fileSize,
			out string? errorMessage)
		{
			errorMessage = null;

			if (string.IsNullOrWhiteSpace(fileName))
			{
				errorMessage = "Nome do arquivo é obrigatório";
				return false;
			}

			if (string.IsNullOrWhiteSpace(storagePath))
			{
				errorMessage = "Caminho de armazenamento é obrigatório";
				return false;
			}

			if (string.IsNullOrWhiteSpace(url))
			{
				errorMessage = "URL é obrigatória";
				return false;
			}

			if (string.IsNullOrWhiteSpace(contentType))
			{
				errorMessage = "Tipo de conteúdo é obrigatório";
				return false;
			}

			if (fileSize <= 0)
			{
				errorMessage = "Tamanho do arquivo deve ser maior que zero";
				return false;
			}

			if (!IsValidUrl(url))
			{
				errorMessage = "URL inválida";
				return false;
			}

			return true;
		}

		/// <summary>
		/// Verifica se é um tipo de mídia de imagem
		/// </summary>
		public static bool IsImageContentType(string contentType)
		{
			if (string.IsNullOrWhiteSpace(contentType))
				return false;

			return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Verifica se é um tipo de mídia de vídeo
		/// </summary>
		public static bool IsVideoContentType(string contentType)
		{
			if (string.IsNullOrWhiteSpace(contentType))
				return false;

			return contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Verifica se é um tipo de mídia de áudio
		/// </summary>
		public static bool IsAudioContentType(string contentType)
		{
			if (string.IsNullOrWhiteSpace(contentType))
				return false;

			return contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Gera um texto alternativo padrão baseado no nome do arquivo
		/// </summary>
		private static string GenerateDefaultAltText(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
				return "Imagem";

			var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			
			// Substituir underscores e hífens por espaços
			nameWithoutExtension = nameWithoutExtension.Replace('_', ' ').Replace('-', ' ');
			
			// Capitalizar primeira letra
			if (nameWithoutExtension.Length > 0)
			{
				nameWithoutExtension = char.ToUpper(nameWithoutExtension[0]) + nameWithoutExtension[1..];
			}

			return nameWithoutExtension;
		}

		/// <summary>
		/// Valida se uma URL é válida
		/// </summary>
		private static bool IsValidUrl(string url)
		{
			return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
				   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
		}

		/// <summary>
		/// Obtém a extensão do arquivo a partir do nome
		/// </summary>
		public static string GetFileExtension(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
				return string.Empty;

			return Path.GetExtension(fileName).ToLowerInvariant();
		}

		/// <summary>
		/// Calcula o tamanho formatado do arquivo
		/// </summary>
		public static string FormatFileSize(long fileSizeBytes)
		{
			string[] sizes = ["B", "KB", "MB", "GB", "TB"];
			double len = fileSizeBytes;
			int order = 0;

			while (len >= 1024 && order < sizes.Length - 1)
			{
				order++;
				len = len / 1024;
			}

			return $"{len:0.##} {sizes[order]}";
		}
	}
}
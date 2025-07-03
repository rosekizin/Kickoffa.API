using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Items;

namespace Kickoffa.API.Domain.Factories
{
	/// <summary>
	/// Factory para criação de tipos de arquivo
	/// </summary>
	public static class FileTypeFactory
	{
		/// <summary>
		/// Cria um novo tipo de arquivo
		/// </summary>
		public static FileType CreateFileType(
			string mimeType,
			string extension,
			string displayName,
			FileTypeCategory category,
			string? description = null,
			int? recommendedMaxSizeMB = null,
			int displayOrder = 0,
			bool isActive = true)
		{
			return new FileType(
				mimeType,
				extension,
				displayName,
				category,
				description,
				recommendedMaxSizeMB,
				displayOrder,
				isActive
			);
		}

		/// <summary>
		/// Cria tipos de arquivo padrão para imagens
		/// </summary>
		public static List<FileType> CreateImageFileTypes()
		{
			return new List<FileType>
			{
				CreateFileType("image/jpeg", ".jpg", "Imagem JPEG", FileTypeCategory.Image, 
					"Formato de imagem comprimida com boa qualidade", 10, 1),
				CreateFileType("image/jpeg", ".jpeg", "Imagem JPEG", FileTypeCategory.Image, 
					"Formato de imagem comprimida com boa qualidade", 10, 2),
				CreateFileType("image/png", ".png", "Imagem PNG", FileTypeCategory.Image, 
					"Formato de imagem sem perda com suporte a transparência", 15, 3),
				CreateFileType("image/gif", ".gif", "Imagem GIF", FileTypeCategory.Image, 
					"Formato de imagem com suporte a animação", 5, 4),
				CreateFileType("image/webp", ".webp", "Imagem WebP", FileTypeCategory.Image, 
					"Formato moderno de imagem com excelente compressão", 8, 5),
				CreateFileType("image/svg+xml", ".svg", "Imagem SVG", FileTypeCategory.Image, 
					"Formato vetorial escalável", 2, 6)
			};
		}

		/// <summary>
		/// Cria tipos de arquivo padrão para documentos
		/// </summary>
		public static List<FileType> CreateDocumentFileTypes()
		{
			return new List<FileType>
			{
				CreateFileType("application/pdf", ".pdf", "Documento PDF", FileTypeCategory.Document, 
					"Formato de documento portátil", 25, 10),
				CreateFileType("application/msword", ".doc", "Documento Word", FileTypeCategory.Document, 
					"Documento Microsoft Word (versão antiga)", 20, 11),
				CreateFileType("application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx", 
					"Documento Word", FileTypeCategory.Document, "Documento Microsoft Word", 20, 12),
				CreateFileType("text/plain", ".txt", "Arquivo de Texto", FileTypeCategory.Document, 
					"Arquivo de texto simples", 5, 13),
				CreateFileType("text/rtf", ".rtf", "Rich Text Format", FileTypeCategory.Document, 
					"Formato de texto rico", 10, 14)
			};
		}

		/// <summary>
		/// Cria tipos de arquivo padrão para design
		/// </summary>
		public static List<FileType> CreateDesignFileTypes()
		{
			return new List<FileType>
			{
				CreateFileType("application/x-photoshop", ".psd", "Arquivo Photoshop", FileTypeCategory.Design, 
					"Arquivo nativo do Adobe Photoshop", 100, 20),
				CreateFileType("application/illustrator", ".ai", "Arquivo Illustrator", FileTypeCategory.Design, 
					"Arquivo nativo do Adobe Illustrator", 50, 21),
				CreateFileType("application/x-indesign", ".indd", "Arquivo InDesign", FileTypeCategory.Design, 
					"Arquivo nativo do Adobe InDesign", 75, 22),
				CreateFileType("application/x-figma", ".fig", "Arquivo Figma", FileTypeCategory.Design, 
					"Arquivo do Figma", 30, 23),
				CreateFileType("application/x-sketch", ".sketch", "Arquivo Sketch", FileTypeCategory.Design, 
					"Arquivo do Sketch", 40, 24)
			};
		}

		/// <summary>
		/// Cria tipos de arquivo padrão para vídeo
		/// </summary>
		public static List<FileType> CreateVideoFileTypes()
		{
			return new List<FileType>
			{
				CreateFileType("video/mp4", ".mp4", "Vídeo MP4", FileTypeCategory.Video, 
					"Formato de vídeo padrão", 200, 30),
				CreateFileType("video/quicktime", ".mov", "Vídeo MOV", FileTypeCategory.Video, 
					"Formato QuickTime", 200, 31),
				CreateFileType("video/x-msvideo", ".avi", "Vídeo AVI", FileTypeCategory.Video, 
					"Formato Audio Video Interleave", 250, 32),
				CreateFileType("video/webm", ".webm", "Vídeo WebM", FileTypeCategory.Video, 
					"Formato web otimizado", 150, 33)
			};
		}

		/// <summary>
		/// Cria tipos de arquivo padrão para áudio
		/// </summary>
		public static List<FileType> CreateAudioFileTypes()
		{
			return new List<FileType>
			{
				CreateFileType("audio/mpeg", ".mp3", "Áudio MP3", FileTypeCategory.Audio, 
					"Formato de áudio comprimido", 50, 40),
				CreateFileType("audio/wav", ".wav", "Áudio WAV", FileTypeCategory.Audio, 
					"Formato de áudio sem perda", 100, 41),
				CreateFileType("audio/ogg", ".ogg", "Áudio OGG", FileTypeCategory.Audio, 
					"Formato de áudio livre", 60, 42),
				CreateFileType("audio/mp4", ".m4a", "Áudio M4A", FileTypeCategory.Audio, 
					"Formato de áudio AAC", 40, 43)
			};
		}

		/// <summary>
		/// Cria tipos de arquivo padrão para compressão
		/// </summary>
		public static List<FileType> CreateArchiveFileTypes()
		{
			return new List<FileType>
			{
				CreateFileType("application/zip", ".zip", "Arquivo ZIP", FileTypeCategory.Archive, 
					"Arquivo comprimido ZIP", 500, 50),
				CreateFileType("application/x-rar-compressed", ".rar", "Arquivo RAR", FileTypeCategory.Archive, 
					"Arquivo comprimido RAR", 500, 51),
				CreateFileType("application/x-7z-compressed", ".7z", "Arquivo 7Z", FileTypeCategory.Archive, 
					"Arquivo comprimido 7-Zip", 500, 52)
			};
		}

		/// <summary>
		/// Cria todos os tipos de arquivo padrão
		/// </summary>
		public static List<FileType> CreateAllDefaultFileTypes()
		{
			var allTypes = new List<FileType>();
			
			allTypes.AddRange(CreateImageFileTypes());
			allTypes.AddRange(CreateDocumentFileTypes());
			allTypes.AddRange(CreateDesignFileTypes());
			allTypes.AddRange(CreateVideoFileTypes());
			allTypes.AddRange(CreateAudioFileTypes());
			allTypes.AddRange(CreateArchiveFileTypes());
			
			return allTypes;
		}

		/// <summary>
		/// Verifica se uma extensão é válida
		/// </summary>
		public static bool IsValidExtension(string extension)
		{
			if (string.IsNullOrWhiteSpace(extension))
				return false;

			return extension.StartsWith('.') && extension.Length > 1;
		}

		/// <summary>
		/// Normaliza uma extensão de arquivo
		/// </summary>
		public static string NormalizeExtension(string extension)
		{
			if (string.IsNullOrWhiteSpace(extension))
				throw new ArgumentException("Extensão não pode ser vazia", nameof(extension));

			extension = extension.Trim().ToLowerInvariant();
			
			if (!extension.StartsWith('.'))
				extension = "." + extension;

			return extension;
		}

		/// <summary>
		/// Obtém a categoria sugerida baseada na extensão
		/// </summary>
		public static FileTypeCategory GetSuggestedCategory(string extension)
		{
			extension = NormalizeExtension(extension);

			return extension switch
			{
				".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".svg" or ".bmp" or ".tiff" => FileTypeCategory.Image,
				".pdf" or ".doc" or ".docx" or ".txt" or ".rtf" or ".odt" => FileTypeCategory.Document,
				".psd" or ".ai" or ".indd" or ".fig" or ".sketch" or ".xd" => FileTypeCategory.Design,
				".mp4" or ".mov" or ".avi" or ".webm" or ".mkv" or ".flv" => FileTypeCategory.Video,
				".mp3" or ".wav" or ".ogg" or ".m4a" or ".flac" or ".aac" => FileTypeCategory.Audio,
				".zip" or ".rar" or ".7z" or ".tar" or ".gz" => FileTypeCategory.Archive,
				_ => FileTypeCategory.Document // Padrão
			};
		}
	}
}

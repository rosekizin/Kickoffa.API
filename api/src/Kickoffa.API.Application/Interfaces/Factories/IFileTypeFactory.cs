using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Application.Interfaces.Factories
{
	/// <summary>
	/// Factory para criação de tipos de arquivo
	/// </summary>
	public interface IFileTypeFactory
	{
		/// <summary>	 
		/// Cria um novo tipo de arquivo	 
		/// </summary>
		public IFileType CreateFileType(
			string mimeType,
			string extension,
			string displayName,
			FileTypeCategory category,
			string? description = null,
			int? recommendedMaxSizeMB = null,
			int displayOrder = 0,
			bool isActive = true);

		/// <summary>
		/// Cria tipos de arquivo padrão para imagens
		/// </summary>
		public List<IFileType> CreateImageFileTypes();

		/// <summary>
		/// Cria tipos de arquivo padrão para documentos
		/// </summary>
		public List<IFileType> CreateDocumentFileTypes();

		/// <summary>
		/// Cria tipos de arquivo padrão para design
		/// </summary>
		public List<IFileType> CreateDesignFileTypes();

		/// <summary>
		/// Cria tipos de arquivo padrão para vídeo
		/// </summary>
		public List<IFileType> CreateVideoFileTypes();

		/// <summary>
		/// Cria tipos de arquivo padrão para áudio
		/// </summary>
		public List<IFileType> CreateAudioFileTypes();

		/// <summary>
		/// Cria tipos de arquivo padrão para compressão
		/// </summary>
		public List<IFileType> CreateArchiveFileTypes();

		/// <summary>
		/// Cria todos os tipos de arquivo padrão
		/// </summary>
		public List<IFileType> CreateAllDefaultFileTypes();

		/// <summary>
		/// Verifica se uma extensão é válida
		/// </summary>
		public bool IsValidExtension(string extension);

		/// <summary>
		/// Normaliza uma extensão de arquivo
		/// </summary>
		public string NormalizeExtension(string extension);

		/// <summary>
		/// Obtém a categoria sugerida baseada na extensão
		/// </summary>
		public FileTypeCategory GetSuggestedCategory(string extension);
	}
}
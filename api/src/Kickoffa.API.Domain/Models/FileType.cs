using Kickoffa.API.Domain.Models.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models
{
	/// <summary>
	/// Representa um tipo de arquivo suportado para upload
	/// </summary>
	public class FileType : BaseEntity<FileType>
	{
		/// <summary>
		/// MIME type do arquivo (ex: "image/jpeg", "application/pdf")
		/// </summary>
		public string MimeType { get; set; } = string.Empty;

		/// <summary>
		/// Extensão do arquivo (ex: ".jpg", ".pdf")
		/// </summary>
		public string Extension { get; set; } = string.Empty;

		/// <summary>
		/// Nome amigável do tipo (ex: "JPEG Image", "PDF Document")
		/// </summary>
		public string DisplayName { get; set; } = string.Empty;

		/// <summary>
		/// Descrição detalhada do tipo de arquivo
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// Categoria do arquivo para organização
		/// </summary>
		public FileTypeCategory Category { get; set; }

		/// <summary>
		/// Se este tipo está ativo/disponível para seleção
		/// </summary>
		public bool IsActive { get; set; } = true;

		/// <summary>
		/// Tamanho máximo recomendado em MB para este tipo
		/// </summary>
		public int? RecommendedMaxSizeMB { get; set; }

		/// <summary>
		/// Ordem de exibição na lista
		/// </summary>
		public int DisplayOrder { get; set; }
	}
}

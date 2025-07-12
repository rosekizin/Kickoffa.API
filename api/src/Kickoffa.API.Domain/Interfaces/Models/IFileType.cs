using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Interfaces.Models
{
	/// <summary>
	/// Representa um tipo de arquivo suportado para upload
	/// </summary>
	public interface IFileType : IBaseEntity
	{
		/// <summary>
		/// MIME type do arquivo (ex: "image/jpeg", "application/pdf")
		/// </summary>
		string MimeType { get; }

		/// <summary>
		/// Extensão do arquivo (ex: ".jpg", ".pdf")
		/// </summary>
		string Extension { get; }

		/// <summary>
		/// Nome amigável do tipo (ex: "JPEG Image", "PDF Document")
		/// </summary>
		string DisplayName { get; }

		/// <summary>
		/// Descrição detalhada do tipo de arquivo
		/// </summary>
		string? Description { get;}

		/// <summary>
		/// Categoria do arquivo para organização
		/// </summary>
		FileTypeCategory Category { get; }

		/// <summary>
		/// Se este tipo está ativo/disponível para seleção
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Tamanho máximo recomendado em MB para este tipo
		/// </summary>
		int? RecommendedMaxSizeMB { get; }

		/// <summary>
		/// Ordem de exibição na lista
		/// </summary>
		int DisplayOrder { get; }

		/// <summary>
		/// Atualiza o nome de exibição
		/// </summary>
		void UpdateDisplayName(string displayName);

		/// <summary>
		/// Atualiza a descrição
		/// </summary>
		void UpdateDescription(string? description);

		/// <summary>
		/// Atualiza o tamanho máximo recomendado
		/// </summary>
		void UpdateRecommendedMaxSize(int? maxSizeMB);

		/// <summary>
		/// Ativa o tipo de arquivo
		/// </summary>
		void Activate();

		/// <summary>
		/// Desativa o tipo de arquivo
		/// </summary>
		void Deactivate();

		/// <summary>
		/// Atualiza a ordem de exibição
		/// </summary>
		void UpdateDisplayOrder(int displayOrder);

		/// <summary>
		/// Verifica se o arquivo é compatível com este tipo
		/// </summary>
		bool IsCompatibleWith(string fileName, string? contentType = null);
	}
}
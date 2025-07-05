using Kickoffa.API.Domain.Models.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.Items
{
	/// <summary>
	/// Representa um tipo de arquivo suportado para upload
	/// </summary>
	public class FileType : BaseEntity
	{
		/// <summary>
		/// MIME type do arquivo (ex: "image/jpeg", "application/pdf")
		/// </summary>
		public string MimeType { get; private set; }

		/// <summary>
		/// Extensão do arquivo (ex: ".jpg", ".pdf")
		/// </summary>
		public string Extension { get; private set; }

		/// <summary>
		/// Nome amigável do tipo (ex: "JPEG Image", "PDF Document")
		/// </summary>
		public string DisplayName { get; private set; }

		/// <summary>
		/// Descrição detalhada do tipo de arquivo
		/// </summary>
		public string? Description { get; private set; }

		/// <summary>
		/// Categoria do arquivo para organização
		/// </summary>
		public FileTypeCategory Category { get; private set; }

		/// <summary>
		/// Se este tipo está ativo/disponível para seleção
		/// </summary>
		public bool IsActive { get; private set; }

		/// <summary>
		/// Tamanho máximo recomendado em MB para este tipo
		/// </summary>
		public int? RecommendedMaxSizeMB { get; private set; }

		/// <summary>
		/// Ordem de exibição na lista
		/// </summary>
		public int DisplayOrder { get; private set; }

		/// <summary>
		/// Construtor para criação de novo tipo de arquivo
		/// </summary>
		public FileType(string mimeType, string extension, string displayName, FileTypeCategory category,
			string? description = null, int? recommendedMaxSizeMB = null, int displayOrder = 0, bool isActive = true)
		{
			MimeType = mimeType ?? throw new ArgumentNullException(nameof(mimeType));
			Extension = extension ?? throw new ArgumentNullException(nameof(extension));
			DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
			Category = category;
			Description = description;
			RecommendedMaxSizeMB = recommendedMaxSizeMB;
			DisplayOrder = displayOrder;
			IsActive = isActive;
		}

		/// <summary>
		/// Construtor sem parâmetros para EF
		/// </summary>
		protected FileType()
		{
			MimeType = string.Empty;
			Extension = string.Empty;
			DisplayName = string.Empty;
		}

		/// <summary>
		/// Atualiza o nome de exibição
		/// </summary>
		public void UpdateDisplayName(string displayName)
		{
			DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Atualiza a descrição
		/// </summary>
		public void UpdateDescription(string? description)
		{
			Description = description;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Atualiza o tamanho máximo recomendado
		/// </summary>
		public void UpdateRecommendedMaxSize(int? maxSizeMB)
		{
			RecommendedMaxSizeMB = maxSizeMB;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Ativa o tipo de arquivo
		/// </summary>
		public void Activate()
		{
			IsActive = true;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Desativa o tipo de arquivo
		/// </summary>
		public void Deactivate()
		{
			IsActive = false;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Atualiza a ordem de exibição
		/// </summary>
		public void UpdateDisplayOrder(int displayOrder)
		{
			DisplayOrder = displayOrder;
			UpdateLastUpdatedDate();
		}

		/// <summary>
		/// Verifica se o arquivo é compatível com este tipo
		/// </summary>
		public bool IsCompatibleWith(string fileName, string? contentType = null)
		{
			if (string.IsNullOrWhiteSpace(fileName))
				return false;

			var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
			var isExtensionMatch = string.Equals(Extension, fileExtension, StringComparison.OrdinalIgnoreCase);

			if (!string.IsNullOrWhiteSpace(contentType))
			{
				var isContentTypeMatch = string.Equals(MimeType, contentType, StringComparison.OrdinalIgnoreCase);
				return isExtensionMatch && isContentTypeMatch;
			}

			return isExtensionMatch;
		}
	}
}
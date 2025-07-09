using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.Components
{
	/// <inheritdoc/>
	public class FileType : BaseEntity, IFileType
	{
		/// <inheritdoc/>
		public string MimeType { get; private set; }

		/// <inheritdoc/>
		public string Extension { get; private set; }

		/// <inheritdoc/>
		public string DisplayName { get; private set; }

		/// <inheritdoc/>
		public string? Description { get; private set; }

		/// <inheritdoc/>
		public FileTypeCategory Category { get; private set; }

		/// <inheritdoc/>
		public bool IsActive { get; private set; }

		/// <inheritdoc/>
		public int? RecommendedMaxSizeMB { get; private set; }

		/// <inheritdoc/>
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

		/// <inheritdoc/>
		public void UpdateDisplayName(string displayName)
		{
			DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void UpdateDescription(string? description)
		{
			Description = description;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void UpdateRecommendedMaxSize(int? maxSizeMB)
		{
			RecommendedMaxSizeMB = maxSizeMB;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void Activate()
		{
			IsActive = true;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void Deactivate()
		{
			IsActive = false;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
		public void UpdateDisplayOrder(int displayOrder)
		{
			DisplayOrder = displayOrder;
			UpdateLastUpdatedDate();
		}

		/// <inheritdoc/>
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
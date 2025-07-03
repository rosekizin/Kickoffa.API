using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Items.Base;

namespace Kickoffa.API.Domain.Models.Items
{
	/// <summary>
	/// Item para upload de arquivos pelo cliente
	/// </summary>
	public class UploadItem : Item
	{
		public UploadItem(
			long sectionId,
			string title,
			int order,
			string? description,
			bool isRequired,
			string? placeholder,
			int? maxSizeMB)
			: base(sectionId, title, order, description, isRequired)
		{
			MaxSizeMB = maxSizeMB;
			AllowedFileTypes = [];
			Placeholder = placeholder;
		}

		public override ItemType Type => ItemType.Upload;

		/*
		/// <summary>
		/// Tipos MIME permitidos para upload (ex: "image/jpeg,application/pdf")
		/// DEPRECATED: Usar AllowedFileTypes ao invés desta propriedade
		/// </summary>
		[Obsolete("Use AllowedFileTypes navigation property instead")]
		public string? AllowedMimeTypes { get; private set; }
		*/

		/// <summary>
		/// Tamanho máximo por arquivo em MB
		/// </summary>
		public int? MaxSizeMB { get; private set; }

		/// <summary>
		/// Texto de placeholder para a área de upload
		/// </summary>
		public string? Placeholder { get; private set; }

		/// <summary>
		/// Tipos de arquivo permitidos para este item de upload
		/// </summary>
		public ICollection<UploadItemFileType> AllowedFileTypes { get; private set; }
	}
}
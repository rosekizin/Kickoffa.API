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
			ItemFiles = [];
			AllowedFileTypes = [];
			Placeholder = placeholder;
		}

		public override ItemType Type => ItemType.Upload;

		/// <summary>
		/// Tamanho máximo por arquivo em MB
		/// </summary>
		public int? MaxSizeMB { get; private set; }

		/// <summary>
		/// Texto de placeholder para a área de upload
		/// </summary>
		public string? Placeholder { get; private set; }

		/// <summary>
		/// Arquivos enviados pelo cliente para este item
		/// </summary>
		public ICollection<UploadItemFile> ItemFiles { get; private set; }

		/// <summary>
		/// Tipos de arquivo permitidos para este item de upload
		/// </summary>
		public ICollection<UploadItemFileType> AllowedFileTypes { get; private set; }

		public void AddFiles(UploadItemFile file)
		{
			ItemFiles.Add(file);
		}
	}
}
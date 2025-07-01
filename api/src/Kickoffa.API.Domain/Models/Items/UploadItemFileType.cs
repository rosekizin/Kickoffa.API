using Kickoffa.API.Domain.Models.Base;

namespace Kickoffa.API.Domain.Models.Items
{
	/// <summary>
	/// Relacionamento many-to-many entre UploadItem e FileType
	/// </summary>
	public class UploadItemFileType : BaseEntity<UploadItemFileType>
	{
		public long UploadItemId { get; set; }
		public long FileTypeId { get; set; }

		// Relacionamentos
		public UploadItem UploadItem { get; set; } = null!;
		public FileType FileType { get; set; } = null!;
	}
}

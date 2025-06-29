using Kickoffa.API.Domain.Models.Base;
using Kickoffa.API.Domain.Models.Items;

namespace Kickoffa.API.Domain.Models
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

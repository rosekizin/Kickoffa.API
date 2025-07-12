using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Base;

namespace Kickoffa.API.Domain.Models.Components
{
	public class UploadComponentFile : BaseEntity, IUploadComponentFile
	{
		public UploadComponentFile(string fileName, string storagePath, long fileSize, string contentType, string sha256Hash)
		{
			FileName = fileName;
			StoragePath = storagePath;
			FileSize = fileSize;
			ContentType = contentType;
			Sha256Hash = sha256Hash;
		}

		// Para uploads
		public string FileName { get; private set; }
		public string StoragePath { get; private set; }
		public long FileSize { get; private set; }
		public string ContentType { get; private set; }
		public string Sha256Hash { get; private set; }

		/// <summary>
		/// Relacionamento com o UploadComponent
		/// </summary>
		public virtual UploadComponent UploadComponent { get; private set; } = null!;

		IUploadComponent IUploadComponentFile.UploadComponent => UploadComponent;
	}
}
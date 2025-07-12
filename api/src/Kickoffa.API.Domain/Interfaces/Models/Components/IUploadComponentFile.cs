namespace Kickoffa.API.Domain.Interfaces.Models.Components
{
	public interface IUploadComponentFile : IBaseEntity
	{
		// Para uploads
		string FileName { get; }
		string StoragePath { get; }
		long FileSize { get; }
		string ContentType { get; }
		string Sha256Hash { get; }

		/// <summary>
		/// Relacionamento com o UploadComponent
		/// </summary>
		IUploadComponent UploadComponent { get; }
	}
}
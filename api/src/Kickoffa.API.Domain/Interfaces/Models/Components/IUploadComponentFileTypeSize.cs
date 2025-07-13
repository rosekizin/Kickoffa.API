namespace Kickoffa.API.Domain.Interfaces.Models.Components
{
	/// <summary>
	/// Interface para configuração de tamanho máximo por tipo de arquivo em componentes de upload
	/// </summary>
	public interface IUploadComponentFileTypeSize : IBaseEntity
	{
		/// <summary>
		/// ID do componente de upload
		/// </summary>
		long UploadComponentId { get; }

		/// <summary>
		/// ID do tipo de arquivo
		/// </summary>
		long FileTypeId { get; }

		/// <summary>
		/// Tamanho máximo personalizado em MB para este tipo de arquivo
		/// </summary>
		int MaxSizeMB { get; }

		/// <summary>
		/// Relacionamento com o UploadComponent
		/// </summary>
		IUploadComponent UploadComponent { get; }

		/// <summary>
		/// Relacionamento com o FileType
		/// </summary>
		IFileType FileType { get; }
	}
}

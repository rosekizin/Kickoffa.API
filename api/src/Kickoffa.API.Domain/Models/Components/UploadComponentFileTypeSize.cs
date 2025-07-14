using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Base;

namespace Kickoffa.API.Domain.Models.Components
{
	/// <summary>
	/// Entidade para configuração de tamanho máximo por tipo de arquivo em componentes de upload
	/// </summary>
	public class UploadComponentFileTypeSize : BaseEntity, IUploadComponentFileTypeSize
	{
		/// <summary>
		/// Construtor para criação de nova configuração de tamanho
		/// </summary>
		/// <param name="uploadComponentId">ID do componente de upload</param>
		/// <param name="fileTypeId">ID do tipo de arquivo</param>
		/// <param name="maxSizeMB">Tamanho máximo em MB</param>
		public UploadComponentFileTypeSize(long uploadComponentId, long fileTypeId, int maxSizeMB)
		{
			UploadComponentId = uploadComponentId;
			FileTypeId = fileTypeId;
			MaxSizeMB = maxSizeMB;
		}

		/// <summary>
		/// Construtor protegido sem parâmetros para EF
		/// </summary>
		protected UploadComponentFileTypeSize()
		{
		}

		/// <summary>
		/// ID do componente de upload
		/// </summary>
		public long UploadComponentId { get; private set; }

		/// <summary>
		/// ID do tipo de arquivo
		/// </summary>
		public long FileTypeId { get; private set; }

		/// <summary>
		/// Tamanho máximo personalizado em MB para este tipo de arquivo
		/// </summary>
		public int MaxSizeMB { get; private set; }

		/// <summary>
		/// Relacionamento com o UploadComponent
		/// </summary>
		public virtual UploadComponent UploadComponent { get; private set; } = null!;
		IUploadComponent IUploadComponentFileTypeSize.UploadComponent => UploadComponent;

		/// <summary>
		/// Relacionamento com o FileType
		/// </summary>
		public virtual FileType FileType { get; private set; } = null!;
		IFileType IUploadComponentFileTypeSize.FileType => FileType;

		/// <summary>
		/// Atualiza o tamanho máximo
		/// </summary>
		/// <param name="maxSizeMB">Novo tamanho máximo em MB</param>
		public void UpdateMaxSize(int maxSizeMB)
		{
			MaxSizeMB = maxSizeMB;
			UpdateLastUpdatedDate();
		}
	}
}
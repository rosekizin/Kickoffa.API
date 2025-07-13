using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.Components
{
	/// <summary>
	/// Componente para upload de arquivos pelo cliente
	/// </summary>
	public class UploadComponent : Component, IUploadComponent
	{
		public UploadComponent(
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
			ComponentFiles = [];
			AllowedFileTypes = [];
			FileTypeSizeConfigs = [];
			Placeholder = placeholder;
		}

		public override ComponentType Type => ComponentType.Upload;

		/// <summary>
		/// Tamanho máximo por arquivo em MB
		/// </summary>
		public int? MaxSizeMB { get; private set; }

		/// <summary>
		/// Texto de placeholder para a área de upload
		/// </summary>
		public string? Placeholder { get; private set; }

		/// <summary>
		/// Arquivos enviados pelo cliente para este componente
		/// </summary>
		public virtual ICollection<UploadComponentFile> ComponentFiles { get; private set; }
		IEnumerable<IUploadComponentFile> IUploadComponent.ComponentFiles => ComponentFiles;

		/// <summary>
		/// Tipos de arquivo permitidos para este somponente de upload
		/// Relacionamento N:N unidirecional - UploadComponent conhece FileType, mas FileType não conhece UploadComponent
		/// </summary>
		public virtual ICollection<FileType> AllowedFileTypes { get; private set; }
		IEnumerable<IFileType> IUploadComponent.AllowedFileTypes => AllowedFileTypes;

		/// <summary>
		/// Configurações de tamanho máximo por tipo de arquivo
		/// </summary>
		public virtual ICollection<UploadComponentFileTypeSize> FileTypeSizeConfigs { get; private set; }
		IEnumerable<IUploadComponentFileTypeSize> IUploadComponent.FileTypeSizeConfigs => FileTypeSizeConfigs;

		public void AddFiles(IUploadComponentFile file)
		{
			ComponentFiles.Add((UploadComponentFile)file);
		}

		public void UpdateBasicProperties(int? maxSizeMB, string? placeholder)
		{
			MaxSizeMB = maxSizeMB;
			Placeholder = placeholder;
		}

		public void AddAllowedFileType(IFileType fileType)
		{
			AllowedFileTypes.Add((FileType)fileType);
		}

		public void AddFileTypeSizeConfig(IUploadComponentFileTypeSize config)
		{
			FileTypeSizeConfigs.Add((UploadComponentFileTypeSize)config);
		}

		public void UpdateFileTypeSizeConfig(long fileTypeId, int maxSizeMB)
		{
			var existingConfig = FileTypeSizeConfigs.FirstOrDefault(c => c.FileTypeId == fileTypeId);
			if (existingConfig != null)
			{
				existingConfig.UpdateMaxSize(maxSizeMB);
			}
			else
			{
				var newConfig = new UploadComponentFileTypeSize(Id, fileTypeId, maxSizeMB);
				FileTypeSizeConfigs.Add(newConfig);
			}
		}

		public void RemoveFileTypeSizeConfig(long fileTypeId)
		{
			var config = FileTypeSizeConfigs.FirstOrDefault(c => c.FileTypeId == fileTypeId);
			if (config != null)
			{
				FileTypeSizeConfigs.Remove(config);
			}
		}
	}
}
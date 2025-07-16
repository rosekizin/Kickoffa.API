using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components;

namespace Kickoffa.API.Application.Factories
{
	public class UploadComponentFileTypeSizeFactory : IUploadComponentFileTypeSizeFactory
	{
		public IUploadComponentFileTypeSize CreateFileTypeSize(
			long uploadComponentId,
			FileTypeSizeConfigRequest fileTypeSizeConfigRequest)
		{
			return new UploadComponentFileTypeSize(uploadComponentId, fileTypeSizeConfigRequest.FileTypeId, fileTypeSizeConfigRequest.MaxSizeMB);
		}
	}
}
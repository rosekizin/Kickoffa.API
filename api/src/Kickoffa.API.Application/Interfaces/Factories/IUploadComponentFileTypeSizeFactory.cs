using Kickoffa.API.Contracts.Checklist.Components;
using Kickoffa.API.Domain.Interfaces.Models.Components;

namespace Kickoffa.API.Application.Interfaces.Factories
{
	public interface IUploadComponentFileTypeSizeFactory
	{
		/// <summary>
		/// Cria a configuração de tamanho máximo por tipo de arquivo permitido no componente de upload
		/// </summary>
		/// <param name="fileTypeSizeConfigRequest">Request</param>
		/// <returns>Instância de UploadComponentFileTypeSize</returns>
		IUploadComponentFileTypeSize CreateFileTypeSize(long uploadComponentId, FileTypeSizeConfigRequest fileTypeSizeConfigRequest);
	}
}
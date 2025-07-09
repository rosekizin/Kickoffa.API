namespace Kickoffa.API.Domain.Interfaces.Models.Components
{
	public interface IUploadComponent : IComponent
	{
		/// <summary>
		/// Tamanho máximo por arquivo em MB
		/// </summary>
		int? MaxSizeMB { get; }

		/// <summary>
		/// Texto de placeholder para a área de upload
		/// </summary>
		string? Placeholder { get; }

		/// <summary>
		/// Arquivos enviados pelo cliente para este componente
		/// </summary>
		IEnumerable<IUploadComponentFile> ComponentFiles { get; }

		/// <summary>
		/// Tipos de arquivo permitidos para este somponente de upload
		/// Relacionamento N:N unidirecional - UploadComponent conhece FileType, mas FileType não conhece UploadComponent
		/// </summary>
		IEnumerable<IFileType> AllowedFileTypes { get; }

		void AddFiles(IUploadComponentFile file);
	}
}
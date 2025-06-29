using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Items.Base;

namespace Kickoffa.API.Domain.Models.Items
{
	/// <summary>
	/// Item para upload de arquivos pelo cliente
	/// </summary>
	public class UploadItem : Item
	{
		public override ItemType Type => ItemType.Upload;

		/// <summary>
		/// Tipos MIME permitidos para upload (ex: "image/jpeg,application/pdf")
		/// </summary>
		public string? AllowedMimeTypes { get; set; }

		/// <summary>
		/// Tamanho máximo por arquivo em MB
		/// </summary>
		public int? MaxSizeMB { get; set; }

		/// <summary>
		/// Texto de placeholder para a área de upload
		/// </summary>
		public string? Placeholder { get; set; }
	}
}

using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.Items
{
	/// <summary>
	/// Item de campo de texto para entrada de dados pelo cliente
	/// </summary>
	public class TextItem : Item
	{
		public override ItemType Type => ItemType.Text;

		/// <summary>
		/// Texto de placeholder para o campo
		/// </summary>
		public string? Placeholder { get; set; }

		/// <summary>
		/// Limite máximo de caracteres
		/// </summary>
		public int? MaxLength { get; set; }
	}
}

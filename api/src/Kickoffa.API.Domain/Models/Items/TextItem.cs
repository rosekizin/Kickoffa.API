using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Items.Base;

namespace Kickoffa.API.Domain.Models.Items
{
	/// <summary>
	/// Item de campo de texto para entrada de dados pelo cliente
	/// </summary>
	public class TextItem : Item
	{
		public TextItem(long sectionId, string title, int order, string? description, bool isRequired, string? placeholder, int? maxLenth)
			: base(sectionId, title, order, description, isRequired)
		{
			MaxLength = maxLenth;
			Placeholder = placeholder;
		}

		public override ItemType Type => ItemType.Text;

		/// <summary>
		/// Texto de placeholder para o campo
		/// </summary>
		public string? Placeholder { get; private set; }

		/// <summary>
		/// Limite máximo de caracteres
		/// </summary>
		public int? MaxLength { get; private set; }
	}
}
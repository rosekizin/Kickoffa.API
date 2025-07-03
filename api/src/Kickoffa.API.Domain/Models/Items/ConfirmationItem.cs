using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Items.Base;

namespace Kickoffa.API.Domain.Models.Items
{
	/// <summary>
	/// Item de confirmação com texto personalizado
	/// </summary>
	public class ConfirmationItem : Item
	{
		public ConfirmationItem(long sectionId, string title, int order, string? description, bool isRequired, string? confirmationText)
			: base(sectionId, title, order, description, isRequired)
		{
			ConfirmationText = confirmationText;
		}

		public override ItemType Type => ItemType.Confirmation;

		/// <summary>
		/// Texto de confirmação que será exibido junto com o checkbox
		/// </summary>
		public string? ConfirmationText { get; private set; }
	}
}
using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Components.Base;

namespace Kickoffa.API.Domain.Models.Components
{
	/// <summary>
	/// Componente de confirmação com texto personalizado
	/// </summary>
	public class ConfirmationComponent : Component
	{
		public ConfirmationComponent(long sectionId, string title, int order, string? description, bool isRequired, string? confirmationText)
			: base(sectionId, title, order, description, isRequired)
		{
			ConfirmationText = confirmationText;
		}

		public override ComponentType Type => ComponentType.Confirmation;

		/// <summary>
		/// Texto de confirmação que será exibido junto com o checkbox
		/// </summary>
		public string? ConfirmationText { get; private set; }
	}
}
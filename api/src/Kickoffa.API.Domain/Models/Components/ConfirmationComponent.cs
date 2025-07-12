using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.Components
{
	/// <inheritdoc/>
	public class ConfirmationComponent : Component, IConfirmationComponent
	{
		public ConfirmationComponent(long sectionId, string title, int order, string? description, bool isRequired, string? confirmationText)
			: base(sectionId, title, order, description, isRequired)
		{
			ConfirmationText = confirmationText;
		}

		public override ComponentType Type => ComponentType.Confirmation;

		/// <inheritdoc/>
		public string? ConfirmationText { get; private set; }

		/// <inheritdoc/>
		public void UpdateBasicProperties(string? confirmationText)
		{
			ConfirmationText = confirmationText;
		}
	}
}
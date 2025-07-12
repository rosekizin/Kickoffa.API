using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.Components
{
	/// <inheritdoc/>
	public class TextComponent : Component, ITextComponent
	{
		public TextComponent(long sectionId, string title, int order, string? description, bool isRequired, string? placeholder, int? maxLength)
			: base(sectionId, title, order, description, isRequired)
		{
			MaxLength = maxLength;
			Placeholder = placeholder;
		}

		/// <inheritdoc/>
		public override ComponentType Type => ComponentType.Text;

		/// <inheritdoc/>
		public string? Placeholder { get; private set; }

		/// <inheritdoc/>
		public int? MaxLength { get; private set; }

		/// <inheritdoc/>
		public void UpdateBasicProperties(int? maxLength, string? placeholder)
		{
			MaxLength = maxLength;
			Placeholder = placeholder;
		}
	}
}
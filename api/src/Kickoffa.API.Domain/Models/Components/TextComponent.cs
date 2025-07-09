using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.Components
{
	/// <summary>
	/// Componente de campo de texto para entrada de dados pelo cliente
	/// </summary>
	public class TextComponent : Component, ITextComponent
	{
		public TextComponent(long sectionId, string title, int order, string? description, bool isRequired, string? placeholder, int? maxLength)
			: base(sectionId, title, order, description, isRequired)
		{
			MaxLength = maxLength;
			Placeholder = placeholder;
		}

		public override ComponentType Type => ComponentType.Text;

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
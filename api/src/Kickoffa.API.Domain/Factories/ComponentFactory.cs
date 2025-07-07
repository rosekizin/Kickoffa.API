using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Models.Components.Base;

namespace Kickoffa.API.Domain.Factories
{
	/// <summary>
	/// Factory para criação de componentes baseado no tipo
	/// </summary>
	public static class ComponentFactory
	{
		public static Component CreateSignatureComponent(long sectionId, string title, int order, string? description, bool isRequired = false)
		{
			return new SignatureComponent(sectionId, title, order, description, isRequired);
		}

		public static Component CreateCheckBoxComponent(long sectionId, string title, int order, string? description, bool isRequired = false)
		{
			return new CheckboxComponent(sectionId, title, order, description, isRequired);
		}

		public static Component CreateTextComponent(long sectionId, string title, int order, string? description, string? placeholder, int? maxLength, bool isRequired = false)
		{
			return new TextComponent(sectionId, title, order, description, isRequired, placeholder, maxLength);
		}

		public static Component CreateUploadComponent(long sectionId, string title, int order, string? description, string? placeholder, int? maxLength, bool isRequired = false)
		{
			return new UploadComponent(sectionId, title, order, description, isRequired, placeholder, maxLength);
		}

		public static Component CreateConfirmationComponent(long sectionId, string title, int order, string? description, string? confirmationText, bool isRequired = false)
		{
			return new ConfirmationComponent(sectionId, title, order, description, isRequired, confirmationText);
		}
	}
}
using Kickoffa.API.Domain.Models.Items;
using Kickoffa.API.Domain.Models.Items.Base;

namespace Kickoffa.API.Domain.Factories
{
	/// <summary>
	/// Factory para criação de itens baseado no tipo
	/// </summary>
	public static class ItemFactory
	{
		public static Item CreateSignatureItem(long sectionId, string title, int order, string? description, bool isRequired = false)
		{
			return new SignatureItem(sectionId, title, order, description, isRequired);
		}

		public static Item CreateCheckBoxItem(long sectionId, string title, int order, string? description, bool isRequired = false)
		{
			return new CheckboxItem(sectionId, title, order, description, isRequired);
		}

		public static Item CreateTextItem(long sectionId, string title, int order, string? description, string? placeholder, int? maxLength, bool isRequired = false)
		{
			return new TextItem(sectionId, title, order, description, isRequired, placeholder, maxLength);
		}

		public static Item CreateUploadItem(long sectionId, string title, int order, string? description, string? placeholder, int? maxLength, bool isRequired = false)
		{
			return new UploadItem(sectionId, title, order, description, isRequired, placeholder, maxLength);
		}

		public static Item CreateConfirmationItem(long sectionId, string title, int order, string? description, string? confirmationText, bool isRequired = false)
		{
			return new ConfirmationItem(sectionId, title, order, description, isRequired, confirmationText);
		}
	}
}
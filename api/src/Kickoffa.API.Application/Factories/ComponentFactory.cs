using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Contracts.Checklist.Components;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components;

namespace Kickoffa.API.Application.Factories
{
	///<inheritdoc/>
	public class ComponentFactory : IComponentFactory
	{
		public IComponent CreateComponent(ComponentRequest componentRequest)
		{
			return componentRequest.Type switch
			{
				ComponentTypeRequest.Checkbox => CreateCheckBoxComponent((CheckboxComponentRequest)componentRequest),
				ComponentTypeRequest.Text => CreateTextComponent((TextComponentRequest)componentRequest),
				ComponentTypeRequest.Upload => CreateUploadComponent((UploadComponentRequest)componentRequest),
				ComponentTypeRequest.Signature => CreateSignatureComponent((SignatureComponentRequest)componentRequest),
				ComponentTypeRequest.Confirmation => CreateConfirmationComponent((ConfirmationComponentRequest)componentRequest),
				_ => throw new ArgumentException($"Tipo de componente inválido: {componentRequest.Type}")
			};
		}

		public static IComponent CreateSignatureComponent(SignatureComponentRequest request)
		{
			return new SignatureComponent(0,
				request.Title,
				request.Order,
				request.Description, request.IsRequired);
		}

		public static IComponent CreateCheckBoxComponent(CheckboxComponentRequest request)
		{
			return new CheckboxComponent(0, request.Title, request.Order, request.Description, request.IsRequired);
		}

		public static IComponent CreateTextComponent(TextComponentRequest request)
		{
			return new TextComponent(
				0,
				request.Title,
				request.Order,
				request.Description,
				request.IsRequired,
				request.Placeholder,
				request.MaxLength);
		}

		public static IComponent CreateUploadComponent(UploadComponentRequest request)
		{
			return new UploadComponent(0,
				request.Title,
				request.Order,
				request.Description,
				request.IsRequired,
				request.Placeholder,
				request.MaxSizeMB);
		}

		public static IComponent CreateConfirmationComponent(ConfirmationComponentRequest request)
		{
			return new ConfirmationComponent(0, 
				request.Title, request.Order, 
				request.Description, request.IsRequired, request.ConfirmationText);
		}
	}
}
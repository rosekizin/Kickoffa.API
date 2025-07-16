using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Repositories;

namespace Kickoffa.API.Application.Factories
{
	///<inheritdoc/>
	public class ComponentFactory : IComponentFactory
	{
		private readonly IFileTypeRepository _fileTypeRepository;
		private readonly IUploadComponentFileTypeSizeFactory _uploadComponentFileTypeSizeFactory;

		public ComponentFactory(
			IFileTypeRepository fileTypeRepository,
			IUploadComponentFileTypeSizeFactory uploadComponentFileTypeSizeFactory)
		{
			_fileTypeRepository = fileTypeRepository;
			_uploadComponentFileTypeSizeFactory = uploadComponentFileTypeSizeFactory;
		}

		public async Task<IComponent> CreateComponent(ComponentRequest componentRequest, CancellationToken cancellationToken)
		{
			return componentRequest.Type switch
			{
				ComponentTypeRequest.Checkbox => CreateCheckBoxComponent((CheckboxComponentRequest)componentRequest),
				ComponentTypeRequest.Text => CreateTextComponent((TextComponentRequest)componentRequest),
				ComponentTypeRequest.Upload => await CreateUploadComponent((UploadComponentRequest)componentRequest, cancellationToken),
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

		public async Task<IComponent> CreateUploadComponent(UploadComponentRequest request, CancellationToken cancellationToken)
		{
			var uploadComponent = new UploadComponent(0,
				request.Title,
				request.Order,
				request.Description,
				request.IsRequired,
				request.Placeholder);

			var allowedFileTypesToAdd = await _fileTypeRepository.GetByIdsAsync(request.AllowedFileTypeIds, cancellationToken);

			foreach (var fileType in allowedFileTypesToAdd)
			{
				uploadComponent.AddAllowedFileType(fileType);
			}

			foreach (var fileTypeSizeConfig in request.FileTypeSizeConfigs!)
			{
				var fileTypeSize = _uploadComponentFileTypeSizeFactory.CreateFileTypeSize(0, fileTypeSizeConfig);
				uploadComponent.AddFileTypeSizeConfig(fileTypeSize);
			}

			return uploadComponent;
		}

		public static IComponent CreateConfirmationComponent(ConfirmationComponentRequest request)
		{
			return new ConfirmationComponent(0,
				request.Title, request.Order,
				request.Description, request.IsRequired, request.ConfirmationText);
		}
	}
}
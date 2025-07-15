using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Contracts.Checklist.Components;
using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.Contracts.FileType;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Models.FreelancerCustomer;

namespace Kickoffa.API.Application.Services.Checklists
{
	public class MapChecklistToResponse : IMapChecklistToResponse
	{
		/// <summary>
		/// Mapeia uma entidade Checklist para ChecklistResponse
		/// </summary>
		public ChecklistResponse MapToResponse(IChecklist checklist)
		{
			return new ChecklistResponse
			{
				Id = checklist.Id,
				OwnerId = checklist.OwnerId,
				CustomerId = checklist.CustomerId,
				Customer = MapCustomerToResponse(checklist),
				Title = checklist.Title,
				Slug = checklist.Slug,
				Description = checklist.Description,
				Deadline = checklist.DueDate,
				AccessToken = checklist.AccessToken,
				IsPublished = checklist.IsPublished,
				Sections = checklist.Sections.OrderBy(s => s.Order).Select(MapSectionToResponse).ToList(),
				CreatedDateUtc = checklist.CreatedDateUtc,
				LastUpdatedDateUtc = checklist.LastUpdatedDateUtc
			};
		}

		/// <summary>
		/// Mapeia as informações do customer do checklist para CustomerResponse
		/// </summary>
		private static CustomerResponse? MapCustomerToResponse(IChecklist checklist)
		{
			return checklist.Customer switch
			{
				NaturalPerson naturalPerson => new CustomerResponse
				{
					Id = naturalPerson.Id,
					FirstName = naturalPerson.FirstName,
					LastName = naturalPerson.LastName,
					Email = naturalPerson.Email,
					PhoneNumber = naturalPerson.PhoneNumber,
					Address = naturalPerson.Address,
					Cpf = naturalPerson.Cpf,
					Type = (CustomerType)naturalPerson.Type,
					CreatedDateUtc = naturalPerson.CreatedDateUtc,
					LastUpdatedDateUtc = naturalPerson.LastUpdatedDateUtc
				},
				LegalPerson legalPerson => new CustomerResponse
				{
					Id = legalPerson.Id,
					Company = legalPerson.Company,
					Email = legalPerson.Email,
					PhoneNumber = legalPerson.PhoneNumber,
					Cnpj = legalPerson.Cnpj,
					Type = (CustomerType)legalPerson.Type,
					CreatedDateUtc = legalPerson.CreatedDateUtc,
					LastUpdatedDateUtc = legalPerson.LastUpdatedDateUtc
				},
				_ => null // Caso não seja um tipo suportado, retorna null
			};
		}

		/// <summary>
		/// Mapeia uma entidade Section para SectionResponse
		/// </summary>
		private static SectionResponse MapSectionToResponse(ISection section)
		{
			return section switch
			{
				BriefingSection briefingSection => new BriefingSectionResponse
				{
					Id = briefingSection.Id,
					ChecklistId = briefingSection.ChecklistId,
					Title = briefingSection.Title,
					Type = "briefing",
					Order = briefingSection.Order,
					ContentJson = briefingSection.ContentJson,
					ContentHtml = briefingSection.ContentHtml,
					ContentLastUpdated = briefingSection.ContentLastUpdated,
					CreatedDateUtc = briefingSection.CreatedDateUtc,
					LastUpdatedDateUtc = briefingSection.LastUpdatedDateUtc
				},
				ChecklistSection checklistSection => new ChecklistSectionResponse
				{
					Id = checklistSection.Id,
					ChecklistId = checklistSection.ChecklistId,
					Title = checklistSection.Title,
					Type = "checklist",
					Order = checklistSection.Order,
					Components = checklistSection.Components.OrderBy(c => c.Order).Select(MapComponentToResponse).ToList(),
					CreatedDateUtc = checklistSection.CreatedDateUtc,
					LastUpdatedDateUtc = checklistSection.LastUpdatedDateUtc
				},
				_ => throw new ArgumentException($"Tipo de seção não suportado: {section.GetType().Name}")
			};
		}

		/// <summary>
		/// Mapeia uma entidade Component para ComponentResponse seguindo padrão do domínio
		/// </summary>
		private static ComponentResponse MapComponentToResponse(IComponent component)
		{
			/*
			return new ComponentResponse
			{
				Id = component.Id,
				SectionId = component.SectionId,
				Title = component.Title,
				Description = component.Description,
				Type = component.Type.ToString().ToLowerInvariant(),
				IsRequired = component.IsRequired,
				Order = component.Order,

				// Propriedades específicas por tipo - seguindo padrão do domínio
				Placeholder = component is ITextComponent textComp ? textComp.Placeholder :
							 component is IUploadComponent uploadComp ? uploadComp.Placeholder : null,
				MaxLength = component is ITextComponent textComponent ? textComponent.MaxLength : null,
				MaxSizeMB = component is IUploadComponent uploadComponent ? uploadComponent.MaxSizeMB : null,
				AllowedFileTypes = component is IUploadComponent upload ?
					upload.AllowedFileTypes?.Select(MapFileTypeToResponse).ToList() : null,
				ComponentFiles = component is IUploadComponent uploadComp2 ?
					uploadComp2.ComponentFiles?.Select(MapUploadComponentFileToResponse).ToList() : null,
				ConfirmationText = component is IConfirmationComponent confirmationComponent ?
					confirmationComponent.ConfirmationText : null,

				Status = component.Status != null ? MapComponentStatusToResponse(component.Status) : null,
				CreatedDateUtc = component.CreatedDateUtc,
				LastUpdatedDateUtc = component.LastUpdatedDateUtc
			};*/

			return component switch
			{
				ICheckboxComponent checkboxComponent => new CheckboxComponentResponse
				{
					Id = checkboxComponent.Id,
					SectionId = checkboxComponent.SectionId,
					Title = checkboxComponent.Title,
					Description = checkboxComponent.Description,
					Type = "checkbox",
					IsRequired = checkboxComponent.IsRequired,
					Order = checkboxComponent.Order,
					Status = MapComponentStatusToResponse(checkboxComponent.Status),
					CreatedDateUtc = checkboxComponent.CreatedDateUtc,
					LastUpdatedDateUtc = checkboxComponent.LastUpdatedDateUtc
				},
				ISignatureComponent signatureComponent => new SignatureComponentResponse
				{
					Id = signatureComponent.Id,
					SectionId = signatureComponent.SectionId,
					Title = signatureComponent.Title,
					Description = signatureComponent.Description,
					Type = "signature",
					IsRequired = signatureComponent.IsRequired,
					Order = signatureComponent.Order,
					Status = MapComponentStatusToResponse(signatureComponent.Status),
					CreatedDateUtc = signatureComponent.CreatedDateUtc,
					LastUpdatedDateUtc = signatureComponent.LastUpdatedDateUtc
				},
				ITextComponent textComponent => new TextComponentResponse
				{
					Id = textComponent.Id,
					SectionId = textComponent.SectionId,
					Title = textComponent.Title,
					Description = textComponent.Description,
					Type = "text",
					IsRequired = textComponent.IsRequired,
					Order = textComponent.Order,
					Placeholder = textComponent.Placeholder,
					MaxLength = textComponent.MaxLength,
					Status = MapComponentStatusToResponse(textComponent.Status),
					CreatedDateUtc = textComponent.CreatedDateUtc,
					LastUpdatedDateUtc = textComponent.LastUpdatedDateUtc
				},
				IUploadComponent uploadComponent => new UploadComponentResponse
				{
					Id = uploadComponent.Id,
					SectionId = uploadComponent.SectionId,
					Title = uploadComponent.Title,
					Description = uploadComponent.Description,
					Type = "upload",
					IsRequired = uploadComponent.IsRequired,
					Order = uploadComponent.Order,
					Placeholder = uploadComponent.Placeholder,
					AllowedFileTypes = uploadComponent.AllowedFileTypes?.Select(MapFileTypeToResponse).ToList(),
					ComponentFiles = uploadComponent.ComponentFiles?.Select(MapUploadComponentFileToResponse).ToList(),
					FileTypeSizeConfigs = uploadComponent.FileTypeSizeConfigs?.Select(MapFileTypeSizeToResponseToResponse).ToList(),
					Status = MapComponentStatusToResponse(uploadComponent.Status),
					CreatedDateUtc = uploadComponent.CreatedDateUtc,
					LastUpdatedDateUtc = uploadComponent.LastUpdatedDateUtc
				},
				IConfirmationComponent confirmationComponent => new ConfirmationComponentResponse
				{
					Id = confirmationComponent.Id,
					SectionId = confirmationComponent.SectionId,
					Title = confirmationComponent.Title,
					Description = confirmationComponent.Description,
					Type = "confirmation",
					IsRequired = confirmationComponent.IsRequired,
					Order = confirmationComponent.Order,
					ConfirmationText = confirmationComponent.ConfirmationText,
					Status = MapComponentStatusToResponse(confirmationComponent.Status),
					CreatedDateUtc = confirmationComponent.CreatedDateUtc,
					LastUpdatedDateUtc = confirmationComponent.LastUpdatedDateUtc
				},
				_ => throw new ArgumentException($"Tipo de componente não suportado: {component.GetType().Name}")
			};
		}

		/// <summary>
		/// Mapeia uma entidade FileType para FileTypeResponse
		/// </summary>
		private static FileTypeResponse MapFileTypeToResponse(IFileType fileType)
		{
			return new FileTypeResponse
			{
				Id = fileType.Id,
				MimeType = fileType.MimeType,
				Extension = fileType.Extension,
				DisplayName = fileType.DisplayName,
				Description = fileType.Description,
				Category = fileType.Category.ToString(),
				RecommendedMaxSizeMB = fileType.RecommendedMaxSizeMB
			};
		}

		/// <summary>
		/// Mapeia uma entidade FileType para FileTypeResponse
		/// </summary>
		private static FileTypeSizeConfigResponse MapFileTypeSizeToResponseToResponse(IUploadComponentFileTypeSize fileTypeSize)
		{
			return new FileTypeSizeConfigResponse
			{
				Id = fileTypeSize.Id,
				FileTypeId = fileTypeSize.FileTypeId,
				MaxSizeMB = fileTypeSize.MaxSizeMB,
				UploadComponentId = fileTypeSize.UploadComponentId,
				CreatedDateUtc = fileTypeSize.CreatedDateUtc,
				LastUpdatedDateUtc = fileTypeSize.LastUpdatedDateUtc
			};
		}

		/// <summary>
		/// Mapeia uma entidade UploadComponentFile para UploadComponentFileResponse
		/// </summary>
		private static UploadComponentFileResponse MapUploadComponentFileToResponse(IUploadComponentFile file)
		{
			return new UploadComponentFileResponse
			{
				Id = file.Id,
				ComponentId = file.UploadComponent.Id, // Usar o ID do componente através do relacionamento
				FileName = file.FileName,
				OriginalName = file.FileName, // Por enquanto, usar o mesmo nome
				MimeType = file.ContentType,
				Size = file.FileSize,
				Url = file.StoragePath, // Por enquanto, usar o storage path como URL
				CreatedDateUtc = file.CreatedDateUtc
			};
		}

		/// <summary>
		/// Mapeia uma entidade ComponentStatus para ComponentStatusResponse
		/// </summary>
		private static ComponentStatusResponse? MapComponentStatusToResponse(IComponentStatus? status)
		{
			if(status is null)
				return default;

			return new ComponentStatusResponse
			{
				Id = status.Id,
				ComponentId = status.ComponentId,
				IsCompleted = status.IsCompleted,
				CompletedAt = status.CompletedAt,
				//TextResponse = status.TextResponse,
				//SignatureData = status.SignatureData,
				//UploadedFiles = status.UploadedFiles?.Select(MapUploadedFileToResponse).ToList(),
				CreatedDateUtc = status.CreatedDateUtc,
				LastUpdatedDateUtc = status.LastUpdatedDateUtc
			};
		}

		/*
		/// <summary>
		/// Mapeia uma entidade UploadComponentFile para UploadedFileResponse
		/// </summary>
		private static UploadedFileResponse MapUploadedFileToResponse(UploadComponentFile file)
		{
			return new UploadedFileResponse
			{
				Id = file.Id,
				ComponentStatusId = file.ComponentStatusId,
				FileName = file.FileName,
				OriginalName = file.OriginalName,
				MimeType = file.MimeType,
				Size = file.Size,
				Url = file.Url,
				CreatedDateUtc = file.CreatedDateUtc
			};
		}
		*/
	}
}
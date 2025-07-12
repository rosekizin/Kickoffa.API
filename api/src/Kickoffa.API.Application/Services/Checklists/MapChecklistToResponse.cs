using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Contracts.FileType;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models;

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
		/// Mapeia uma entidade Section para SectionResponse
		/// </summary>
		private static SectionResponse MapSectionToResponse(ISection section)
		{
			return section switch
			{
				BriefingSection briefingSection => new SectionResponse
				{
					Id = briefingSection.Id,
					ChecklistId = briefingSection.ChecklistId,
					Title = briefingSection.Title,
					Type = "briefing",
					Order = briefingSection.Order,
					ContentJson = briefingSection.ContentJson,
					ContentHtml = briefingSection.ContentHtml,
					ContentLastUpdated = briefingSection.ContentLastUpdated,
					Components = null,
					CreatedDateUtc = briefingSection.CreatedDateUtc,
					LastUpdatedDateUtc = briefingSection.LastUpdatedDateUtc
				},
				ChecklistSection checklistSection => new SectionResponse
				{
					Id = checklistSection.Id,
					ChecklistId = checklistSection.ChecklistId,
					Title = checklistSection.Title,
					Type = "checklist",
					Order = checklistSection.Order,
					ContentJson = null,
					ContentHtml = null,
					ContentLastUpdated = null,
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
			};
		}

		/// <summary>
		/// Mapeia uma entidade FileType para FileTypeResponse
		/// </summary>
		private static FileTypeResponse MapFileTypeToResponse(IFileType fileType)
		{
			return new FileTypeResponse(
				fileType.Id,
				fileType.MimeType,
				fileType.Extension,
				fileType.DisplayName,
				fileType.Description,
				fileType.Category.ToString(),
				fileType.RecommendedMaxSizeMB
			);
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
		private static ComponentStatusResponse MapComponentStatusToResponse(IComponentStatus status)
		{
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
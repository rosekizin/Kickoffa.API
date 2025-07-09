using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Contracts.Checklist;
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
		/// Mapeia uma entidade Component para ComponentResponse
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
				AllowedMimeTypes = GetComponentProperty<string?>(component, "AllowedMimeTypes"),
				MaxSizeMB = GetComponentProperty<decimal?>(component, "MaxSizeMB"),
				Placeholder = GetComponentProperty<string?>(component, "Placeholder"),
				MaxLength = GetComponentProperty<int?>(component, "MaxLength"),
				ConfirmationText = GetComponentProperty<string?>(component, "ConfirmationText"),
				Status = component.Status != null ? MapComponentStatusToResponse(component.Status) : null,
				CreatedDateUtc = component.CreatedDateUtc,
				LastUpdatedDateUtc = component.LastUpdatedDateUtc
			};
		}

		/// <summary>
		/// Obtém uma propriedade específica de um componente usando reflexão
		/// </summary>
		private static T? GetComponentProperty<T>(IComponent component, string propertyName)
		{
			var property = component.GetType().GetProperty(propertyName);
			return property != null ? (T?)property.GetValue(component) : default;
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
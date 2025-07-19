using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Repositories;
using Component = Kickoffa.API.Domain.Models.Components.Base.Component;

namespace Kickoffa.API.Application.Services.Checklists
{
	/// <summary>
	/// Serviço para atualização de checklists
	/// </summary>
	public class UpdateChecklistService : IUpdateChecklistService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IChecklistRepository _checklistRepository;
		private readonly IBriefingMediaRepository _briefingMediaRepository;
		private readonly ISectionRepository _sectionRepository;
		private readonly IComponentRepository _componentRepository;
		private readonly ISectionFactory _sectionFactory;
		private readonly IComponentFactory _componentFactory;
		private readonly IMapChecklistToResponse _mapChecklistToResponse;
		private readonly IFileTypeRepository _fileTypeRepository;
		private readonly IUploadComponentFileTypeSizeRepository _uploadComponentFileTypeSizeRepository;

		public UpdateChecklistService(
			IUnitOfWork unitOfWork,
			IChecklistRepository checklistRepository,
			ISectionRepository sectionRepository,
			IComponentRepository componentRepository,
			ISectionFactory sectionFactory,
			IComponentFactory componentFactory,
			IMapChecklistToResponse mapChecklistToResponse,
			IBriefingMediaRepository briefingMediaRepository,
			IFileTypeRepository fileTypeRepository,
			IUploadComponentFileTypeSizeRepository uploadComponentFileTypeSizeRepository)
		{
			_unitOfWork = unitOfWork;
			_checklistRepository = checklistRepository;
			_sectionRepository = sectionRepository;
			_componentRepository = componentRepository;
			_sectionFactory = sectionFactory;
			_componentFactory = componentFactory;
			_mapChecklistToResponse = mapChecklistToResponse;
			_briefingMediaRepository = briefingMediaRepository;
			_fileTypeRepository = fileTypeRepository;
			_uploadComponentFileTypeSizeRepository = uploadComponentFileTypeSizeRepository;
		}

		/// <inheritdoc />
		public async Task<ChecklistResponse?> UpdateAsync(long id, ChecklistRequest request, CancellationToken cancellationToken)
		{
			// Buscar checklist existente
			var checklist = await _checklistRepository.GetByIdWithCompleteHierarchyAsync(id, cancellationToken);
			if (checklist == null)
				return null;

			// Atualizar propriedades básicas do checklist
			checklist.UpdateTitle(request.Title);
			checklist.UpdateDescription(request.Description);
			checklist.UpdateDueDate(request.Deadline);
			checklist.UpdateCustomer(request.CustomerId);

			// Atualizar seções
			await UpdateSectionsAsync(checklist, request.Sections, cancellationToken);

			// Salvar alterações
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			// Retornar checklist atualizado
			return _mapChecklistToResponse.MapToResponse(checklist);
		}

		/// <summary>
		/// Atualiza as seções do checklist de forma inteligente, preservando dados existentes quando possível
		/// </summary>
		private async Task UpdateSectionsAsync(IChecklist checklist, IEnumerable<SectionRequest> requestSections, CancellationToken cancellationToken)
		{
			// Buscar seções existentes
			//var existingSections = await _sectionRepository.GetByChecklistIdAsync(checklist.Id, cancellationToken);

			var sectionsToAdd = new List<SectionRequest>();
			var requestExistingSections = new List<SectionRequest>();

			// Identificar seções para remover (existem no banco mas não no request)
			foreach (var sectionRequest in requestSections)
			{
				// Identificar seções para adicionar (existem no request mas não no banco)
				if (sectionRequest.Id == 0)
				{
					sectionsToAdd.Add(sectionRequest);
				}
				else
				{
					requestExistingSections.Add(sectionRequest);
				}
			}

			var sectionsToRemove = checklist.Sections.Where(s => !requestExistingSections.Select(x => x.Id).Contains(s.Id));

			// Identificar seções para atualizar (existem em ambos mas podem ter mudanças)
			var sectionsToUpdate = checklist.Sections.Where(x => requestExistingSections.Select(x => x.Id).Contains(x.Id));

			// Remover seções que não existem mais
			foreach (var section in sectionsToRemove)
			{
				RemoveSection(section);
			}

			// Atualizar seções existentes
			foreach (var existingSection in sectionsToUpdate)
			{
				var updatedSectionRequest = requestSections.First(x => x.Id == existingSection.Id);
				await UpdateExistingSectionAsync(existingSection, updatedSectionRequest, cancellationToken);
			}

			// Adicionar novas seções
			foreach (var sectionRequest in sectionsToAdd.OrderBy(s => s.Order))
			{
				await CreateNewSectionAsync(checklist.Id, sectionRequest, cancellationToken);
			}
		}

		/// <summary>
		/// Cria uma nova seção
		/// </summary>
		private async Task CreateNewSectionAsync(long checklistId, SectionRequest sectionRequest, CancellationToken cancellationToken)
		{
			ISection newSection;

			if (sectionRequest.Type == SectionTypeRequest.Briefing)
			{
				newSection = _sectionFactory.CreateBriefingSection(
					checklistId,
					sectionRequest);
			}
			else
			{
				newSection = _sectionFactory.CreateChecklistSection(
					checklistId,
					sectionRequest.Title,
					sectionRequest.Order);

				var checklistSectionRequest = (ChecklistSectionRequest)sectionRequest;

				if (checklistSectionRequest.Components != null)
				{
					foreach (var componentRequest in checklistSectionRequest.Components.OrderBy(c => c.Order))
					{
						var component = await _componentFactory.CreateComponent(componentRequest, cancellationToken);
						((ChecklistSection)newSection).AddComponent(component);
					}
				}
			}

			await _sectionRepository.AddAsync(newSection, cancellationToken);
		}

		/// <summary>
		/// Remove uma seção e todos os seus componentes
		/// </summary>
		private void RemoveSection(ISection section)
		{
			// Remover componentes da seção primeiro
			if (section is ChecklistSection checklistSection)
			{
				foreach (var component in checklistSection.Components)
				{
					_componentRepository.Remove(component);
				}
			}
			else if (section is BriefingSection briefingSection)
			{
				foreach (var media in briefingSection.Media)
				{
					_briefingMediaRepository.Remove(media);
				}
			}

			// Remover a seção
			_sectionRepository.Remove(section);
		}

		/// <summary>
		/// Atualiza uma seção existente
		/// </summary>
		private async Task UpdateExistingSectionAsync(ISection existingSection, SectionRequest updatedSectionRequest, CancellationToken cancellationToken)
		{
			// Atualizar propriedades básicas da seção
			existingSection.UpdateTitle(updatedSectionRequest.Title);
			existingSection.UpdateOrder(updatedSectionRequest.Order);

			// Se for seção de briefing, atualizar conteúdo
			if (existingSection is BriefingSection briefingSection && updatedSectionRequest.Type == SectionTypeRequest.Briefing)
			{
				var updatedBriefingSectionRequest = (BriefingSectionRequest)updatedSectionRequest;
				briefingSection.UpdateContent(updatedBriefingSectionRequest.ContentJson, updatedBriefingSectionRequest.ContentHtml);
			}
			// Se for seção de checklist, atualizar componentes
			else if (existingSection is ChecklistSection checklistSection && updatedSectionRequest.Type == SectionTypeRequest.Checklist)
			{
				var updatedChecklistSectionRequest = (ChecklistSectionRequest)updatedSectionRequest;
				await UpdateSectionComponentsAsync(checklistSection, updatedChecklistSectionRequest.Components ?? [], cancellationToken);
			}
		}

		/// <summary>
		/// Atualiza os componentes de uma seção de checklist de forma inteligente
		/// </summary>
		private async Task UpdateSectionComponentsAsync(ChecklistSection section, IEnumerable<ComponentRequest> requestComponents, CancellationToken cancellationToken)
		{
			// Buscar componentes existentes da seção
			var existingComponents = section.Components;
			//var existingComponents = await _componentRepository.GetBySectionIdAsync(section.Id, cancellationToken);
			var requestComponentsList = requestComponents.ToList();

			var componentsToRemove = existingComponents.Where(x => !requestComponentsList.Any(c => c.Id == x.Id)).ToList();
			var componentsToAdd = requestComponentsList.Where(x => x.Id == 0).ToList();
			var componentsToUpdate = existingComponents.Where(x => requestComponentsList.Any(c => c.Id == x.Id)).ToList();

			foreach (var component in componentsToRemove)
			{
				_componentRepository.Remove(component);
			}

			foreach (var existingComponent in componentsToUpdate)
			{
				var updatedComponentRequest = requestComponentsList.First(x => x.Id == existingComponent.Id);
				await UpdateExistingComponent(existingComponent, updatedComponentRequest, cancellationToken);
			}

			foreach (var componentRequest in componentsToAdd.OrderBy(c => c.Order))
			{
				var newComponent = await _componentFactory.CreateComponent(componentRequest, cancellationToken);
				section.AddComponent(newComponent);
			}
		}

		/// <summary>
		/// Atualiza um componente existente com os dados do request (apenas propriedades básicas)
		/// </summary>
		private async Task UpdateExistingComponent(Component existingComponent, ComponentRequest componentRequest, CancellationToken cancellationToken)
		{
			// Atualiza propriedades básicas comuns entre todos os componentes
			existingComponent.UpdateTitle(componentRequest.Title);
			existingComponent.UpdateDescription(componentRequest.Description);
			existingComponent.UpdateRequired(componentRequest.IsRequired);
			existingComponent.UpdateOrder(componentRequest.Order);

			switch (componentRequest.Type)
			{
				case ComponentTypeRequest.Checkbox:
					break;
				case ComponentTypeRequest.Text:
					UpdateTextComponent((TextComponent)existingComponent, (TextComponentRequest)componentRequest);
					break;
				case ComponentTypeRequest.Upload:
					await UpdateUploadComponent((UploadComponent)existingComponent, (UploadComponentRequest)componentRequest, cancellationToken);
					break;
				case ComponentTypeRequest.Signature:
					break;
				case ComponentTypeRequest.Confirmation:
					UpdateConfirmationComponent((ConfirmationComponent)existingComponent, (ConfirmationComponentRequest)componentRequest);
					break;
				default:
					throw new ArgumentException($"Tipo de componente inválido: {componentRequest.Type}");
			}
		}

		private static void UpdateConfirmationComponent(ConfirmationComponent confirmationComponent, ConfirmationComponentRequest updatedConfirmationComponent)
		{
			confirmationComponent.UpdateBasicProperties(updatedConfirmationComponent.ConfirmationText);
		}

		private static void UpdateTextComponent(TextComponent textComponent, TextComponentRequest updatedTextComponent)
		{
			textComponent.UpdateBasicProperties(updatedTextComponent.MaxLength, updatedTextComponent.Placeholder);
		}

		private async Task UpdateUploadComponent(UploadComponent uploadComponent, UploadComponentRequest updatedUploadComponent, CancellationToken cancellationToken)
		{
			uploadComponent.UpdateBasicProperties(updatedUploadComponent.Placeholder);

			var existingAllowedFileTypes = uploadComponent.AllowedFileTypes;
			var updatingAllowedFileTypes = updatedUploadComponent.AllowedFileTypeIds;

			var allowedFileTypesToRemove = existingAllowedFileTypes.Where(x => !updatingAllowedFileTypes.Contains(x.Id));
			var allowedFileTypesToAddIds = updatingAllowedFileTypes.Where(x => !existingAllowedFileTypes.Select(y => y.Id).Contains(x));

			var allowedFileTypesToAdd = await _fileTypeRepository.GetByIdsAsync(allowedFileTypesToAddIds, cancellationToken);

			foreach (var fileType in allowedFileTypesToRemove)
				uploadComponent.AllowedFileTypes.Remove(fileType);

			foreach (var fileType in allowedFileTypesToAdd)
				uploadComponent.AllowedFileTypes.Add((FileType)fileType);

			// Atualizar configurações de tamanho por tipo de arquivo
			if (updatedUploadComponent.FileTypeSizeConfigs != null && updatedUploadComponent.FileTypeSizeConfigs.Any())
			{
				await UpdateFileTypeSizeConfigs(uploadComponent, updatedUploadComponent.FileTypeSizeConfigs, cancellationToken);
			}
		}

		private async Task UpdateFileTypeSizeConfigs(UploadComponent uploadComponent, IEnumerable<FileTypeSizeConfigRequest> fileTypeSizeConfigs, CancellationToken cancellationToken)
		{
			// Buscar configurações existentes
			var existingConfigs = await _uploadComponentFileTypeSizeRepository.GetByUploadComponentIdAsync(uploadComponent.Id, cancellationToken);
			var existingConfigsDict = existingConfigs.ToDictionary(c => c.FileTypeId, c => c);

			// Processar cada configuração recebida
			foreach (var configRequest in fileTypeSizeConfigs)
			{
				if (existingConfigsDict.TryGetValue(configRequest.FileTypeId, out var existingConfig))
				{
					// Atualizar configuração existente se o valor mudou
					if (existingConfig.MaxSizeMB != configRequest.MaxSizeMB)
					{
						uploadComponent.UpdateFileTypeSizeConfig(configRequest.FileTypeId, configRequest.MaxSizeMB);
					}
				}
				else
				{
					// Criar nova configuração
					uploadComponent.UpdateFileTypeSizeConfig(configRequest.FileTypeId, configRequest.MaxSizeMB);
				}
			}

			// Remover configurações que não estão mais na lista (tipos de arquivo removidos)
			var requestedFileTypeIds = fileTypeSizeConfigs.Select(c => c.FileTypeId).ToHashSet();
			var allowedFileTypeIds = uploadComponent.AllowedFileTypes.Select(ft => ft.Id).ToHashSet();

			foreach (var existingConfig in existingConfigs)
			{
				// Remove configuração se o tipo de arquivo não está mais permitido
				if (!allowedFileTypeIds.Contains(existingConfig.FileTypeId))
				{
					uploadComponent.RemoveFileTypeSizeConfig(existingConfig.FileTypeId);
				}
			}
		}
	}
}
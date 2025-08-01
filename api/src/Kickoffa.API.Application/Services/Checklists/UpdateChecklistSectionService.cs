using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Repositories;

namespace Kickoffa.API.Application.Services.Checklists
{
    public class UpdateChecklistSectionService : IUpdateChecklistSectionService
    {
        private readonly IComponentFactory _componentFactory;
        private readonly IFileTypeRepository _fileTypeRepository;
        private readonly IComponentRepository _componentRepository;
        private readonly IUploadComponentFileTypeSizeRepository _uploadComponentFileTypeSizeRepository;

        public UpdateChecklistSectionService(
            IComponentFactory componentFactory,
            IFileTypeRepository fileTypeRepository,
            IComponentRepository componentRepository,
            IUploadComponentFileTypeSizeRepository uploadComponentFileTypeSizeRepository)
        {
            _componentFactory = componentFactory;
            _fileTypeRepository = fileTypeRepository;
            _componentRepository = componentRepository;
            _uploadComponentFileTypeSizeRepository = uploadComponentFileTypeSizeRepository;
        }

        public Task<IChecklistSection> CreateAsync(long checklistId, ChecklistSectionRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(IChecklistSection section, IEnumerable<ComponentRequest> requestComponents, CancellationToken cancellationToken)
        {
            await UpdateSectionComponentsAsync(section, requestComponents, cancellationToken);
        }

        /// <summary>
        /// Atualiza os componentes de uma seção de checklist de forma inteligente
        /// </summary>
        private async Task UpdateSectionComponentsAsync(IChecklistSection section, IEnumerable<ComponentRequest> requestComponents, CancellationToken cancellationToken)
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
        private async Task UpdateExistingComponent(IComponent existingComponent, ComponentRequest componentRequest, CancellationToken cancellationToken)
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
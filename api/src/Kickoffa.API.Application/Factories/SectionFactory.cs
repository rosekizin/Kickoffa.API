using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Application.Factories
{
    ///<inheritdoc/>
    public class SectionFactory : ISectionFactory
    {
        private readonly IComponentFactory _componentFactory;
        private readonly IBriefingMediaFactory _briefingMediaFactory;
        private readonly IAddBriefingMediaService _addBriefingMediaService;
        private readonly ITipTapContentParserService _tipTapContentParserService;

        public SectionFactory(
            IComponentFactory componentFactory,
            IBriefingMediaFactory briefingMediaFactory,
            IAddBriefingMediaService addBriefingMediaService,
            ITipTapContentParserService tipTapContentParserService)
        {
            _componentFactory = componentFactory;
            _briefingMediaFactory = briefingMediaFactory;
            _addBriefingMediaService = addBriefingMediaService;
            _tipTapContentParserService = tipTapContentParserService;
        }

        ///<inheritdoc/>
        public async Task<ISection> CreateSection(SectionRequest sectionRequest, CancellationToken cancellationToken)
        {
            return sectionRequest.Type switch
            {
                SectionTypeRequest.Briefing => CreateBriefingSection(0, sectionRequest),
                SectionTypeRequest.Checklist => await CreateChecklistSection(sectionRequest, cancellationToken),
                _ => throw new ArgumentException($"Tipo de se��o inv�lido: {sectionRequest.Type}")
            };
        }

        ///<inheritdoc/>
        public IBriefingSection CreateBriefingSection(
            long checklistId,
            SectionRequest sectionRequest)
        {
            var briefingSection = ((BriefingSectionRequest)sectionRequest);
            var section = new BriefingSection(checklistId, briefingSection.Title, briefingSection.Order, briefingSection.ContentJson, briefingSection.ContentHtml);

            // Identificar e processar imagens no contentJson
            if (!string.IsNullOrWhiteSpace(briefingSection.ContentJson))
            {
                var imageUrls = _tipTapContentParserService.ExtractImageUrlsFromContentJson(briefingSection.ContentJson);
                _addBriefingMediaService.CreateAndAddBriefingMediaFromImageUrls(section, imageUrls);
            }

            return section;
        }

        ///<inheritdoc/>
        public IBriefingSection CreateBriefingSection(
            long checklistId,
            string title,
            int order,
            string? contentJson = null,
            string? contentHtml = null)
        {
            return new BriefingSection(checklistId, title, order, contentJson, contentHtml);
        }

        ///<inheritdoc/>
        public IChecklistSection CreateChecklistSection(long checklistId, string title, int order)
        {
            return new ChecklistSection(checklistId, title, order);
        }

        ///<inheritdoc/>
        public bool IsValidSectionType(SectionType type)
        {
            return Enum.IsDefined(type);
        }

        ///<inheritdoc/>
        public SectionType[] GetAvailableSectionTypes()
        {
            return Enum.GetValues<SectionType>();
        }

        /// <summary>
        /// Cria uma seção de checklist com seus componentes
        /// </summary>
        private async Task<IChecklistSection> CreateChecklistSection(SectionRequest sectionRequest, CancellationToken cancellationToken)
        {
            var section = CreateChecklistSection(
                checklistId: 0, // Será definido quando adicionado ao checklist
                title: sectionRequest.Title,
                order: sectionRequest.Order
            );

            // Adicionar componentes se existirem
            if (sectionRequest is ChecklistSectionRequest checklistSection && checklistSection.Components is not null)
            {
                foreach (var componentRequest in checklistSection.Components.OrderBy(c => c.Order))
                {
                    var component = await _componentFactory.CreateComponent(componentRequest, cancellationToken);
                    section.AddComponent(component);
                }
            }

            return section;
        }
    }
}
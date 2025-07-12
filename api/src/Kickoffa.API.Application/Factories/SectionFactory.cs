using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Application.Factories
{
	///<inheritdoc/>
	public class SectionFactory : ISectionFactory
	{
		private readonly IComponentFactory _componentFactory;

		public SectionFactory(IComponentFactory componentFactory)
		{
			_componentFactory = componentFactory;
		}

		///<inheritdoc/>
		public async Task<ISection> CreateSection(SectionRequest sectionRequest, CancellationToken cancellationToken)
		{
			return sectionRequest.Type switch
			{
				SectionTypeRequest.Briefing => CreateBriefingSection(
					checklistId: 0, // Será definido quando adicionado ao checklist
					title: sectionRequest.Title,
					order: sectionRequest.Order,
					contentJson: sectionRequest.ContentJson,
					contentHtml: sectionRequest.ContentHtml
				),
				SectionTypeRequest.Checklist => await CreateChecklistSection(sectionRequest, cancellationToken),
				_ => throw new ArgumentException($"Tipo de seção inválido: {sectionRequest.Type}")
			};
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
		private async Task<IChecklistSection> CreateChecklistSection(SectionRequest sectionRequest,	CancellationToken cancellationToken)
		{
			var section = CreateChecklistSection(
				checklistId: 0, // Será definido quando adicionado ao checklist
				title: sectionRequest.Title,
				order: sectionRequest.Order
			);

			// Adicionar componentes se existirem
			if (sectionRequest.Components is not null)
			{
				foreach (var componentRequest in sectionRequest.Components.OrderBy(c => c.Order))
				{
					var component = await _componentFactory.CreateComponent(componentRequest, cancellationToken);
					section.AddComponent(component);
				}
			}

			return section;
		}
	}
}
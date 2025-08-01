using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.Domain.Interfaces.Models;

namespace Kickoffa.API.Application.Interfaces.Checkilists
{
    public interface IUpdateChecklistSectionService
    {
        Task<IChecklistSection> CreateAsync(long checklistId, ChecklistSectionRequest request, CancellationToken cancellationToken);

        Task UpdateAsync(IChecklistSection section, IEnumerable<ComponentRequest> requestComponents, CancellationToken cancellationToken);
    }
}
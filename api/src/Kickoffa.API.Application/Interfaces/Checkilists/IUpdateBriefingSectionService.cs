using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.Domain.Interfaces.Models;

namespace Kickoffa.API.Application.Interfaces.Checkilists
{
    public interface IUpdateBriefingSectionService
    {
        Task UpdateAsync(IBriefingSection briefingSection, BriefingSectionRequest updatedBriefingSectionRequest, CancellationToken cancellationToken);

        Task<IBriefingSection> CreateAsync(long checklistId, BriefingSectionRequest updatedBriefingSectionRequest, CancellationToken cancellationToken);
    }
} 
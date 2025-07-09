using Kickoffa.API.Contracts.Checklist;

namespace Kickoffa.API.Application.Interfaces.Checkilists
{
	public interface ICreateChecklistService
	{
		Task<ChecklistResponse> CreateAsync(long ownerId, ChecklistRequest request, CancellationToken cancellationToken);
	}
}
using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Domain.Interfaces.Models;

namespace Kickoffa.API.Application.Interfaces.Checkilists
{
	public interface IMapChecklistToResponse
	{
		ChecklistResponse MapToResponse(IChecklist checklist);
	}
}
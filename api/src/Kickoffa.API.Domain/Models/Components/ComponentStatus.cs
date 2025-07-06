using Kickoffa.API.Domain.Models.Base;
using Kickoffa.API.Domain.Models.Components.Base;

namespace Kickoffa.API.Domain.Models.Components
{
	public class ComponentStatus : BaseEntity
	{
		public ComponentStatus(long componentId, bool isCompleted, DateTime? completedAt, string? response)
		{
			ComponentId = componentId;
			IsCompleted = isCompleted;
			CompletedAt = completedAt;
			Response = response;
		}

		public long ComponentId { get; private set; }
		public bool IsCompleted { get; private set; }
		public DateTime? CompletedAt { get; private set; }
		public string? Response { get; private set; } // Resposta textual quando aplicável

		// Relacionamento
		public virtual Component Component { get; set; } = null!;
	}
}
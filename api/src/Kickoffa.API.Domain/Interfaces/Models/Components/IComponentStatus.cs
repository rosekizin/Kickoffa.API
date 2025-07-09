namespace Kickoffa.API.Domain.Interfaces.Models.Components
{
	public interface IComponentStatus : IBaseEntity
	{
		long ComponentId { get; }
		bool IsCompleted { get; }
		DateTime? CompletedAt { get; }
		string? Response { get; } // Resposta textual quando aplicável

		// Relacionamento
		IComponent Component { get; }
	}
}
using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.Domain.Interfaces.Models.Components;

namespace Kickoffa.API.Application.Interfaces.Factories
{
	/// <summary>
	/// Factory para criação de componentes baseado no tipo
	/// </summary>
	public interface IComponentFactory
	{
		Task<IComponent> CreateComponent(ComponentRequest componentRequest, CancellationToken cancellationToken);
	}
}
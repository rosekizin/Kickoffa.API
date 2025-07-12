using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Interfaces.Models
{
	/// <summary>
	/// Seção do tipo Checklist - contém componentes interativos
	/// </summary>
	public interface IChecklistSection : ISection
	{
		/// <summary>
		/// Tipo da seção (sempre Checklist)
		/// </summary>
		new SectionType Type { get; }

		/// <summary>
		/// Componentes da seção de checklist
		/// </summary>
		IEnumerable<IComponent> Components { get; }

		/// <summary>
		/// Adiciona um componente à seção
		/// </summary>
		/// <param name="component">Componente a ser adicionado</param>
		void AddComponent(IComponent component);

		/// <summary>
		/// Remove um componente da seção
		/// </summary>
		/// <param name="component">Componente a ser removido</param>
		void RemoveComponent(IComponent component);

		/// <summary>
		/// Reordena os componentes da seção
		/// </summary>
		void ReorderComponents();

		/// <summary>
		/// Move um componente para uma nova posição
		/// </summary>
		/// <param name="component">Componente a ser movido</param>
		/// <param name="newOrder">Nova posição</param>
		void MoveComponent(IComponent component, int newOrder);

		/// <summary>
		/// Obtém componentes por tipo
		/// </summary>
		/// <param name="componenteType">Tipo do componente</param>
		/// <returns>Lista de componentes do tipo especificado</returns>
		IEnumerable<IComponent> GetComponentsByType(ComponentType componenteType);

		/// <summary>
		/// Verifica se a seção tem componentes
		/// </summary>
		/// <returns>True se tem componentes, false caso contrário</returns>
		bool HasComponents();

		/// <summary>
		/// Calcula o progresso da seção (percentual de componentes completados)
		/// </summary>
		/// <returns>Percentual de progresso (0-100)</returns>
		decimal CalculateProgress();

		/// <summary>
		/// Verifica se todos os componentes obrigatórios foram completados
		/// </summary>
		/// <returns>True se todos os obrigatórios estão completos, false caso contrário</returns>
		bool AreRequiredComponentsCompleted();
	}
}
using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Components.Base;
using Kickoffa.API.Domain.Interfaces.Models.Components;

namespace Kickoffa.API.Domain.Models.Components
{
	/// <summary>
	/// Componente simples de checkbox para marcar como concluído
	/// </summary>
	public class CheckboxComponent : Component, ICheckboxComponent
	{
		public CheckboxComponent(long sectionId, string title, int order, string? description, bool isRequired)
			: base(sectionId, title, order, description, isRequired)
		{
		}

		public override ComponentType Type => ComponentType.Checkbox;
	}
}
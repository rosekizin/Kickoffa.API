using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Items.Base;

namespace Kickoffa.API.Domain.Models.Items
{
	/// <summary>
	/// Item simples de checkbox para marcar como concluído
	/// </summary>
	public class CheckboxItem : Item
	{
		public override ItemType Type => ItemType.Checkbox;
	}
}

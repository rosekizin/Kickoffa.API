using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Items.Base;

namespace Kickoffa.API.Application.Interfaces
{
	public interface IItemFactory
	{
		Item CreateItem(ItemType type);

		Item CreateItem(ItemType type, long sectionId, string title, int order, bool isRequired = false);
	}
}
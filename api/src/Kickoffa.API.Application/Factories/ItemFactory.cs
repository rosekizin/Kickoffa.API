using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Items;
using Kickoffa.API.Domain.Models.Items.Base;

namespace Kickoffa.API.Application.Factories
{
	/// <summary>
	/// Factory para criação de itens baseado no tipo
	/// </summary>
	public sealed class ItemFactory : IItemFactory
	{
		/// <summary>
		/// Cria uma instância do item baseado no tipo especificado
		/// </summary>
		/// <param name="type">Tipo do item a ser criado</param>
		/// <returns>Instância da classe específica do item</returns>
		/// <exception cref="ArgumentException">Quando o tipo não é suportado</exception>
		public Item CreateItem(ItemType type)
		{
			return type switch
			{
				ItemType.Checkbox => new CheckboxItem(),
				ItemType.Text => new TextItem(),
				ItemType.Upload => new UploadItem(),
				ItemType.Signature => new SignatureItem(),
				ItemType.Confirmation => new ConfirmationItem(),
				_ => throw new ArgumentException($"Tipo de item não suportado: {type}", nameof(type))
			};
		}

		/// <summary>
		/// Cria uma instância do item com propriedades básicas preenchidas
		/// </summary>
		/// <param name="type">Tipo do item</param>
		/// <param name="sectionId">ID da seção</param>
		/// <param name="title">Título do item</param>
		/// <param name="order">Ordem do item</param>
		/// <param name="isRequired">Se o item é obrigatório</param>
		/// <returns>Instância configurada do item</returns>
		public Item CreateItem(ItemType type, long sectionId, string title, int order, bool isRequired = false)
		{
			var item = CreateItem(type);
			item.SectionId = sectionId;
			item.Title = title;
			item.Order = order;
			item.IsRequired = isRequired;
			return item;
		}
	}
}
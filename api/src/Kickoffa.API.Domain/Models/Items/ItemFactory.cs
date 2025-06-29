using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.Items
{
	/// <summary>
	/// Factory para criação de itens baseado no tipo
	/// </summary>
	public static class ItemFactory
	{
		/// <summary>
		/// Cria uma instância do item baseado no tipo especificado
		/// </summary>
		/// <param name="type">Tipo do item a ser criado</param>
		/// <returns>Instância da classe específica do item</returns>
		/// <exception cref="ArgumentException">Quando o tipo não é suportado</exception>
		public static Item CreateItem(ItemType type)
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
		public static Item CreateItem(ItemType type, long sectionId, string title, int order, bool isRequired = false)
		{
			var item = CreateItem(type);
			item.SectionId = sectionId;
			item.Title = title;
			item.Order = order;
			item.IsRequired = isRequired;
			return item;
		}

		/// <summary>
		/// Verifica se um tipo de item é válido
		/// </summary>
		/// <param name="type">Tipo a ser verificado</param>
		/// <returns>True se o tipo é válido</returns>
		public static bool IsValidItemType(ItemType type)
		{
			return type is ItemType.Checkbox or ItemType.Text or ItemType.Upload or ItemType.Signature or ItemType.Confirmation;
		}

		/// <summary>
		/// Obtém todos os tipos de item disponíveis
		/// </summary>
		/// <returns>Array com todos os tipos suportados</returns>
		public static ItemType[] GetAvailableTypes()
		{
			return new[] { ItemType.Checkbox, ItemType.Text, ItemType.Upload, ItemType.Signature, ItemType.Confirmation };
		}
	}
}

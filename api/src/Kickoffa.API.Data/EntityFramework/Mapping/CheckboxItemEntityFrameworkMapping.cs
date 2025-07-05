using Kickoffa.API.Domain.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	/// <summary>
	/// Interface para mapeamento da entidade CheckboxItem
	/// </summary>
	public interface ICheckboxItemEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	/// <summary>
	/// Implementação do mapeamento da entidade CheckboxItem
	/// </summary>
	public class CheckboxItemEntityFrameworkMapping : ICheckboxItemEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<CheckboxItem>();

			// Configuração da tabela específica para TPC
			entity.ToTable("CheckboxItems");

			// CheckboxItem não possui propriedades específicas além das herdadas de Item
			// Mas podemos configurar índices específicos para otimização

			// Índices específicos para CheckboxItem
			entity.HasIndex(c => new { c.SectionId, c.Order })
				.HasDatabaseName("IX_CheckboxItems_SectionId_Order");

			entity.HasIndex(c => new { c.SectionId, c.IsRequired })
				.HasDatabaseName("IX_CheckboxItems_SectionId_IsRequired");

			// Índice para buscar apenas checkboxes obrigatórios
			entity.HasIndex(c => c.IsRequired)
				.HasDatabaseName("IX_CheckboxItems_IsRequired")
				.HasFilter("[IsRequired] = 1"); // Índice filtrado para performance
		}
	}
}

using Kickoffa.API.Domain.Models.Components;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	/// <summary>
	/// Interface para mapeamento da entidade CheckboxComponent
	/// </summary>
	public interface ICheckboxComponentEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	/// <summary>
	/// Implementação do mapeamento da entidade CheckboxComponent
	/// </summary>
	public class CheckboxComponentEntityFrameworkMapping : ICheckboxComponentEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<CheckboxComponent>();

			// Configuração da tabela específica para TPC
			entity.ToTable("CheckboxComponents");

			// CheckboxComponent não possui propriedades específicas além das herdadas de Component
			// Mas podemos configurar índices específicos para otimização

			// Índices específicos para CheckboxComponent
			entity.HasIndex(c => new { c.SectionId, c.Order })
				.HasDatabaseName("IX_CheckboxComponents_SectionId_Order");

			entity.HasIndex(c => new { c.SectionId, c.IsRequired })
				.HasDatabaseName("IX_CheckboxComponents_SectionId_IsRequired");

			// Índice para buscar apenas checkboxes obrigatórios
			entity.HasIndex(c => c.IsRequired)
				.HasDatabaseName("IX_CheckboxComponents_IsRequired")
				.HasFilter("\"IsRequired\" = true"); // Índice filtrado para PostgreSQL
		}
	}
}
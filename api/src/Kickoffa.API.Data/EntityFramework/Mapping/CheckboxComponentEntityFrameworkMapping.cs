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

			// Índices básicos (necessários para TPC)

			/* Nao usar o HasDatabaseName aqui, pois o nome do índice é gerado automaticamente pelo EF Core
			 * Inclusive há um bug no TPC do EF Core que faz com que o nome do índice seja duplicado se usar HasDatabaseName
			 * https://github.com/efcore/EFCore.NamingConventions/issues/185#issuecomment-1876016568
			 */

			entity.HasIndex(c => c.SectionId);

			entity.HasIndex(c => new { c.SectionId, c.Order });

			entity.HasIndex(c => new { c.SectionId, c.IsRequired });

			// Índice para buscar apenas checkboxes obrigatórios
			entity.HasIndex(c => c.IsRequired)
				.HasFilter("\"IsRequired\" = true"); // Índice filtrado para PostgreSQL
		}
	}
}
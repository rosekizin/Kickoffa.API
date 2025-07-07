using Kickoffa.API.Domain.Models.Components;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	/// <summary>
	/// Interface para mapeamento da entidade TextComponent
	/// </summary>
	public interface ITextComponentEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	/// <summary>
	/// Implementação do mapeamento da entidade TextComponent
	/// </summary>
	public class TextComponentEntityFrameworkMapping : ITextComponentEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<TextComponent>();

			// Configuração da tabela específica para TPC
			entity.ToTable("TextComponents");

			// Configuração das propriedades específicas
			entity.Property(t => t.Placeholder)
				.HasMaxLength(200);

			entity.Property(t => t.MaxLength);

			// Índices básicos (necessários para TPC)

			/* Nao usar o HasDatabaseName aqui, pois o nome do índice é gerado automaticamente pelo EF Core
			 * Inclusive há um bug no TPC do EF Core que faz com que o nome do índice seja duplicado se usar HasDatabaseName
			 * https://github.com/efcore/EFCore.NamingConventions/issues/185#issuecomment-1876016568
			 */

			entity.HasIndex(t => t.SectionId);

			entity.HasIndex(t => new { t.SectionId, t.Order });

			// Índices específicos para TextComponent
			entity.HasIndex(t => t.Placeholder);

			entity.HasIndex(t => new { t.SectionId, t.Placeholder });

			entity.HasIndex(t => t.MaxLength);
		}
	}
}
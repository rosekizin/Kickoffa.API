using Kickoffa.API.Domain.Models.Components;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	/// <summary>
	/// Interface para mapeamento da entidade ConfirmationComponent
	/// </summary>
	public interface IConfirmationComponentEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	/// <summary>
	/// Implementação do mapeamento da entidade ConfirmationComponent
	/// </summary>
	public class ConfirmationComponentEntityFrameworkMapping : IConfirmationComponentEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<ConfirmationComponent>();

			// Configuração da tabela específica para TPC
			entity.ToTable("ConfirmationComponents");

			// Configuração das propriedades específicas
			entity.Property(c => c.ConfirmationText)
				.IsRequired()
				.HasMaxLength(1000);

			// Índices básicos (necessários para TPC)

			/* Nao usar o HasDatabaseName aqui, pois o nome do índice é gerado automaticamente pelo EF Core
			 * Inclusive há um bug no TPC do EF Core que faz com que o nome do índice seja duplicado se usar HasDatabaseName
			 * https://github.com/efcore/EFCore.NamingConventions/issues/185#issuecomment-1876016568
			 */

			entity.HasIndex(c => c.SectionId);

			entity.HasIndex(c => new { c.SectionId, c.Order });

			// Índices específicos para ConfirmationComponent
			entity.HasIndex(c => c.ConfirmationText);

			// Índice para busca de texto (full-text search se necessário)
			entity.HasIndex(c => new { c.SectionId, c.ConfirmationText });
		}
	}
}
using Kickoffa.API.Domain.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	/// <summary>
	/// Interface para mapeamento da entidade ConfirmationItem
	/// </summary>
	public interface IConfirmationItemEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	/// <summary>
	/// Implementação do mapeamento da entidade ConfirmationItem
	/// </summary>
	public class ConfirmationItemEntityFrameworkMapping : IConfirmationItemEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<ConfirmationItem>();

			// Configuração da tabela específica para TPC
			entity.ToTable("ConfirmationItems");

			// Configuração das propriedades específicas
			entity.Property(c => c.ConfirmationText)
				.IsRequired()
				.HasMaxLength(1000);

			// Índices específicos para ConfirmationItem
			entity.HasIndex(c => c.ConfirmationText)
				.HasDatabaseName("IX_ConfirmationItems_ConfirmationText");

			// Índice para busca de texto (full-text search se necessário)
			entity.HasIndex(c => new { c.SectionId, c.ConfirmationText })
				.HasDatabaseName("IX_ConfirmationItems_SectionId_ConfirmationText");
		}
	}
}

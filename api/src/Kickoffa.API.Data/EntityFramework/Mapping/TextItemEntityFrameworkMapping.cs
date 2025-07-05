using Kickoffa.API.Domain.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	/// <summary>
	/// Interface para mapeamento da entidade TextItem
	/// </summary>
	public interface ITextItemEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	/// <summary>
	/// Implementação do mapeamento da entidade TextItem
	/// </summary>
	public class TextItemEntityFrameworkMapping : ITextItemEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<TextItem>();

			// Configuração da tabela específica para TPC
			entity.ToTable("TextItems");

			// Configuração das propriedades específicas
			entity.Property(t => t.Placeholder)
				.HasMaxLength(200);

			entity.Property(t => t.MaxLength);

			// Índices específicos para TextItem
			entity.HasIndex(t => t.Placeholder)
				.HasDatabaseName("IX_TextItems_Placeholder");

			entity.HasIndex(t => new { t.SectionId, t.Placeholder })
				.HasDatabaseName("IX_TextItems_SectionId_Placeholder");

			entity.HasIndex(t => t.MaxLength)
				.HasDatabaseName("IX_TextItems_MaxLength");
		}
	}
}

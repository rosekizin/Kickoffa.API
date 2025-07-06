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

			// Índices específicos para TextComponent
			entity.HasIndex(t => t.Placeholder)
				.HasDatabaseName("IX_TextComponents_Placeholder");

			entity.HasIndex(t => new { t.SectionId, t.Placeholder })
				.HasDatabaseName("IX_TextComponents_SectionId_Placeholder");

			entity.HasIndex(t => t.MaxLength)
				.HasDatabaseName("IX_TextComponents_MaxLength");
		}
	}
}
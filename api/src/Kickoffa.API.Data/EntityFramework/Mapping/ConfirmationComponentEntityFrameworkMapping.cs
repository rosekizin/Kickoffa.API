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

			// Índices específicos para ConfirmationComponent
			entity.HasIndex(c => c.ConfirmationText)
				.HasDatabaseName("IX_ConfirmationComponents_ConfirmationText");

			// Índice para busca de texto (full-text search se necessário)
			entity.HasIndex(c => new { c.SectionId, c.ConfirmationText })
				.HasDatabaseName("IX_ConfirmationComponents_SectionId_ConfirmationText");
		}
	}
}
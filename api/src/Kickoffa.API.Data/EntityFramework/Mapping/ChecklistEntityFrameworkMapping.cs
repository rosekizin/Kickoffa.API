using Kickoffa.API.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface IChecklistEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	public class ChecklistEntityFrameworkMapping : IChecklistEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<Checklist>();

			// Configuração da tabela
			entity.ToTable("Checklists");

			// Chave primária
			entity.HasKey(c => c.Id);

			// Propriedades
			entity.Property(c => c.Id)
				.ValueGeneratedOnAdd()
				.IsRequired();

			entity.Property(c => c.OwnerId)
				.IsRequired();

			entity.Property(c => c.Title)
				.IsRequired()
				.HasMaxLength(200);

			entity.Property(c => c.Slug)
				.IsRequired()
				.HasMaxLength(250);

			entity.Property(c => c.Description)
				.HasMaxLength(1000);

			entity.Property(c => c.AccessToken)
				.HasMaxLength(100);

			entity.Property(c => c.IsPublished)
				.IsRequired()
				.HasDefaultValue(false);

			entity.Property(c => c.CreatedDateUtc)
				.IsRequired();

			entity.Property(c => c.LastUpdatedDateUtc)
				.IsRequired();

			// Índices
			entity.HasIndex(c => c.Slug)
				.IsUnique()
				.HasDatabaseName("IX_Checklists_Slug");

			entity.HasIndex(c => c.AccessToken)
				.IsUnique()
				.HasDatabaseName("IX_Checklists_AccessToken");

			entity.HasIndex(c => c.OwnerId)
				.HasDatabaseName("IX_Checklists_OwnerId");

			entity.HasIndex(c => c.IsPublished)
				.HasDatabaseName("IX_Checklists_IsPublished");

			// Relacionamentos
			entity.HasMany(c => c.Sections)
				.WithOne(s => s.Checklist)
				.HasForeignKey(s => s.ChecklistId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
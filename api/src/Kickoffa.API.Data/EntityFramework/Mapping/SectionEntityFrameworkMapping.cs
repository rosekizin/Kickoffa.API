using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface ISectionEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	public class SectionEntityFrameworkMapping : ISectionEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<Section>();

			// Configuração da tabela
			entity.ToTable("Sections");

			// Chave primária
			entity.HasKey(s => s.Id);

			// Propriedades
			entity.Property(s => s.Id)
				.ValueGeneratedOnAdd()
				.IsRequired();

			entity.Property(s => s.ChecklistId)
				.IsRequired();

			entity.Property(s => s.Title)
				.IsRequired()
				.HasMaxLength(200);

			entity.Property(s => s.Type)
				.IsRequired()
				.HasConversion<string>();

			entity.Property(s => s.Order)
				.IsRequired();

			entity.Property(s => s.ContentJson)
				.HasColumnType("TEXT");

			entity.Property(s => s.ContentHtml)
				.HasColumnType("TEXT");

			entity.Property(s => s.CreatedDateUtc)
				.IsRequired();

			entity.Property(s => s.LastUpdatedDateUtc)
				.IsRequired();

			// Índices
			entity.HasIndex(s => s.ChecklistId)
				.HasDatabaseName("IX_Sections_ChecklistId");

			entity.HasIndex(s => new { s.ChecklistId, s.Order })
				.HasDatabaseName("IX_Sections_ChecklistId_Order");

			entity.HasIndex(s => s.Type)
				.HasDatabaseName("IX_Sections_Type");

			// Relacionamentos
			entity.HasOne(s => s.Checklist)
				.WithMany(c => c.Sections)
				.HasForeignKey(s => s.ChecklistId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasMany(s => s.Items)
				.WithOne(i => i.Section)
				.HasForeignKey(i => i.SectionId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasMany(s => s.Media)
				.WithOne(m => m.Section)
				.HasForeignKey(m => m.SectionId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
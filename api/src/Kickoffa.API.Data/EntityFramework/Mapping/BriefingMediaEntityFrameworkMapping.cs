using Kickoffa.API.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface IBriefingMediaEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	public class BriefingMediaEntityFrameworkMapping : IBriefingMediaEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<BriefingMedia>();

			// Configuração da tabela
			entity.ToTable("BriefingMedias");

			// Chave primária
			entity.HasKey(bm => bm.Id);

			// Propriedades
			entity.Property(bm => bm.Id)
				.ValueGeneratedOnAdd()
				.IsRequired();

			entity.Property(bm => bm.SectionId)
				.IsRequired();

			entity.Property(bm => bm.FileName)
				.IsRequired()
				.HasMaxLength(255);

			entity.Property(bm => bm.StoragePath)
				.IsRequired()
				.HasMaxLength(500);

			entity.Property(bm => bm.Url)
				.IsRequired()
				.HasMaxLength(1000);

			entity.Property(bm => bm.ContentType)
				.IsRequired()
				.HasMaxLength(100);

			entity.Property(bm => bm.FileSize)
				.IsRequired();

			entity.Property(bm => bm.AltText)
				.HasMaxLength(200);

			entity.Property(bm => bm.UploadedAt)
				.IsRequired();

			entity.Property(bm => bm.CreatedDateUtc)
				.IsRequired();

			entity.Property(bm => bm.LastUpdatedDateUtc)
				.IsRequired();

			// Índices
			entity.HasIndex(bm => bm.SectionId)
				.HasDatabaseName("IX_BriefingMedias_SectionId");

			entity.HasIndex(bm => bm.ContentType)
				.HasDatabaseName("IX_BriefingMedias_ContentType");

			entity.HasIndex(bm => bm.UploadedAt)
				.HasDatabaseName("IX_BriefingMedias_UploadedAt");

			// Relacionamentos
			entity.HasOne(bm => bm.Section)
				.WithMany(s => s.Media)
				.HasForeignKey(bm => bm.SectionId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
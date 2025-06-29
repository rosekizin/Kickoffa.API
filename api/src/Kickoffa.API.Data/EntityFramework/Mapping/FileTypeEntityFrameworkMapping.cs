using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface IFileTypeEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	public class FileTypeEntityFrameworkMapping : IFileTypeEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<FileType>();

			// Configuração da tabela
			entity.ToTable("FileTypes");

			// Chave primária
			entity.HasKey(ft => ft.Id);

			// Propriedades
			entity.Property(ft => ft.Id)
				.ValueGeneratedOnAdd()
				.IsRequired();

			entity.Property(ft => ft.MimeType)
				.IsRequired()
				.HasMaxLength(100);

			entity.Property(ft => ft.Extension)
				.IsRequired()
				.HasMaxLength(20);

			entity.Property(ft => ft.DisplayName)
				.IsRequired()
				.HasMaxLength(100);

			entity.Property(ft => ft.Description)
				.HasMaxLength(500);

			entity.Property(ft => ft.Category)
				.IsRequired()
				.HasConversion<string>();

			entity.Property(ft => ft.IsActive)
				.IsRequired()
				.HasDefaultValue(true);

			entity.Property(ft => ft.DisplayOrder)
				.IsRequired()
				.HasDefaultValue(0);

			entity.Property(ft => ft.CreatedDateUtc)
				.IsRequired();

			entity.Property(ft => ft.LastUpdatedDateUtc)
				.IsRequired();

			// Índices
			entity.HasIndex(ft => ft.MimeType)
				.HasDatabaseName("IX_FileTypes_MimeType");

			entity.HasIndex(ft => ft.Extension)
				.HasDatabaseName("IX_FileTypes_Extension");

			entity.HasIndex(ft => ft.Category)
				.HasDatabaseName("IX_FileTypes_Category");

			entity.HasIndex(ft => ft.IsActive)
				.HasDatabaseName("IX_FileTypes_IsActive");

			entity.HasIndex(ft => new { ft.Category, ft.DisplayOrder })
				.HasDatabaseName("IX_FileTypes_Category_DisplayOrder");

			// Índice único para MIME type + extensão
			entity.HasIndex(ft => new { ft.MimeType, ft.Extension })
				.IsUnique()
				.HasDatabaseName("IX_FileTypes_MimeType_Extension");
		}
	}
}
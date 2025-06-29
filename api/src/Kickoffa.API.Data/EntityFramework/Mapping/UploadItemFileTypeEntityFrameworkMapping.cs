using Kickoffa.API.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface IUploadItemFileTypeEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	public class UploadItemFileTypeEntityFrameworkMapping : IUploadItemFileTypeEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<UploadItemFileType>();

			// Configuração da tabela
			entity.ToTable("UploadItemFileTypes");

			// Chave primária composta
			entity.HasKey(uft => new { uft.UploadItemId, uft.FileTypeId });

			// Propriedades
			entity.Property(uft => uft.UploadItemId)
				.IsRequired();

			entity.Property(uft => uft.FileTypeId)
				.IsRequired();

			entity.Property(uft => uft.CreatedDateUtc)
				.IsRequired();

			entity.Property(uft => uft.LastUpdatedDateUtc)
				.IsRequired();

			// Relacionamentos
			entity.HasOne(uft => uft.UploadItem)
				.WithMany(ui => ui.AllowedFileTypes)
				.HasForeignKey(uft => uft.UploadItemId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(uft => uft.FileType)
				.WithMany()
				.HasForeignKey(uft => uft.FileTypeId)
				.OnDelete(DeleteBehavior.Restrict); // Não deletar FileType se estiver em uso

			// Índices
			entity.HasIndex(uft => uft.UploadItemId)
				.HasDatabaseName("IX_UploadItemFileTypes_UploadItemId");

			entity.HasIndex(uft => uft.FileTypeId)
				.HasDatabaseName("IX_UploadItemFileTypes_FileTypeId");
		}
	}
}
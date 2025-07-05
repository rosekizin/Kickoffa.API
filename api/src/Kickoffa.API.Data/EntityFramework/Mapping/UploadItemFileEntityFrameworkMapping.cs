using Kickoffa.API.Domain.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	/// <summary>
	/// Interface para mapeamento da entidade UploadItemFile
	/// </summary>
	public interface IUploadItemFileEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	/// <summary>
	/// Implementação do mapeamento da entidade UploadItemFile
	/// </summary>
	public class UploadItemFileEntityFrameworkMapping : IUploadItemFileEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<UploadItemFile>();

			// Configuração da tabela
			entity.ToTable("UploadItemFiles");

			// Configuração da chave primária
			entity.HasKey(f => f.Id);

			// Configuração das propriedades
			entity.Property(f => f.Id)
				.ValueGeneratedOnAdd();

			entity.Property(f => f.FileName)
				.IsRequired()
				.HasMaxLength(255);

			entity.Property(f => f.StoragePath)
				.IsRequired()
				.HasMaxLength(500);

			entity.Property(f => f.FileSize)
				.IsRequired();

			entity.Property(f => f.ContentType)
				.IsRequired()
				.HasMaxLength(100);

			entity.Property(f => f.Sha256Hash)
				.IsRequired()
				.HasMaxLength(64);

			// Propriedades herdadas de BaseEntity
			entity.Property(f => f.CreatedDateUtc)
				.IsRequired();

			entity.Property(f => f.LastUpdatedDateUtc)
				.IsRequired();

			// Configuração do relacionamento com UploadItem
			entity.HasOne(f => f.UploadItem)
				.WithMany(u => u.ItemFiles)
				.HasForeignKey("UploadItemId")
				.OnDelete(DeleteBehavior.Cascade);

			// Configuração de índices
			entity.HasIndex(f => f.FileName)
				.HasDatabaseName("IX_UploadItemFiles_FileName");

			entity.HasIndex(f => f.Sha256Hash)
				.HasDatabaseName("IX_UploadItemFiles_Sha256Hash");

			entity.HasIndex("UploadItemId")
				.HasDatabaseName("IX_UploadItemFiles_UploadItemId");

			// Configuração de índice composto para performance
			entity.HasIndex(f => new { f.Id, f.CreatedDateUtc })
				.HasDatabaseName("IX_UploadItemFiles_UploadItemId_CreatedDateUtc");
		}
	}
}
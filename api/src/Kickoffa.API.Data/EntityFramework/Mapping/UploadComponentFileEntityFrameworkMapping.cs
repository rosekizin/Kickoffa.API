using Kickoffa.API.Domain.Models.Components;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	/// <summary>
	/// Interface para mapeamento da entidade UploadComponentFile
	/// </summary>
	public interface IUploadComponentFileEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	/// <summary>
	/// Implementação do mapeamento da entidade UploadComponentFile
	/// </summary>
	public class UploadComponentFileEntityFrameworkMapping : IUploadComponentFileEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<UploadComponentFile>();

			// Configuração da tabela
			entity.ToTable("UploadComponentFiles");

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

			// Configuração do relacionamento com UploadComponent
			entity.HasOne(f => f.UploadComponent)
				.WithMany(u => u.ComponentFiles)
				.HasForeignKey("UploadComponentId")
				.OnDelete(DeleteBehavior.Cascade);

			// Configuração de índices
			entity.HasIndex(f => f.FileName)
				.HasDatabaseName("IX_UploadComponentFiles_FileName");

			entity.HasIndex(f => f.Sha256Hash)
				.HasDatabaseName("IX_UploadComponentFiles_Sha256Hash");

			entity.HasIndex("UploadComponentId")
				.HasDatabaseName("IX_UploadComponentFiles_UploadComponentId");

			// Configuração de índice composto para performance
			entity.HasIndex(f => new { f.Id, f.CreatedDateUtc })
				.HasDatabaseName("IX_UploadComponentFiles_UploadComponentId_CreatedDateUtc");
		}
	}
}
using Kickoffa.API.Domain.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	/// <summary>
	/// Interface para mapeamento da entidade UploadItem
	/// </summary>
	public interface IUploadItemEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	/// <summary>
	/// Implementação do mapeamento da entidade UploadItem
	/// </summary>
	public class UploadItemEntityFrameworkMapping : IUploadItemEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<UploadItem>();

			// Configuração da tabela específica para TPC
			entity.ToTable("UploadItems");

			// Configuração das propriedades específicas
			entity.Property(u => u.MaxSizeMB);

			entity.Property(u => u.Placeholder)
				.HasMaxLength(200);

			// Relacionamento com UploadItemFile (1:N)
			entity.HasMany(u => u.ItemFiles)
				.WithOne(f => f.UploadItem)
				.HasForeignKey("UploadItemId")
				.OnDelete(DeleteBehavior.Cascade);

			// Relacionamento N:N unidirecional (UploadItem → FileType)
			// FileType é autônomo e não conhece UploadItems
			entity.HasMany(u => u.AllowedFileTypes)
				.WithMany() // Sem propriedade de volta no FileType
				.UsingEntity("UploadItemAllowedFileTypes");

			// Índices específicos para UploadItem
			entity.HasIndex(u => u.MaxSizeMB)
				.HasDatabaseName("IX_UploadItems_MaxSizeMB");

			entity.HasIndex(u => u.Placeholder)
				.HasDatabaseName("IX_UploadItems_Placeholder");

			entity.HasIndex(u => new { u.SectionId, u.MaxSizeMB })
				.HasDatabaseName("IX_UploadItems_SectionId_MaxSizeMB");
		}
	}
}
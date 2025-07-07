using Kickoffa.API.Domain.Models.Components;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	/// <summary>
	/// Interface para mapeamento da entidade UploadComponent
	/// </summary>
	public interface IUploadComponentEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	/// <summary>
	/// Implementação do mapeamento da entidade UploadComponent
	/// </summary>
	public class UploadComponentEntityFrameworkMapping : IUploadComponentEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<UploadComponent>();

			// Configuração da tabela específica para TPC
			entity.ToTable("UploadComponents");

			// Configuração das propriedades específicas
			entity.Property(u => u.MaxSizeMB);

			entity.Property(u => u.Placeholder)
				.HasMaxLength(200);

			// Relacionamento com UploadComponentFile (1:N)
			entity.HasMany(u => u.ComponentFiles)
				.WithOne(f => f.UploadComponent)
				.HasForeignKey("UploadComponentId")
				.OnDelete(DeleteBehavior.Cascade);

			// Relacionamento N:N unidirecional (UploadComponent → FileType)
			// FileType é autônomo e não conhece UploadComponents
			entity.HasMany(u => u.AllowedFileTypes)
				.WithMany() // Sem propriedade de volta no FileType
				.UsingEntity("UploadComponentAllowedFileTypes");

			// Índices básicos (necessários para TPC)

			/* Nao usar o HasDatabaseName aqui, pois o nome do índice é gerado automaticamente pelo EF Core
			 * Inclusive há um bug no TPC do EF Core que faz com que o nome do índice seja duplicado se usar HasDatabaseName
			 * https://github.com/efcore/EFCore.NamingConventions/issues/185#issuecomment-1876016568
			 */

			entity.HasIndex(u => u.SectionId);

			entity.HasIndex(u => new { u.SectionId, u.Order });

			// Índices específicos para UploadComponent
			entity.HasIndex(u => u.MaxSizeMB);

			entity.HasIndex(u => u.Placeholder);

			entity.HasIndex(u => new { u.SectionId, u.MaxSizeMB });
		}
	}
}
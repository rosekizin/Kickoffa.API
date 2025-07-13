using Kickoffa.API.Domain.Models.Components;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	/// <summary>
	/// Interface para mapeamento da entidade UploadComponentFileTypeSize
	/// </summary>
	public interface IUploadComponentFileTypeSizeEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	/// <summary>
	/// Implementação do mapeamento da entidade UploadComponentFileTypeSize
	/// </summary>
	public class UploadComponentFileTypeSizeEntityFrameworkMapping : IUploadComponentFileTypeSizeEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<UploadComponentFileTypeSize>();

			// Configuração da tabela
			entity.ToTable("UploadComponentFileTypeSizes");

			// Configuração da chave primária
			entity.HasKey(s => s.Id);

			// Configuração das propriedades
			entity.Property(s => s.Id)
				.ValueGeneratedOnAdd();

			entity.Property(s => s.UploadComponentId)
				.IsRequired();

			entity.Property(s => s.FileTypeId)
				.IsRequired();

			entity.Property(s => s.MaxSizeMB)
				.IsRequired();

			// Propriedades herdadas de BaseEntity
			entity.Property(s => s.CreatedDateUtc)
				.IsRequired();

			entity.Property(s => s.LastUpdatedDateUtc)
				.IsRequired();

			// Configuração do relacionamento com UploadComponent
			entity.HasOne(s => s.UploadComponent)
				.WithMany(u => u.FileTypeSizeConfigs)
				.HasForeignKey(s => s.UploadComponentId)
				.OnDelete(DeleteBehavior.Cascade);

			// Configuração do relacionamento com FileType
			entity.HasOne(s => s.FileType)
				.WithMany() // FileType não conhece UploadComponentFileTypeSize
				.HasForeignKey(s => s.FileTypeId)
				.OnDelete(DeleteBehavior.Restrict); // Não permitir deletar FileType se houver configurações

			// Configuração de índices
			entity.HasIndex(s => s.UploadComponentId);

			entity.HasIndex(s => s.FileTypeId);

			// Índice único para garantir que não haja duplicatas de configuração para o mesmo componente e tipo de arquivo
			entity.HasIndex(s => new { s.UploadComponentId, s.FileTypeId })
				.IsUnique();

			// Índice composto para performance em consultas
			entity.HasIndex(s => new { s.UploadComponentId, s.FileTypeId, s.MaxSizeMB });
		}
	}
}

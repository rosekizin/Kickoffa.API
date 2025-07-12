using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface IChecklistEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder, ICurrentUserService currentUserService);
	}

	public class ChecklistEntityFrameworkMapping : IChecklistEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder, ICurrentUserService currentUserService)
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

			// Query Filter para isolamento por usuário
			if (currentUserService.IsAuthenticated)
			{
				var currentUserId = currentUserService.UserId!.Value;
				entity.HasQueryFilter(c => c.OwnerId == currentUserId);
			}

			// Índices otimizados para multi-tenancy
			entity.HasIndex(c => new { c.OwnerId, c.CreatedDateUtc })
				.HasDatabaseName("IX_Checklists_OwnerId_CreatedDateUtc");

			entity.HasIndex(c => new { c.OwnerId, c.IsPublished })
				.HasDatabaseName("IX_Checklists_OwnerId_IsPublished");

			entity.HasIndex(c => new { c.OwnerId, c.Slug })
				.IsUnique()
				.HasDatabaseName("IX_Checklists_OwnerId_Slug");

			entity.HasIndex(c => c.AccessToken)
				.IsUnique()
				.HasDatabaseName("IX_Checklists_AccessToken");

			// Índice filtrado para checklists publicados (consultas públicas)
			entity.HasIndex(c => new { c.AccessToken, c.OwnerId })
				.HasDatabaseName("IX_Checklists_AccessToken_OwnerId")
				.HasFilter("\"IsPublished\" = true");

			// Relacionamentos
			entity.HasMany(c => c.Sections)
				.WithOne(s => s.Checklist)
				.HasForeignKey(s => s.ChecklistId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
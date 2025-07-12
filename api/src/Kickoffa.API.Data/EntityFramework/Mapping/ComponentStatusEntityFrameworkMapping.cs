using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface IComponentStatusEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder, ICurrentUserService currentUserService);
	}

	public class ComponentStatusEntityFrameworkMapping : IComponentStatusEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder, ICurrentUserService currentUserService)
		{
			var entity = modelBuilder.Entity<ComponentStatus>();

			// Configuração da tabela
			entity.ToTable("ComponentStatuses");

			// Chave primária
			entity.HasKey(ist => ist.Id);

			// Propriedades
			entity.Property(ist => ist.Id)
				.ValueGeneratedOnAdd()
				.IsRequired();

			entity.Property(ist => ist.ComponentId)
				.IsRequired();

			entity.Property(ist => ist.IsCompleted)
				.IsRequired()
				.HasDefaultValue(false);

			entity.Property(ist => ist.Response)
				.HasColumnType("TEXT");

			entity.Property(ist => ist.CreatedDateUtc)
				.IsRequired();

			entity.Property(ist => ist.LastUpdatedDateUtc)
				.IsRequired();

			// Query Filter através do relacionamento Component -> Section -> Checklist
			if (currentUserService.IsAuthenticated)
			{
				var currentUserId = currentUserService.UserId!.Value;
				entity.HasQueryFilter(cs => cs.Component.Section.Checklist.OwnerId == currentUserId);
			}

			// Índices
			entity.HasIndex(ist => ist.ComponentId)
				.IsUnique()
				.HasDatabaseName("IX_ComponentStatuses_ComponentId");

			entity.HasIndex(ist => ist.IsCompleted)
				.HasDatabaseName("IX_ComponentStatuses_IsCompleted");

			// Relacionamentos
			entity.HasOne(ist => ist.Component)
				.WithOne(i => i.Status)
				.HasForeignKey<ComponentStatus>(ist => ist.ComponentId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
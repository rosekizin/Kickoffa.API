using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface IComponentStatusEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	public class ComponentStatusEntityFrameworkMapping : IComponentStatusEntityFrameworkMapping
	{
		private readonly ICurrentUserService _currentUserService;

		public ComponentStatusEntityFrameworkMapping(ICurrentUserService currentUserService)
		{
			_currentUserService = currentUserService;
		}

		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<ComponentStatus>();

			// Query Filter através do relacionamento Component -> Section -> Checklist
			if (_currentUserService.IsAuthenticated)
			{
				var currentUserId = _currentUserService.UserId!.Value;
				entity.HasQueryFilter(cs => cs.Component.Section.Checklist.OwnerId == currentUserId);
			}

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
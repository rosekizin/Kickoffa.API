using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Models.Components.Base;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface IComponentEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	public class ComponentEntityFrameworkMapping : IComponentEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<Component>();

			// Configuração TPC (Table-Per-Concrete-Type)
			entity.UseTpcMappingStrategy();

			// Chave primária
			entity.HasKey(i => i.Id);

			// Propriedades base
			entity.Property(i => i.Id)
				.ValueGeneratedOnAdd()
				.IsRequired();

			entity.Property(i => i.SectionId)
				.IsRequired();

			entity.Property(i => i.Title)
				.IsRequired()
				.HasMaxLength(200);

			entity.Property(i => i.Description)
				.HasMaxLength(1000);

			entity.Property(i => i.Order)
				.IsRequired();

			entity.Property(i => i.IsRequired)
				.IsRequired()
				.HasDefaultValue(false);

			entity.Property(i => i.CreatedDateUtc)
				.IsRequired();

			entity.Property(i => i.LastUpdatedDateUtc)
				.IsRequired();

			// Índices
			entity.HasIndex(i => i.SectionId)
				.HasDatabaseName("IX_Components_SectionId");

			entity.HasIndex(i => new { i.SectionId, i.Order })
				.HasDatabaseName("IX_Components_SectionId_Order");

			// Relacionamentos
			entity.HasOne(i => i.Section)
				.WithMany(s => s.Components)
				.HasForeignKey(i => i.SectionId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(i => i.Status)
				.WithOne(s => s.Component)
				.HasForeignKey<ComponentStatus>(s => s.ComponentId)
				.OnDelete(DeleteBehavior.Cascade);

			// Nota: Configurações específicas de cada tipo são feitas em seus próprios mapeamentos
			// TextComponentEntityFrameworkMapping, UploadComponentEntityFrameworkMapping, etc.
		}
	}
}
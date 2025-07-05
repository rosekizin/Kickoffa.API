using Kickoffa.API.Domain.Models.Items;
using Kickoffa.API.Domain.Models.Items.Base;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface IItemEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	public class ItemEntityFrameworkMapping : IItemEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<Item>();

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
				.HasDatabaseName("IX_Items_SectionId");

			entity.HasIndex(i => new { i.SectionId, i.Order })
				.HasDatabaseName("IX_Items_SectionId_Order");

			// Relacionamentos
			entity.HasOne(i => i.Section)
				.WithMany(s => s.Items)
				.HasForeignKey(i => i.SectionId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(i => i.Status)
				.WithOne(s => s.Item)
				.HasForeignKey<ItemStatus>(s => s.ItemId)
				.OnDelete(DeleteBehavior.Cascade);

			// Nota: Configurações específicas de cada tipo são feitas em seus próprios mapeamentos
			// TextItemEntityFrameworkMapping, UploadItemEntityFrameworkMapping, etc.
		}
	}
}
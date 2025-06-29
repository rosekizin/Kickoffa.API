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

			// Configuração da tabela (Table-Per-Hierarchy)
			entity.ToTable("Items");

			// Chave primária
			entity.HasKey(i => i.Id);

			// Discriminator para hierarquia
			entity.HasDiscriminator<string>("ItemType")
				.HasValue<CheckboxItem>("Checkbox")
				.HasValue<TextItem>("Text")
				.HasValue<UploadItem>("Upload")
				.HasValue<SignatureItem>("Signature")
				.HasValue<ConfirmationItem>("Confirmation");

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

			entity.HasIndex("ItemType")
				.HasDatabaseName("IX_Items_ItemType");

			// Relacionamentos
			entity.HasOne(i => i.Section)
				.WithMany(s => s.Items)
				.HasForeignKey(i => i.SectionId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(i => i.Status)
				.WithOne(s => s.Item)
				.HasForeignKey<ItemStatus>(s => s.ItemId)
				.OnDelete(DeleteBehavior.Cascade);

			// Configurações específicas para TextItem
			entity.OwnsOne<TextItem>("TextItem", textItem =>
			{
				textItem.Property("Placeholder")
					.HasMaxLength(200);

				textItem.Property("MaxLength");
			});

			// Configurações específicas para UploadItem
			entity.OwnsOne<UploadItem>("UploadItem", uploadItem =>
			{
				uploadItem.Property("AllowedMimeTypes")
					.HasMaxLength(500);

				uploadItem.Property("MaxSizeMB");

				uploadItem.Property("Placeholder")
					.HasMaxLength(200);
			});

			// Configurações específicas para ConfirmationItem
			entity.OwnsOne<ConfirmationItem>("ConfirmationItem", confirmationItem =>
			{
				confirmationItem.Property("ConfirmationText")
					.HasMaxLength(1000);
			});
		}
	}
}
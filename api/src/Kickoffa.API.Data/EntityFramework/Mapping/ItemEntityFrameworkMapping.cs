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

			// Configuração específica para TextItem
			var textEntity = modelBuilder.Entity<TextItem>();

			textEntity.Property(t => t.Placeholder)
				.HasMaxLength(200);

			textEntity.Property(t => t.MaxLength);

			// Configuração específica para UploadItem
			var uploadEntity = modelBuilder.Entity<UploadItem>();

			uploadEntity.Property(u => u.MaxSizeMB);

			uploadEntity.Property(u => u.Placeholder)
				.HasMaxLength(200);

			// Relacionamento com UploadItemFile (1:N)
			uploadEntity.HasMany(u => u.ItemFiles)
				.WithOne(f => f.UploadItem)
				.HasForeignKey("UploadItemId")
				.OnDelete(DeleteBehavior.Cascade);

			// Configuração específica para ConfirmationItem
			var confirmationEntity = modelBuilder.Entity<ConfirmationItem>();

			confirmationEntity.Property(c => c.ConfirmationText)
				.HasMaxLength(1000);
		}
	}
}
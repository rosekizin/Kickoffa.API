using Kickoffa.API.Domain.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface IItemStatusEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	public class ItemStatusEntityFrameworkMapping : IItemStatusEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<ItemStatus>();

			// Configuração da tabela
			entity.ToTable("ItemStatuses");

			// Chave primária
			entity.HasKey(ist => ist.Id);

			// Propriedades
			entity.Property(ist => ist.Id)
				.ValueGeneratedOnAdd()
				.IsRequired();

			entity.Property(ist => ist.ItemId)
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
			entity.HasIndex(ist => ist.ItemId)
				.IsUnique()
				.HasDatabaseName("IX_ItemStatuses_ItemId");

			entity.HasIndex(ist => ist.IsCompleted)
				.HasDatabaseName("IX_ItemStatuses_IsCompleted");

			// Relacionamentos
			entity.HasOne(ist => ist.Item)
				.WithOne(i => i.Status)
				.HasForeignKey<ItemStatus>(ist => ist.ItemId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
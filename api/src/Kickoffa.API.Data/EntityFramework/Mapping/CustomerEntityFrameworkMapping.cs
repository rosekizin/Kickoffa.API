using Kickoffa.API.Domain.Models.Customer;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface ICustomerEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder);
	}

	public class CustomerEntityFrameworkMapping : ICustomerEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			// Configuração da tabela
			modelBuilder.Entity<Customer>().ToTable("Customers");

			// Configuração da chave primária
			modelBuilder.Entity<Customer>().HasKey(c => c.Id);
			modelBuilder.Entity<Customer>().Property(c => c.Id)
				.HasColumnName("Id")
				.HasColumnType("uuid")
				.IsRequired()
				.ValueGeneratedNever(); // Guid é gerado pela aplicação

			modelBuilder.Entity<Customer>().Property(c => c.FirstName)
				.HasColumnName("FirstName")
				.HasMaxLength(100)
				.IsRequired();

			modelBuilder.Entity<Customer>().Property(c => c.LastName)
				.HasColumnName("LastName")
				.HasMaxLength(100).IsRequired();

			modelBuilder.Entity<Customer>().Property(c => c.Email)
				.HasColumnName("Email")
				.HasMaxLength(255)
				.IsRequired(false);

			modelBuilder.Entity<Customer>().Property(c => c.Cpf)
				.HasColumnName("Cpf")
				.HasMaxLength(11)
				.IsRequired(false);

			modelBuilder.Entity<Customer>().Property(c => c.Cnpj)
				.HasColumnName("Cnpj")
				.HasMaxLength(14)
				.IsRequired(false);

			modelBuilder.Entity<Customer>().Property(c => c.PhoneNumber)
				.HasColumnName("PhoneNumber")
				.HasMaxLength(20)
				.IsRequired(false);

			modelBuilder.Entity<Customer>().Property(c => c.Address)
				.HasColumnName("address")
				.HasColumnType("text")
				.IsRequired(false);

			// Configuração das propriedades da BaseEntity
			modelBuilder.Entity<Customer>().Property(c => c.CreatedDateUtc)
				.HasColumnName("CreatedDateUtc")
				.IsRequired()
				.HasDefaultValueSql("CURRENT_TIMESTAMP");

			modelBuilder.Entity<Customer>().Property(c => c.LastUpdatedDateUtc)
				.HasColumnName("LastUpdatedDateUtc")
				.IsRequired()
				.HasDefaultValueSql("CURRENT_TIMESTAMP");

			// Indexes
			modelBuilder.Entity<Customer>().HasIndex(e => e.Cpf).IsUnique();

			modelBuilder.Entity<Customer>().HasIndex(e => e.Cnpj).IsUnique();
		}
	}
}
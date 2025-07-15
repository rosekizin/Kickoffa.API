using Kickoffa.API.Domain.Models.FreelancerCustomer;
using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface ICustomerEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder);
	}

	public class CustomerEntityFrameworkMapping : ICustomerEntityFrameworkMapping
	{
		private readonly ICurrentUserService _currentUserService;

		public CustomerEntityFrameworkMapping(ICurrentUserService currentUserService)
		{
			_currentUserService = currentUserService;
		}

		public void Map(ModelBuilder modelBuilder)
		{
			// Configuração TPH (Table-Per-Hierarchy) para Customer
			var entity = modelBuilder.Entity<Customer>();

			// Configuração da tabela
			entity.ToTable("Customers");

			// Configuração TPH - discriminator
			entity.HasDiscriminator<CustomerType>("Type")
				.HasValue<NaturalPerson>(CustomerType.NaturalPerson)
				.HasValue<LegalPerson>(CustomerType.LegalCompany);

			// Query Filter para isolamento por usuário
			if (_currentUserService.IsAuthenticated)
			{
				var currentUserId = _currentUserService.UserId!.Value;
				entity.HasQueryFilter(c => c.OwnerId == currentUserId);
			}

			// Configuração da chave primária
			entity.HasKey(c => c.Id);
			entity.Property(c => c.Id)
				.HasColumnName("Id")
				.HasColumnType("bigint")
				.IsRequired()
				.ValueGeneratedOnAdd(); // ID é auto gerado pelo banco

			// Propriedades da classe base Customer
			entity.Property(c => c.OwnerId)
				.HasColumnName("OwnerId")
				.IsRequired();

			entity.Property(c => c.PhoneNumber)
				.HasColumnName("PhoneNumber")
				.HasMaxLength(20)
				.IsRequired(false);

			entity.Property(c => c.Address)
				.HasColumnName("Address")
				.HasColumnType("text")
				.IsRequired(false);

			entity.Property(c => c.Email)
				.HasColumnName("Email")
				.HasMaxLength(255)
				.IsRequired(false);

			entity.Property(c => c.Type)
				.HasColumnName("Type")
				.IsRequired();

			// Configuração das propriedades da BaseEntity
			entity.Property(c => c.CreatedDateUtc)
				.HasColumnName("CreatedDateUtc")
				.IsRequired()
				.HasDefaultValueSql("CURRENT_TIMESTAMP");

			entity.Property(c => c.LastUpdatedDateUtc)
				.HasColumnName("LastUpdatedDateUtc")
				.IsRequired()
				.HasDefaultValueSql("CURRENT_TIMESTAMP");

			// Configuração específica para NaturalPerson
			modelBuilder.Entity<NaturalPerson>(np =>
			{
				np.Property(n => n.FirstName)
					.HasColumnName("FirstName")
					.HasMaxLength(100)
					.IsRequired(false); // Será required apenas para NaturalPerson via validação

				np.Property(n => n.LastName)
					.HasColumnName("LastName")
					.HasMaxLength(100)
					.IsRequired(false); // Será required apenas para NaturalPerson via validação

				np.Property(n => n.Cpf)
					.HasColumnName("Cpf")
					.HasMaxLength(11)
					.IsRequired(false);
			});

			// Configuração específica para LegalPerson
			modelBuilder.Entity<LegalPerson>(lp =>
			{
				lp.Property(l => l.Company)
					.HasColumnName("Company")
					.HasMaxLength(200)
					.IsRequired(false); // Será required apenas para LegalPerson via validação

				lp.Property(l => l.Cnpj)
					.HasColumnName("Cnpj")
					.HasMaxLength(14)
					.IsRequired(false);
			});

			// Indexes únicos condicionais para TPH
			entity.HasIndex(e => e.Type);

			// Índice único para CPF apenas quando Type = NaturalPerson e CPF não é nulo
			modelBuilder.Entity<NaturalPerson>()
				.HasIndex(np => np.Cpf)
				.IsUnique()
				.HasFilter("\"Cpf\" IS NOT NULL AND \"Type\" = 1"); // 1 = NaturalPerson

			// Índice único para CNPJ apenas quando Type = LegalCompany e CNPJ não é nulo
			modelBuilder.Entity<LegalPerson>()
				.HasIndex(lp => lp.Cnpj)
				.IsUnique()
				.HasFilter("\"Cnpj\" IS NOT NULL AND \"Type\" = 2"); // 2 = LegalCompany
		}
	}
}
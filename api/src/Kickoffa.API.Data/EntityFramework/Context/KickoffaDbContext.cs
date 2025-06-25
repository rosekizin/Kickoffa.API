using Kickoffa.API.Data.EntityFramework.Mapping;
using Kickoffa.API.Domain.Models.FreelancerCustomer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kickoffa.API.Data.EntityFramework.Context
{
	public interface IKickoffaDbContext : IDisposable
	{
		DbSet<Customer> Customers { get; }
		IDbContextTransaction BeginTransaction();
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
		int SaveChanges();
	}

	public class KickoffaDbContext : DbContext, IKickoffaDbContext
	{
		private readonly ICustomerEntityFrameworkMapping _customerEntityFrameworkMapping;

		public KickoffaDbContext(
			DbContextOptions<KickoffaDbContext> options,
			ICustomerEntityFrameworkMapping customerEntityFrameworkMapping) : base(options)
		{
			_customerEntityFrameworkMapping = customerEntityFrameworkMapping;
		}

		public DbSet<Customer> Customers { get; private set; }

		public IDbContextTransaction BeginTransaction()
		{
			return Database.BeginTransaction();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			_customerEntityFrameworkMapping.Map(modelBuilder);
		}
	}
}
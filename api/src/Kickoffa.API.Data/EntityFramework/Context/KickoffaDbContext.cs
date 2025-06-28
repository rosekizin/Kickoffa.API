using Kickoffa.API.Data.EntityFramework.Mapping;
using Kickoffa.API.Domain.Models.FreelancerCustomer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Kickoffa.API.Domain.Models.AppUser;

namespace Kickoffa.API.Data.EntityFramework.Context
{
	public interface IKickoffaDbContext : IDisposable
	{
		DbSet<Customer> Customers { get; }
		DbSet<User> Users { get; }
		IDbContextTransaction BeginTransaction();
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
		int SaveChanges();
	}

	public class KickoffaDbContext : DbContext, IKickoffaDbContext
	{
		private readonly ICustomerEntityFrameworkMapping _customerEntityFrameworkMapping;
		private readonly IUserEntityFrameworkMapping _userEntityFrameworkMapping;

		public KickoffaDbContext(
			DbContextOptions<KickoffaDbContext> options,
			ICustomerEntityFrameworkMapping customerEntityFrameworkMapping,
			IUserEntityFrameworkMapping userEntityFrameworkMapping) : base(options)
		{
			_customerEntityFrameworkMapping = customerEntityFrameworkMapping;
			_userEntityFrameworkMapping = userEntityFrameworkMapping;
		}

		public DbSet<Customer> Customers { get; private set; }
		public DbSet<User> Users { get; private set; }

		public IDbContextTransaction BeginTransaction()
		{
			return Database.BeginTransaction();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			_customerEntityFrameworkMapping.Map(modelBuilder);
			_userEntityFrameworkMapping.Map(modelBuilder);
		}
	}
}
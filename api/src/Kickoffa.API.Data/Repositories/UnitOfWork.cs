using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kickoffa.API.Data.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private IDbContextTransaction? _dbContextTransaction;
		private readonly IKickoffaDbContext _kickoffaDbContext;

		public UnitOfWork(IKickoffaDbContext kickoffaDbContext)
		{
			_kickoffaDbContext = kickoffaDbContext;
		}

		public void BeginTransaction()
		{
			_dbContextTransaction = _kickoffaDbContext.BeginTransaction();
		}

		public void Commit()
		{
			_dbContextTransaction?.Commit();
		}

		public void Dispose()
		{
			_dbContextTransaction?.Dispose();
			_kickoffaDbContext?.Dispose();
			GC.SuppressFinalize(this);
		}

		public async Task SaveChangesAsync(CancellationToken cancellationToken)
		{
			await _kickoffaDbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
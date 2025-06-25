namespace Kickoffa.API.Domain.Repositories
{
	public interface IUnitOfWork : IDisposable
	{
		void BeginTransaction();
		void Commit();

		Task SaveChangesAsync(CancellationToken cancellationToken);
	}
}
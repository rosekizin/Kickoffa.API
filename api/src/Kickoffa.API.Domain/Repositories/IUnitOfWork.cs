namespace Kickoffa.API.Domain.Repositories
{
	public interface IUnitOfWork : IDisposable
	{
		// Repositórios
		ICustomerRepository Customers { get; }
		IFileTypeRepository FileTypes { get; }
		IChecklistRepository Checklists { get; }
		ISectionRepository Sections { get; }
		IItemRepository Items { get; }
		IItemStatusRepository ItemStatuses { get; }
		IBriefingMediaRepository BriefingMedias { get; }

		// Transações
		void BeginTransaction();
		void Commit();

		Task SaveChangesAsync(CancellationToken cancellationToken);
	}
}
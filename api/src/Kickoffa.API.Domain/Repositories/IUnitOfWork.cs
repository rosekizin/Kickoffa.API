namespace Kickoffa.API.Domain.Repositories
{
	public interface IUnitOfWork : IDisposable
	{
		// Repositórios
		ICustomerRepository Customers { get; }
		IFileTypeRepository FileTypes { get; }
		IChecklistRepository Checklists { get; }
		ISectionRepository Sections { get; }
		IComponentRepository Components { get; }
		IComponentStatusRepository ComponentStatuses { get; }
		IBriefingMediaRepository BriefingMedias { get; }

		// Transações
		void BeginTransaction();
		void Commit();

		Task SaveChangesAsync(CancellationToken cancellationToken);
	}
}
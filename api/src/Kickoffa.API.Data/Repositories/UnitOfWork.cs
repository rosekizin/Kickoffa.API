using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kickoffa.API.Data.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private IDbContextTransaction? _dbContextTransaction;
		private readonly IKickoffaDbContext _kickoffaDbContext;

		public UnitOfWork(
			IKickoffaDbContext kickoffaDbContext,
			ICustomerRepository customerRepository,
			IFileTypeRepository fileTypeRepository,
			IChecklistRepository checklistRepository,
			ISectionRepository sectionRepository,
			IComponentRepository componentRepository,
			IComponentStatusRepository componentStatusRepository,
			IBriefingMediaRepository briefingMediaRepository)
		{
			_kickoffaDbContext = kickoffaDbContext;

			// Repositórios injetados diretamente
			Customers = customerRepository;
			FileTypes = fileTypeRepository;
			Checklists = checklistRepository;
			Sections = sectionRepository;
			Components = componentRepository;
			ComponentStatuses = componentStatusRepository;
			BriefingMedias = briefingMediaRepository;
		}

		// Propriedades dos repositórios
		public ICustomerRepository Customers { get; }
		public IFileTypeRepository FileTypes { get; }
		public IChecklistRepository Checklists { get; }
		public ISectionRepository Sections { get; }
		public IComponentRepository Components { get; }
		public IComponentStatusRepository ComponentStatuses { get; }
		public IBriefingMediaRepository BriefingMedias { get; }

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
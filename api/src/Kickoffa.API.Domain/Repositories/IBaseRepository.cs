using System.Linq.Expressions;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models.Base;

namespace Kickoffa.API.Domain.Repositories
{
    public interface IBaseRepository<TInterface, T> where T : BaseEntity, TInterface where TInterface : IBaseEntity
	{
		// Métodos síncronos
		TInterface? GetById(long id);
        IEnumerable<TInterface> GetAll();
        IEnumerable<TInterface> Find(Expression<Func<T, bool>> predicate);
		TInterface? FirstOrDefault(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate);
        int Count(Expression<Func<T, bool>>? predicate = null);

        // Métodos assíncronos
        Task<TInterface?> GetByIdAsync(long id, CancellationToken cancellationToken);
        Task<IEnumerable<TInterface>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<TInterface>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task<TInterface?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken);

        // Métodos de paginação
        Task<(IEnumerable<TInterface> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate,
            Expression<Func<T, object>>? orderBy,
            bool ascending,
            CancellationToken cancellationToken);

        // Métodos de modificação
        void Add(TInterface entity);
        void AddRange(IEnumerable<TInterface> entities);
        void Update(TInterface entity);
        void UpdateRange(IEnumerable<TInterface> entities);
        void Remove(TInterface entity);
        void RemoveRange(IEnumerable<TInterface> entities);
        void RemoveById(long id);

        // Métodos de modificação assíncronos
        Task AddAsync(TInterface entity, CancellationToken cancellationToken);
        Task AddRangeAsync(IEnumerable<TInterface> entities, CancellationToken cancellationToken);

        // Métodos de persistência
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
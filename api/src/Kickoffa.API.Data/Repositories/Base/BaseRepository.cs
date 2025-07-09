using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Kickoffa.API.Domain.Models.Base;
using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Domain.Repositories;
using Kickoffa.API.Domain.Interfaces.Models;

namespace Kickoffa.API.Data.Repositories.Base
{
    public class BaseRepository<TInterface, T>
        : IBaseRepository<TInterface, T> where T : BaseEntity, TInterface where TInterface : IBaseEntity
	{
        protected readonly KickoffaDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(KickoffaDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        // Métodos síncronos
        public virtual TInterface? GetById(long id)
        {
            return _dbSet.Find(id);
        }

        public virtual IEnumerable<TInterface> GetAll()
        {
            return [.. _dbSet];
        }

        public virtual IEnumerable<TInterface> Find(Expression<Func<T, bool>> predicate)
        {
            return [.. _dbSet.Where(predicate)];
        }

        public virtual TInterface? FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public virtual bool Any(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }

        public virtual int Count(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null ? _dbSet.Count() : _dbSet.Count(predicate);
        }

        // Métodos assíncronos
        public virtual async Task<TInterface?> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            return await _dbSet.FindAsync([id], cancellationToken);
        }

        public virtual async Task<IEnumerable<TInterface>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TInterface>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<TInterface?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken)
        {
            return predicate == null
                ? await _dbSet.CountAsync(cancellationToken)
                : await _dbSet.CountAsync(predicate, cancellationToken);
        }

        // Métodos de paginação
        public virtual async Task<(IEnumerable<TInterface> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate,
            Expression<Func<T, object>>? orderBy,
            bool ascending,
            CancellationToken cancellationToken)
        {
            var query = _dbSet.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            if (orderBy != null)
            {
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }
            else
            {
                // Ordenação padrão por data de criação
                query = query.OrderByDescending(x => x.CreatedDateUtc);
            }

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        // Métodos de modificação
        public virtual void Add(TInterface entity)
        {
            _dbSet.Add((T)entity);
        }

        public virtual void AddRange(IEnumerable<TInterface> entities)
        {
            _dbSet.AddRange(entities.Cast<T>());
        }

        public virtual void Update(TInterface entity)
        {
            _dbSet.Update((T)entity);
        }

        public virtual void UpdateRange(IEnumerable<TInterface> entities)
        {
            _dbSet.UpdateRange(entities.Cast<T>());
        }

        public virtual void Remove(TInterface entity)
        {
            _dbSet.Remove((T)entity);
        }

        public virtual void RemoveRange(IEnumerable<TInterface> entities)
        {
            _dbSet.RemoveRange(entities.Cast<T>());
        }

        public virtual void RemoveById(long id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                Remove((T)entity);
            }
        }

        // Métodos de modificação assíncronos
        public virtual async Task AddAsync(TInterface entity, CancellationToken cancellationToken)
        {
            await _dbSet.AddAsync((T)entity, cancellationToken);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TInterface> entities, CancellationToken cancellationToken)
        {
            await _dbSet.AddRangeAsync(entities.Cast<T>(), cancellationToken);
        }

        // Métodos de persistência
        public virtual int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
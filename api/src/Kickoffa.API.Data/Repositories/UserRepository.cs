using Microsoft.EntityFrameworkCore;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Domain.Repositories;
using Kickoffa.API.Domain.Models.AppUser;

namespace Kickoffa.API.Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(KickoffaDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAndPasswordAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var cleanEmail = email.Trim().ToLowerInvariant();

            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email.ToLower() == cleanEmail && u.Password == password, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var cleanEmail = email.Trim().ToLowerInvariant();

            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email.ToLower() == cleanEmail, cancellationToken);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var cleanEmail = email.Trim().ToLowerInvariant();

            return await _dbSet
                .AnyAsync(u => u.Email.ToLower() == cleanEmail, cancellationToken);
        }

        // Override do método GetPagedAsync para incluir ordenação específica de User
        public override async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            System.Linq.Expressions.Expression<Func<User, bool>>? predicate = null,
            System.Linq.Expressions.Expression<Func<User, object>>? orderBy = null,
            bool ascending = true,
            CancellationToken cancellationToken = default)
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
                // Ordenação padrão por nome para User
                query = query.OrderBy(u => u.Name).ThenBy(u => u.Email);
            }

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}

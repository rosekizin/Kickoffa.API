using Microsoft.EntityFrameworkCore;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Domain.Repositories;
using Kickoffa.API.Domain.Models.FreelancerCustomer;

namespace Kickoffa.API.Data.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(KickoffaDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return null;

            // Remove formatação do CPF (pontos e hífen)
            var cleanCpf = cpf.Replace(".", "").Replace("-", "").Trim();

            return await _dbSet
                .FirstOrDefaultAsync(c => c.Cpf == cleanCpf, cancellationToken);
        }

        public async Task<Customer?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return null;

            // Remove formatação do CNPJ (pontos, barras e hífen)
            var cleanCnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Trim();

            return await _dbSet
                .FirstOrDefaultAsync(c => c.Cnpj == cleanCnpj, cancellationToken);
        }

        public async Task<IEnumerable<Customer>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return [];

            var searchTerm = name.Trim().ToLowerInvariant();

            return await _dbSet
                .Where(c => 
                    EF.Functions.Like(c.FirstName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(c.LastName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like((c.FirstName + " " + c.LastName).ToLower(), $"%{searchTerm}%"))
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToListAsync(cancellationToken);
        }

        // Override do método GetPagedAsync para incluir ordenação específica de Customer
        public override async Task<(IEnumerable<Customer> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            System.Linq.Expressions.Expression<Func<Customer, bool>>? predicate = null,
            System.Linq.Expressions.Expression<Func<Customer, object>>? orderBy = null,
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
                // Ordenação padrão por nome para Customer
                query = query.OrderBy(c => c.FirstName).ThenBy(c => c.LastName);
            }

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
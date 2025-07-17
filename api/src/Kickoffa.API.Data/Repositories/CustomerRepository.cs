using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Interfaces.Models.Customer;
using Kickoffa.API.Domain.Models.FreelancerCustomer;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
    public class CustomerRepository : BaseRepository<ICustomer, Customer>, ICustomerRepository
    {
        public CustomerRepository(KickoffaDbContext context) : base(context)
        {
        }

        public async Task<ICustomer?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return null;

            // Remove formatação do CPF (pontos e hífen)
            var cleanCpf = cpf.Replace(".", "").Replace("-", "").Trim();

            return await _dbSet
                .OfType<NaturalPerson>()
                .FirstOrDefaultAsync(c => c.Cpf == cleanCpf, cancellationToken);
        }

        public async Task<ICustomer?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return null;

            // Remove formatação do CNPJ (pontos, barras e hífen)
            var cleanCnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Trim();

            return await _dbSet
                .OfType<LegalPerson>()
                .FirstOrDefaultAsync(c => c.Cnpj == cleanCnpj, cancellationToken);
        }

        public async Task<IEnumerable<ICustomer>> SearchByNameAsync(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
                return [];

            var searchTerm = name.Trim().ToLowerInvariant();

            // Busca unificada usando UNION para melhor performance
            // Busca em todas as colunas de nome: FirstName, LastName, Company
            var results = await _dbSet
                .Where(c =>
                    // Para NaturalPerson: buscar em FirstName, LastName e nome completo
                    (c is NaturalPerson && (
                        EF.Functions.Like(((NaturalPerson)c).FirstName.ToLower(), $"%{searchTerm}%") ||
                        EF.Functions.Like(((NaturalPerson)c).LastName.ToLower(), $"%{searchTerm}%") ||
                        EF.Functions.Like((((NaturalPerson)c).FirstName + " " + ((NaturalPerson)c).LastName).ToLower(), $"%{searchTerm}%")
                    )) ||
                    // Para LegalPerson: buscar em Company
                    (c is LegalPerson &&
                        EF.Functions.Like(((LegalPerson)c).Company.ToLower(), $"%{searchTerm}%")
                    ))
                .OrderBy(c => c.Type) // Ordenar por tipo primeiro
                .ThenBy(c => c.Id) // Depois por ID para consistência
                .ToListAsync(cancellationToken);

            return results;
        }

        /// <summary>
        /// Busca customers por nome usando SQL otimizado para melhor performance
        /// Busca em FirstName, LastName e Company de forma unificada
        /// </summary>
        public async Task<IEnumerable<ICustomer>> SearchByNameOptimizedAsync(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
                return [];

            var searchTerm = $"%{name.Trim().ToLowerInvariant()}%";

            // SQL otimizado que busca em todas as colunas de nome
            var sql = @"
                SELECT * FROM ""Customers""
                WHERE
                    LOWER(COALESCE(""FirstName"", '')) LIKE {0} OR
                    LOWER(COALESCE(""LastName"", '')) LIKE {0} OR
                    LOWER(COALESCE(""FirstName"" || ' ' || ""LastName"", '')) LIKE {0} OR
                    LOWER(COALESCE(""Company"", '')) LIKE {0}
                ORDER BY ""Type"", ""Id""";

            return await _dbSet
                .FromSqlRaw(sql, searchTerm)
                .ToListAsync(cancellationToken);
        }

        // Override do método GetPagedAsync para incluir ordenação específica de Customer
        public override async Task<(IEnumerable<ICustomer> Items, int TotalCount)> GetPagedAsync(
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
                // Ordenação padrão por tipo e nome para Customer
                query = query.OrderBy(c => c.Type).ThenBy(c => c.Id);
            }

            var components = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (components, totalCount);
        }
    }
}
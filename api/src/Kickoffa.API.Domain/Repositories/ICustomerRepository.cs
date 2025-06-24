using Kickoffa.API.Domain.Models.FreelancerCustomer;

namespace Kickoffa.API.Domain.Repositories
{
	public interface ICustomerRepository : IBaseRepository<Customer>
	{
		Task<Customer?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);
		Task<Customer?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
		Task<IEnumerable<Customer>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
	}
}
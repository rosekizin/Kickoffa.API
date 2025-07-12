using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models.FreelancerCustomer;

namespace Kickoffa.API.Domain.Repositories
{
	public interface ICustomerRepository : IBaseRepository<ICustomer, Customer>
	{
		Task<ICustomer?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);
		Task<ICustomer?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
		Task<IEnumerable<ICustomer>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
	}
}
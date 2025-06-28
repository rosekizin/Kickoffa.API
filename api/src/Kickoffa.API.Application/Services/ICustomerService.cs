using Kickoffa.API.Contracts.Customer;

namespace Kickoffa.API.Application.Services;

/// <summary>
/// Interface para serviços relacionados a Customer
/// </summary>
public interface ICustomerService
{
	/// <summary>
	/// Busca todos os customers
	/// </summary>
	/// <param name="cancellationToken">Token de cancelamento</param>
	/// <returns>Lista de customers</returns>
	Task<IEnumerable<CustomerResponse>> GetAllAsync(CancellationToken cancellationToken);

	/// <summary>
	/// Busca um customer por ID
	/// </summary>
	/// <param name="id">ID do customer</param>
	/// <param name="cancellationToken">Token de cancelamento</param>
	/// <returns>Customer encontrado ou null</returns>
	Task<CustomerResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

	/// <summary>
	/// Cria um novo customer
	/// </summary>
	/// <param name="request">Request com os dados do customer</param>
	/// <param name="cancellationToken">Token de cancelamento</param>
	/// <returns>Response com os dados do customer criado</returns>
	Task<CustomerResponse> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken);

	/// <summary>
	/// Atualiza um customer existente
	/// </summary>
	/// <param name="id">ID do customer</param>
	/// <param name="request">Request com os dados atualizados</param>
	/// <param name="cancellationToken">Token de cancelamento</param>
	/// <returns>Response com os dados do customer atualizado ou null se não encontrado</returns>
	Task<CustomerResponse?> UpdateAsync(Guid id, CreateCustomerRequest request, CancellationToken cancellationToken);

	/// <summary>
	/// Remove um customer
	/// </summary>
	/// <param name="id">ID do customer</param>
	/// <param name="cancellationToken">Token de cancelamento</param>
	/// <returns>True se removido com sucesso, false se não encontrado</returns>
	Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
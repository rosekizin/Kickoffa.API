using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.Domain.Interfaces.ProcessResult;

namespace Kickoffa.API.Application.Interfaces
{
	/// <summary>
	/// Interface para serviços relacionados a clientes
	/// </summary>
	public interface ICustomerService
	{
		/// <summary>
		/// Busca todos os clientes
		/// </summary>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de clientes</returns>
		Task<IEnumerable<CustomerResponse>> GetAllAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Busca um cliente por ID
		/// </summary>
		/// <param name="id">ID do cliente</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Cliente encontrado ou null</returns>
		Task<CustomerResponse?> GetByIdAsync(long id, CancellationToken cancellationToken);

		/// <summary>
		/// Cria um novo cliente
		/// </summary>
		/// <param name="request">Dados do cliente</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Cliente criado</returns>
		Task<CustomerResponse> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken);

		/// <summary>
		/// Atualiza um cliente existente
		/// </summary>
		/// <param name="id">ID do cliente</param>
		/// <param name="request">Dados atualizados do cliente</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Result contendo o cliente atualizado ou erro</returns>
		Task<IResult<CustomerResponse>> UpdateAsync(long id, CreateCustomerRequest request, CancellationToken cancellationToken);

		/// <summary>
		/// Remove um cliente
		/// </summary>
		/// <param name="id">ID do cliente</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>True se removido com sucesso</returns>
		Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
	}
}
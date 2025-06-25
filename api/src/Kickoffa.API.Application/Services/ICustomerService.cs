using Kickoffa.API.Contracts.Customer;

namespace Kickoffa.API.Application.Services;

/// <summary>
/// Interface para serviços relacionados a Customer
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Cria um novo customer
    /// </summary>
    /// <param name="request">Request com os dados do customer</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Response com os dados do customer criado</returns>
    Task<CreateCustomerResponse> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
}

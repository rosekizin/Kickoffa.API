using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.Domain.Models.FreelancerCustomer;

namespace Kickoffa.API.Application.Interfaces;

/// <summary>
/// Factory para criação de clientes
/// </summary>
public interface ICreateCustomerFactory
{
    /// <summary>
    /// Cria um novo cliente baseado no request
    /// </summary>
    /// <param name="request">Request com os dados do cliente</param>
    /// <returns>Nova instância de Customer</returns>
    Customer Create(CreateCustomerRequest request);
}

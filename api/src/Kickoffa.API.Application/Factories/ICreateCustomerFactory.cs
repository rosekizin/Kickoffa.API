using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.Domain.Models.FreelancerCustomer;

namespace Kickoffa.API.Application.Factories;

/// <summary>
/// Interface para factory de criação de Customer
/// </summary>
public interface ICreateCustomerFactory
{
    /// <summary>
    /// Cria uma nova instância de Customer baseada no request
    /// </summary>
    /// <param name="request">Request com os dados do customer</param>
    /// <returns>Nova instância de Customer</returns>
    Customer Create(CreateCustomerRequest request);
}

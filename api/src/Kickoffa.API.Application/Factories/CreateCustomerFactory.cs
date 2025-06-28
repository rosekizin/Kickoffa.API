using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Domain.Models.FreelancerCustomer;

namespace Kickoffa.API.Application.Factories;

/// <summary>
/// Factory para criação de Customer
/// </summary>
public sealed class CreateCustomerFactory : ICreateCustomerFactory
{
    /// <summary>
    /// Cria uma nova instância de Customer baseada no request
    /// </summary>
    /// <param name="request">Request com os dados do customer</param>
    /// <returns>Nova instância de Customer</returns>
    /// <exception cref="ArgumentNullException">Quando request é null</exception>
    public Customer Create(CreateCustomerRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Customer(
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            cpf: request.Cpf,
            cnpj: request.Cnpj,
            phoneNumber: request.PhoneNumber,
            address: request.Address
        );
    }
}

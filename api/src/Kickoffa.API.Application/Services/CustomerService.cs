using Kickoffa.API.Application.Factories;
using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.Domain.Repositories;

namespace Kickoffa.API.Application.Services;

/// <summary>
/// Serviço para operações relacionadas a Customer
/// </summary>
public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICreateCustomerFactory _createCustomerFactory;

    /// <summary>
    /// Inicializa uma nova instância do CustomerService
    /// </summary>
    /// <param name="customerRepository">Repositório de customers</param>
    /// <param name="createCustomerFactory">Factory para criação de customers</param>
    public CustomerService(
        ICustomerRepository customerRepository,
        ICreateCustomerFactory createCustomerFactory)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _createCustomerFactory = createCustomerFactory ?? throw new ArgumentNullException(nameof(createCustomerFactory));
    }

	public async Task<CustomerResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

		return new CustomerResponse
		{
			Id = customer.Id,
			FirstName = customer.FirstName,
			LastName = customer.LastName,
			Email = customer.Email,
			Cpf = customer.Cpf,
			Cnpj = customer.Cnpj,
			PhoneNumber = customer.PhoneNumber,
			Address = customer.Address,
			CreatedDateUtc = customer.CreatedDateUtc,
			LastUpdatedDateUtc = customer.LastUpdatedDateUtc
		};
	}
	
    /// <summary>
	/// Cria um novo customer
	/// </summary>
	/// <param name="request">Request com os dados do customer</param>
	/// <param name="cancellationToken">Token de cancelamento</param>
	/// <returns>Response com os dados do customer criado</returns>
	/// <exception cref="ArgumentNullException">Quando request é null</exception>
	public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Criar customer usando factory
        var customer = _createCustomerFactory.Create(request);

        // Adicionar customer ao repositório
        await _customerRepository.AddAsync(customer, cancellationToken);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        // Mapear para response
        return new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Cpf = customer.Cpf,
            Cnpj = customer.Cnpj,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            CreatedDateUtc = customer.CreatedDateUtc,
            LastUpdatedDateUtc = customer.LastUpdatedDateUtc
        };
    }
}

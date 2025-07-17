using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.Domain.Interfaces.Models.Customer;
using Kickoffa.API.Domain.Models.FreelancerCustomer;
using Kickoffa.API.Domain.Repositories;

namespace Kickoffa.API.Application.Services
{
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

		/// <inheritdoc />
		public async Task<IEnumerable<CustomerResponse>> GetAllAsync(CancellationToken cancellationToken)
		{
			var customers = await _customerRepository.GetAllAsync(cancellationToken);

			return customers.Select(MapToResponse);
		}

		/// <inheritdoc />
		public async Task<CustomerResponse?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

			if (customer is null)
				return null;

			return MapToResponse(customer);
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
			return MapToResponse(customer);
		}

		/// <inheritdoc />
		public async Task<CustomerResponse?> UpdateAsync(long id, CreateCustomerRequest request, CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(request);

			// Buscar customer existente
			var existingCustomer = await _customerRepository.GetByIdAsync(id, cancellationToken);
			if (existingCustomer is null)
				return null;

			// Verificar se o tipo do customer não mudou (não permitido)
			if (existingCustomer.Type != (Domain.Models.Enums.CustomerType)request.Type)
			{
				throw new InvalidOperationException("Não é possível alterar o tipo do cliente após a criação");
			}

			// Atualizar baseado no tipo
			switch (existingCustomer)
			{
				case NaturalPerson naturalPerson when request is CreateNaturalPersonRequest naturalPersonRequest:
					naturalPerson.UpdateBasicInfo(
						naturalPersonRequest.FirstName,
						naturalPersonRequest.LastName,
						naturalPersonRequest.PhoneNumber,
						naturalPersonRequest.Address,
						naturalPersonRequest.Email);
					break;

				case LegalPerson legalPerson when request is CreateLegalPersonRequest legalPersonRequest:
					legalPerson.UpdateBasicInfo(
						legalPersonRequest.Company,
						legalPersonRequest.PhoneNumber,
						legalPersonRequest.Address,
						legalPersonRequest.Email);
					break;

				default:
					throw new InvalidOperationException($"Tipo de request incompatível com o tipo do customer existente");
			}

			// Salvar alterações
			await _customerRepository.SaveChangesAsync(cancellationToken);

			// Retornar response atualizado
			return MapToResponse(existingCustomer);
		}

		/// <inheritdoc />
		public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
		{
			var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
			if (customer is null)
				return false;

			_customerRepository.Remove(customer);
			await _customerRepository.SaveChangesAsync(cancellationToken);

			return true;
		}

		/// <summary>
		/// Mapeia um Customer para CustomerResponse baseado no tipo
		/// </summary>
		/// <param name="customer">Customer a ser mapeado</param>
		/// <returns>CustomerResponse correspondente</returns>
		private static CustomerResponse MapToResponse(ICustomer customer)
		{
			return customer switch
			{
				NaturalPerson naturalPerson => new CustomerResponse
				{
					Id = naturalPerson.Id,
					PhoneNumber = naturalPerson.PhoneNumber,
					Address = naturalPerson.Address,
					Email = naturalPerson.Email,
					Type = (CustomerType)naturalPerson.Type,
					CreatedDateUtc = naturalPerson.CreatedDateUtc,
					LastUpdatedDateUtc = naturalPerson.LastUpdatedDateUtc,
					FirstName = naturalPerson.FirstName,
					LastName = naturalPerson.LastName,
					Cpf = naturalPerson.Cpf
				},
				LegalPerson legalPerson => new CustomerResponse
				{
					Id = legalPerson.Id,
					PhoneNumber = legalPerson.PhoneNumber,
					Address = legalPerson.Address,
					Email = legalPerson.Email,
					Type = (CustomerType)legalPerson.Type,
					CreatedDateUtc = legalPerson.CreatedDateUtc,
					LastUpdatedDateUtc = legalPerson.LastUpdatedDateUtc,
					Company = legalPerson.Company,
					Cnpj = legalPerson.Cnpj
				},
				_ => throw new InvalidOperationException($"Unknown customer type: {customer.GetType()}")
			};
		}
	}
}
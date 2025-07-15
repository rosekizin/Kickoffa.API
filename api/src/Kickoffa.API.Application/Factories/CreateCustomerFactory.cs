using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.Domain.Models.FreelancerCustomer;
using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Services;

namespace Kickoffa.API.Application.Factories
{
	/// <summary>
	/// Factory para criação de Customer
	/// </summary>
	public sealed class CreateCustomerFactory : ICreateCustomerFactory
	{
		private readonly ICurrentUserService _currentUserService;

		public CreateCustomerFactory(ICurrentUserService currentUserService)
		{
			_currentUserService = currentUserService;
		}

		/// <summary>
		/// Cria uma nova instância de Customer baseada no request
		/// </summary>
		/// <param name="request">Request com os dados do customer</param>
		/// <returns>Nova instância de Customer</returns>
		/// <exception cref="ArgumentNullException">Quando request é null</exception>
		public Customer Create(CreateCustomerRequest request)
		{
			ArgumentNullException.ThrowIfNull(request);

			var ownerId = _currentUserService.UserId!.Value;

			return request switch
			{
				CreateNaturalPersonRequest naturalPersonRequest => new NaturalPerson(
					ownerId: ownerId,
					firstName: naturalPersonRequest.FirstName,
					lastName: naturalPersonRequest.LastName,
					cpf: naturalPersonRequest.Cpf,
					phoneNumber: naturalPersonRequest.PhoneNumber,
					address: naturalPersonRequest.Address,
					email: naturalPersonRequest.Email
				),
				CreateLegalPersonRequest legalPersonRequest => new LegalPerson(
					ownerId: ownerId,
					company: legalPersonRequest.Company,
					cnpj: legalPersonRequest.Cnpj,
					phoneNumber: legalPersonRequest.PhoneNumber,
					address: legalPersonRequest.Address,
					email: legalPersonRequest.Email
				),
				_ => throw new InvalidOperationException($"Unknown CreateCustomerRequest type: {request.GetType()}")
			};
		}
	}
}
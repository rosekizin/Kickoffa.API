using Kickoffa.API.Domain.Interfaces.Models.Customer;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.FreelancerCustomer
{
	/// <summary>
	/// Representa uma pessoa jurídica (Customer do tipo LegalCompany)
	/// </summary>
	public class LegalPerson : Customer, ILegalPerson
	{
		public string Company { get; private set; } // Razão Social
		public string Cnpj { get; private set; }

		public LegalPerson(long ownerId, string company, string cnpj, string? phoneNumber, string? address, string? email)
			: base(ownerId, phoneNumber, address, email, CustomerType.LegalCompany)
		{
			Company = company;
			Cnpj = cnpj;
		}

		public void UpdateBasicInfo(string company, string? phoneNumber, string? address, string? email)
		{
			Company = company;
			PhoneNumber = phoneNumber;
			Address = address;
			Email = email;
		}
	}
}
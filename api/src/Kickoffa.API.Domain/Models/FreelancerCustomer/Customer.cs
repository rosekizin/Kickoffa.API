using Kickoffa.API.Domain.Models.Base;

namespace Kickoffa.API.Domain.Models.FreelancerCustomer
{
	public class Customer : BaseEntity<Customer>
	{
		public string FirstName { get; private set; }
		public string LastName { get; private set; }
		public string? Email { get; private set; }
		public string? Cpf { get; private set; }
		public string? Cnpj { get; private set; }
		public string? PhoneNumber { get; private set; }
		public string? Address { get; private set; }

		public Customer(string firstName, string lastName, string? email, string? cpf, string? cnpj, string? phoneNumber, string? address)
		{
			FirstName = firstName;
			LastName = lastName;
			Email = email;
			Cpf = cpf;
			Cnpj = cnpj;
			PhoneNumber = phoneNumber;
			Address = address;
		}
	}
}
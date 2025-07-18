using Kickoffa.API.Domain.Interfaces.Models.Customer;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.FreelancerCustomer
{
	/// <summary>
	/// Representa uma pessoa física (Customer do tipo NaturalPerson)
	/// </summary>
	public class NaturalPerson : Customer, INaturalPerson
	{
		public string FirstName { get; private set; }
		public string LastName { get; private set; }
		public string Cpf { get; private set; }

		public NaturalPerson(long ownerId, string firstName, string lastName, string cpf, string? phoneNumber, string? address, string? email)
			: base(ownerId, phoneNumber, address, email, CustomerType.NaturalPerson)
		{
			FirstName = firstName;
			LastName = lastName;
			Cpf = cpf;
		}

		public void UpdateBasicInfo(string firstName, string lastName, string? phoneNumber, string? address, string? email)
		{
			FirstName = firstName;
			LastName = lastName;
			PhoneNumber = phoneNumber;
			Address = address;
			Email = email;
		}
	}
}
namespace Kickoffa.API.Domain.Interfaces.Models.Customer
{
	public interface INaturalPerson : ICustomer
	{
		string FirstName { get; }
		string LastName { get; }
		string? Cpf { get; }

		void UpdateBasicInfo(string firstName, string lastName, string? phoneNumber, string? address, string? email);
	}
}
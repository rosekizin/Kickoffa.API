namespace Kickoffa.API.Domain.Interfaces.Models.Customer
{
	public interface ILegalPerson : ICustomer
	{
		string Company { get; }
		string Cnpj { get; }

		void UpdateBasicInfo(string company, string? phoneNumber, string? address, string? email);
	}
}
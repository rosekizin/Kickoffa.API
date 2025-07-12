namespace Kickoffa.API.Domain.Interfaces.Models
{
	public interface ICustomer : IBaseEntity
	{
		string FirstName { get;}
		string LastName { get; }
		string? Email { get; }
		string? Cpf { get; }
		string? Cnpj { get; }
		string? PhoneNumber { get; }
		string? Address { get; }
	}
}
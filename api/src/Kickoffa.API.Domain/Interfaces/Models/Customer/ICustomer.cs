using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Interfaces.Models.Customer
{
	public interface ICustomer : IBaseEntity
	{
		long OwnerId { get; }
		string? PhoneNumber { get; }
		string? Address { get; }
		string? Email { get; }
		CustomerType Type { get; }
	}
}
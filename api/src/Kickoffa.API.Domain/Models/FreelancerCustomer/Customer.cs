using Kickoffa.API.Domain.Interfaces.Models.Customer;
using Kickoffa.API.Domain.Models.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.FreelancerCustomer
{
	/// <summary>
	/// Classe base abstrata para Customer usando Table-Per-Hierarchy (TPH)
	/// </summary>
	public abstract class Customer : BaseEntity, ICustomer
	{
		public long OwnerId { get; private set; } // ID do freelancer que criou o customer
		public string? PhoneNumber { get; protected set; }
		public string? Address { get; protected set; }
		public string? Email { get; protected set; }
		public CustomerType Type { get; private set; }

		protected Customer(long ownerId, string? phoneNumber, string? address, string? email, CustomerType type)
		{
			OwnerId = ownerId;
			PhoneNumber = phoneNumber;
			Address = address;
			Email = email;
			Type = type;
		}
	}
}
namespace Kickoffa.API.Domain.Interfaces.Models
{
	public interface IBaseEntity
	{
		long Id { get; }
		DateTime CreatedDateUtc { get; }
		DateTime LastUpdatedDateUtc { get; }

		void UpdateLastUpdatedDate();
	}
}
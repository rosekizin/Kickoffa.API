namespace Kickoffa.API.Domain.Models.Base
{
	public class BaseEntity<T> where T : class
	{
		public Guid Id { get; private set; }
		public DateTime CreatedDateUtc { get; private set; }
		public DateTime LastUpdatedDateUtc { get; private set; }

		public BaseEntity()
		{
			Id = Guid.NewGuid();
			CreatedDateUtc = DateTime.UtcNow;
			LastUpdatedDateUtc = CreatedDateUtc;
		}
	}
}
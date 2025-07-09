using Kickoffa.API.Domain.Interfaces.Models;

namespace Kickoffa.API.Domain.Models.Base
{
	public class BaseEntity : IBaseEntity
	{
		public BaseEntity()
		{
			// Id será gerado pelo banco de dados (auto increment)
			CreatedDateUtc = DateTime.UtcNow;
			LastUpdatedDateUtc = CreatedDateUtc;
		}

		public long Id { get; private set; }
		public DateTime CreatedDateUtc { get; private set; }
		public DateTime LastUpdatedDateUtc { get; private set; }

		public void UpdateLastUpdatedDate() 
		{
			LastUpdatedDateUtc = DateTime.UtcNow;
		}

		// Método interno para definir o ID quando carregado do banco
		internal void SetId(long id)
		{
			Id = id;
		}
	}
}
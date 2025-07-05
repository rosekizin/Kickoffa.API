namespace Kickoffa.API.Domain.Models.Base
{
	public class BaseEntity
	{
		public long Id { get; private set; }
		public DateTime CreatedDateUtc { get; private set; }
		public DateTime LastUpdatedDateUtc { get; private set; }

		public BaseEntity()
		{
			// Id será gerado pelo banco de dados (auto increment)
			CreatedDateUtc = DateTime.UtcNow;
			LastUpdatedDateUtc = CreatedDateUtc;
		}

		protected void UpdateLastUpdatedDate() 
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
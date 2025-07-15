using Kickoffa.API.Domain.Models.Components.Base;
using Kickoffa.API.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface IComponentEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	public class ComponentEntityFrameworkMapping : IComponentEntityFrameworkMapping
	{
		private readonly ICurrentUserService _currentUserService;

		public ComponentEntityFrameworkMapping(ICurrentUserService currentUserService)
		{
			_currentUserService = currentUserService;
		}

		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<Component>();

			// Query Filter através do relacionamento Section -> Checklist
			if (_currentUserService.IsAuthenticated)
			{
				var currentUserId = _currentUserService.UserId!.Value;
				entity.HasQueryFilter(c => c.Section.Checklist.OwnerId == currentUserId);
			}

			// Configuração TPC (Table-Per-Concrete-Type)
			entity.UseTpcMappingStrategy();


			// Chave primária
			entity.HasKey(i => i.Id);

			// Propriedades base
			entity.Property(i => i.Id)
				.ValueGeneratedOnAdd()
				.IsRequired();

			entity.Property(i => i.SectionId)
				.IsRequired();

			entity.Property(i => i.Title)
				.IsRequired()
				.HasMaxLength(200);

			entity.Property(i => i.Description)
				.HasMaxLength(1000);

			entity.Property(i => i.Order)
				.IsRequired();

			entity.Property(i => i.IsRequired)
				.IsRequired()
				.HasDefaultValue(false);

			entity.Property(i => i.CreatedDateUtc)
				.IsRequired();

			entity.Property(i => i.LastUpdatedDateUtc)
				.IsRequired();

			// Ignorar propriedade Type abstrata (pode causar conflitos TPC)
			entity.Ignore(i => i.Type);

			// Nota: Com TPC, relacionamentos são inferidos automaticamente através das foreign keys
			// Não configuramos relacionamentos explicitamente na classe base para evitar
			// conflitos de índices automáticos.
			//
			// Os relacionamentos funcionarão normalmente através das propriedades de navegação
			// definidas nas entidades do domínio.
			//
			// Índices específicos são definidos nos mapeamentos de cada tipo concreto.
		}
	}
}
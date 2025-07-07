using Kickoffa.API.Domain.Models.Components;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	/// <summary>
	/// Interface para mapeamento da entidade SignatureComponent
	/// </summary>
	public interface ISignatureComponentEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	/// <summary>
	/// Implementação do mapeamento da entidade SignatureComponent
	/// </summary>
	public class SignatureComponentEntityFrameworkMapping : ISignatureComponentEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<SignatureComponent>();

			// Configuração da tabela específica para TPC
			entity.ToTable("SignatureComponents");

			// SignatureComponent não possui propriedades específicas além das herdadas de Component
			// Mas podemos configurar índices específicos para otimização

			// Índices básicos (necessários para TPC)

			/* Nao usar o HasDatabaseName aqui, pois o nome do índice é gerado automaticamente pelo EF Core
			 * Inclusive há um bug no TPC do EF Core que faz com que o nome do índice seja duplicado se usar HasDatabaseName
			 * https://github.com/efcore/EFCore.NamingConventions/issues/185#issuecomment-1876016568
			 */

			entity.HasIndex(s => s.SectionId);

			entity.HasIndex(s => new { s.SectionId, s.Order });

			// Índices específicos para SignatureComponent
			entity.HasIndex(s => new { s.SectionId, s.IsRequired });

			// Índice para buscar apenas assinaturas obrigatórias
			entity.HasIndex(s => s.IsRequired)
				.HasFilter("\"IsRequired\" = true"); // Índice filtrado para PostgreSQL

			// Índice para buscar assinaturas por checklist (através de joins)
			entity.HasIndex(s => new { s.SectionId, s.IsRequired, s.Order });
		}
	}
}
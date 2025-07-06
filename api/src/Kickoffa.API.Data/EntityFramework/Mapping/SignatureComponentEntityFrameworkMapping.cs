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

			// Índices específicos para SignatureComponent
			entity.HasIndex(s => new { s.SectionId, s.Order })
				.HasDatabaseName("IX_SignatureComponents_SectionId_Order");

			entity.HasIndex(s => new { s.SectionId, s.IsRequired })
				.HasDatabaseName("IX_SignatureComponents_SectionId_IsRequired");

			// Índice para buscar apenas assinaturas obrigatórias
			entity.HasIndex(s => s.IsRequired)
				.HasDatabaseName("IX_SignatureComponents_IsRequired")
				.HasFilter("\"IsRequired\" = 1"); // Índice filtrado para performance

			// Índice para buscar assinaturas por checklist (através de joins)
			entity.HasIndex(s => new { s.SectionId, s.IsRequired, s.Order })
				.HasDatabaseName("IX_SignatureComponents_SectionId_IsRequired_Order");
		}
	}
}
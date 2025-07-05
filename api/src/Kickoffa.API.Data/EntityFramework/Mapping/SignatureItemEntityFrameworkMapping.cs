using Kickoffa.API.Domain.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	/// <summary>
	/// Interface para mapeamento da entidade SignatureItem
	/// </summary>
	public interface ISignatureItemEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder);
	}

	/// <summary>
	/// Implementação do mapeamento da entidade SignatureItem
	/// </summary>
	public class SignatureItemEntityFrameworkMapping : ISignatureItemEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<SignatureItem>();

			// Configuração da tabela específica para TPC
			entity.ToTable("SignatureItems");

			// SignatureItem não possui propriedades específicas além das herdadas de Item
			// Mas podemos configurar índices específicos para otimização

			// Índices específicos para SignatureItem
			entity.HasIndex(s => new { s.SectionId, s.Order })
				.HasDatabaseName("IX_SignatureItems_SectionId_Order");

			entity.HasIndex(s => new { s.SectionId, s.IsRequired })
				.HasDatabaseName("IX_SignatureItems_SectionId_IsRequired");

			// Índice para buscar apenas assinaturas obrigatórias
			entity.HasIndex(s => s.IsRequired)
				.HasDatabaseName("IX_SignatureItems_IsRequired")
				.HasFilter("[IsRequired] = 1"); // Índice filtrado para performance

			// Índice para buscar assinaturas por checklist (através de joins)
			entity.HasIndex(s => new { s.SectionId, s.IsRequired, s.Order })
				.HasDatabaseName("IX_SignatureItems_SectionId_IsRequired_Order");
		}
	}
}

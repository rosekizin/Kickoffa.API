using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
	public interface ISectionEntityFrameworkMapping
	{
		void Map(ModelBuilder modelBuilder, ICurrentUserService currentUserService);
	}

	public class SectionEntityFrameworkMapping : ISectionEntityFrameworkMapping
	{
		public void Map(ModelBuilder modelBuilder, ICurrentUserService currentUserService)
		{
			// Configuração da hierarquia usando Table-Per-Hierarchy (TPH)
			var entity = modelBuilder.Entity<Section>();

			// Configuração da tabela
			entity.ToTable("Sections");

			// Configuração do discriminador para TPH
			entity.HasDiscriminator<string>("SectionType")
				.HasValue<BriefingSection>("Briefing")
				.HasValue<ChecklistSection>("Checklist");

			// Chave primária
			entity.HasKey(s => s.Id);

			// Propriedades da classe base
			entity.Property(s => s.Id)
				.ValueGeneratedOnAdd()
				.IsRequired();

			entity.Property(s => s.ChecklistId)
				.IsRequired();

			entity.Property(s => s.Title)
				.IsRequired()
				.HasMaxLength(200);

			entity.Property(s => s.Order)
				.IsRequired();

			entity.Property(s => s.CreatedDateUtc)
				.IsRequired();

			entity.Property(s => s.LastUpdatedDateUtc)
				.IsRequired();

			// Query Filter através do relacionamento com Checklist
			if (currentUserService.IsAuthenticated)
			{
				var currentUserId = currentUserService.UserId.Value;
				entity.HasQueryFilter(s => s.Checklist.OwnerId == currentUserId);
			}

			// Relacionamento com Checklist
			entity.HasOne(s => s.Checklist)
				.WithMany(c => c.Sections)
				.HasForeignKey(s => s.ChecklistId)
				.OnDelete(DeleteBehavior.Cascade);

			// Configuração específica para BriefingSection
			var briefingEntity = modelBuilder.Entity<BriefingSection>();

			briefingEntity.Property(bs => bs.ContentJson)
				.HasColumnType("TEXT");

			briefingEntity.Property(bs => bs.ContentHtml)
				.HasColumnType("TEXT");

			briefingEntity.Property(bs => bs.ContentLastUpdated);

			// Relacionamento com Media (apenas para BriefingSection)
			briefingEntity.HasMany(bs => bs.Media)
				.WithOne(m => m.Section)
				.HasForeignKey(m => m.SectionId)
				.OnDelete(DeleteBehavior.Cascade);

			// Configuração específica para ChecklistSection
			var checklistEntity = modelBuilder.Entity<ChecklistSection>();

			// Relacionamento com Components (apenas para ChecklistSection)
			checklistEntity.HasMany(cs => cs.Components)
				.WithOne(i => i.Section)
				.HasForeignKey(i => i.SectionId)
				.OnDelete(DeleteBehavior.Cascade);

			// Índices
			entity.HasIndex(s => s.ChecklistId)
				.HasDatabaseName("IX_Sections_ChecklistId");

			entity.HasIndex(s => new { s.ChecklistId, s.Order })
				.HasDatabaseName("IX_Sections_ChecklistId_Order");

			entity.HasIndex("SectionType")
				.HasDatabaseName("IX_Sections_SectionType");
		}
	}
}
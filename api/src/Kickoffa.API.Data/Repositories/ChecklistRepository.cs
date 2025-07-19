using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.FreelancerCustomer;
using Kickoffa.API.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.Repositories
{
	/// <summary>
	/// Implementação do repositório de Checklist
	/// </summary>
	public class ChecklistRepository : BaseRepository<IChecklist, Checklist>, IChecklistRepository
	{
		public ChecklistRepository(KickoffaDbContext context) : base(context)
		{
		}

		/// <summary>
		/// Busca checklist por slug
		/// </summary>
		public async Task<IChecklist?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.Include(c => c.Sections)
				.FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
		}

		/// <summary>
		/// Busca checklist por token de acesso
		/// </summary>
		public async Task<IChecklist?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.Include(c => c.Sections)
				.FirstOrDefaultAsync(c => c.AccessToken == accessToken, cancellationToken);
		}

		/// <summary>
		/// Busca todas as checklists
		/// </summary>
		public override async Task<IEnumerable<IChecklist>> GetAllAsync(CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.Include(c => c.Sections)
				.Include(c => c.Customer)
				.OrderByDescending(c => c.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Busca checklists publicados por proprietário
		/// </summary>
		public async Task<IEnumerable<IChecklist>> GetPublishedByOwnerIdAsync(long ownerId, CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.Include(c => c.Sections)
				.Where(c => c.OwnerId == ownerId && c.Status == ChecklistStatus.Active)
				.OrderByDescending(c => c.CreatedDateUtc)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Verifica se existe checklist com o slug especificado
		/// </summary>
		public async Task<bool> ExistsBySlugAsync(string slug, long? excludeId, CancellationToken cancellationToken)
		{
			var query = _context.Checklists.Where(c => c.Slug == slug);

			if (excludeId.HasValue)
			{
				query = query.Where(c => c.Id != excludeId.Value);
			}

			return await query.AnyAsync(cancellationToken);
		}

		/// <summary>
		/// Override para incluir seções por padrão
		/// </summary>
		public override async Task<IChecklist?> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.Include(c => c.Sections)
				.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
		}

		/// <summary>
		/// Busca checklist por ID incluindo toda a hierarquia: Sections -> Components -> Status
		/// </summary>
		public async Task<IChecklist?> GetByIdWithFullHierarchyAsync(long id, CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.Include(c => c.Sections)
					.ThenInclude(s => ((ChecklistSection)s).Components)
						.ThenInclude(comp => comp.Status)
				.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
		}

		/// <summary>
		/// Busca checklist por ID incluindo toda a hierarquia com componentes específicos
		/// Inclui todos os tipos de componentes (TextComponent, UploadComponent, etc.) e seus relacionamentos
		/// OTIMIZADO: Usa Split Queries para melhor performance
		/// </summary>
		public async Task<IChecklist?> GetByIdWithCompleteHierarchyAsync(long id, CancellationToken cancellationToken)
		{
			// Estratégia otimizada: Split Queries para evitar JOINs complexos
			var checklist = await _context.Checklists
				//.AsSplitQuery() // Divide em múltiplas queries menores e mais eficientes
				.Include(c => c.Customer)
				.Include(c => c.Sections)
					.ThenInclude(s => ((ChecklistSection)s).Components)
						.ThenInclude(comp => comp.Status)
				.Include(c => c.Sections)
					.ThenInclude(s => ((ChecklistSection)s).Components)
						.ThenInclude(comp => ((UploadComponent)comp).AllowedFileTypes)
				.Include(c => c.Sections)
					.ThenInclude(s => ((ChecklistSection)s).Components)
						.ThenInclude(comp => ((UploadComponent)comp).FileTypeSizeConfigs)
				.Include(c => c.Sections)
					.ThenInclude(s => ((ChecklistSection)s).Components)
						.ThenInclude(comp => ((UploadComponent)comp).ComponentFiles)
				.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

			return checklist;
		}

		/// <summary>
		/// Versão SUPER OTIMIZADA: Carregamento manual em etapas para máxima performance
		/// Use quando performance for crítica
		/// </summary>
		public async Task<IChecklist?> GetByIdWithOptimizedHierarchyAsync(long id, CancellationToken cancellationToken)
		{
			// 1. Buscar checklist básico
			var checklist = await _context.Checklists
				.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

			if (checklist == null)
				return null;

			// 2. Carregar seções em uma query separada
			var sections = await _context.Sections
				.Where(s => s.ChecklistId == id)
				.OrderBy(s => s.Order)
				.ToListAsync(cancellationToken);

			// 3. Carregar componentes de todas as seções de checklist em uma query
			var checklistSectionIds = sections
				.Where(s => s is ChecklistSection)
				.Select(s => s.Id)
				.ToList();

			if (checklistSectionIds.Count != 0)
			{
				var components = await _context.Components
					.Include(c => c.Status)
					.Where(c => checklistSectionIds.Contains(c.SectionId))
					.OrderBy(c => c.SectionId).ThenBy(c => c.Order)
					.ToListAsync(cancellationToken);

				// 4. Carregar relacionamentos específicos de UploadComponent
				var uploadComponentIds = components
					.Where(c => c is UploadComponent)
					.Select(c => c.Id)
					.ToList();

				if (uploadComponentIds.Count != 0)
				{
					// Carregar FileTypes permitidos
					var allowedFileTypes = await _context.UploadComponents
						.Where(uc => uploadComponentIds.Contains(uc.Id))
						.SelectMany(uc => uc.AllowedFileTypes)
						.Distinct()
						.ToListAsync(cancellationToken);

					// Carregar arquivos enviados
					var componentFiles = await _context.UploadComponentFiles
						.Where(ucf => uploadComponentIds.Contains(ucf.UploadComponent.Id))
						.ToListAsync(cancellationToken);
				}
			}

			// O EF Core automaticamente conecta os relacionamentos carregados
			return checklist;
		}
		/*
		/// <summary>
		/// Versão com PROJEÇÃO: Busca apenas os dados necessários (mais eficiente para leitura)
		/// Use quando você não precisa modificar as entidades, apenas ler
		/// </summary>
		public async Task<IChecklist?> GetByIdWithProjectionAsync(long id, CancellationToken cancellationToken)
		{
			// Projeção otimizada - busca apenas os campos necessários
			var result = await _context.Checklists
				.Where(c => c.Id == id)
				.Select(c => new
				{
					Checklist = c,
					Sections = c.Sections.OrderBy(s => s.Order).Select(s => new
					{
						Section = s,
						Components = s is ChecklistSection cs
							? cs.Components.OrderBy(comp => comp.Order).Select(comp => new
							{
								Component = comp,
								Status = comp.Status,
								// Propriedades específicas de UploadComponent
								AllowedFileTypes = comp is UploadComponent uc ? uc.AllowedFileTypes : null,
								ComponentFiles = comp is UploadComponent ucf ? ucf.ComponentFiles : null
							})
							: null
					})
				})
				.FirstOrDefaultAsync(cancellationToken);

			return result?.Checklist;
		}*/

		/// <summary>
		/// Versão com FILTRO por owner: Otimizada para buscar apenas checklists do usuário
		/// Inclui verificação de segurança
		/// </summary>
		public async Task<IChecklist?> GetByIdWithFullHierarchyForOwnerAsync(long id, long ownerId, CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.AsSplitQuery()
				.Where(c => c.Id == id && c.OwnerId == ownerId) // Filtro de segurança
				.Include(c => c.Sections)
					.ThenInclude(s => ((ChecklistSection)s).Components)
						.ThenInclude(comp => comp.Status)
				.Include(c => c.Sections)
					.ThenInclude(s => ((ChecklistSection)s).Components)
						.ThenInclude(comp => ((UploadComponent)comp).AllowedFileTypes)
				.Include(c => c.Sections)
					.ThenInclude(s => ((ChecklistSection)s).Components)
						.ThenInclude(comp => ((UploadComponent)comp).ComponentFiles)
				.FirstOrDefaultAsync(cancellationToken);
		}

		/// <summary>
		/// Busca checklist por slug incluindo toda a hierarquia
		/// </summary>
		public async Task<IChecklist?> GetBySlugWithFullHierarchyAsync(string slug, CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.Include(c => c.Sections)
					.ThenInclude(s => ((ChecklistSection)s).Components)
						.ThenInclude(comp => comp.Status)
				.Include(c => c.Sections)
					.ThenInclude(s => ((ChecklistSection)s).Components)
						.ThenInclude(comp => ((UploadComponent)comp).AllowedFileTypes)
				.Include(c => c.Sections)
					.ThenInclude(s => ((ChecklistSection)s).Components)
						.ThenInclude(comp => ((UploadComponent)comp).ComponentFiles)
				.FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
		}

		/// <summary>
		/// Busca checklist por token de acesso incluindo toda a hierarquia
		/// </summary>
		public async Task<IChecklist?> GetByAccessTokenWithFullHierarchyAsync(string accessToken, CancellationToken cancellationToken)
		{
			return await _context.Checklists
				.Include(c => c.Sections)
					.ThenInclude(s => ((ChecklistSection)s).Components)
						.ThenInclude(comp => comp.Status)
				.Include(c => c.Sections)
					.ThenInclude(s => ((ChecklistSection)s).Components)
						.ThenInclude(comp => ((UploadComponent)comp).AllowedFileTypes)
				.Include(c => c.Sections)
					.ThenInclude(s => ((ChecklistSection)s).Components)
						.ThenInclude(comp => ((UploadComponent)comp).ComponentFiles)
				.FirstOrDefaultAsync(c => c.AccessToken == accessToken, cancellationToken);
		}

		/// <summary>
		/// Busca checklists com paginação e filtros
		/// </summary>
		public async Task<(IEnumerable<IChecklist> Checklists, int TotalCount)> GetPagedAsync(
			string? search,
			int page,
			int pageSize,
			IEnumerable<ChecklistStatus> statusFilter,
			CancellationToken cancellationToken)
		{
			IQueryable<Checklist> query = _context
				.Checklists
				.AsNoTracking()
				.Include(c => c.Customer);

			// Aplicar filtro de busca
			if (!string.IsNullOrWhiteSpace(search))
			{
				var searchLower = search.ToLower();

				query = query.Where(c =>
					EF.Functions.ILike(c.Title, $"%{search}%") ||
					(c.Customer != null && (
						// Busca em NaturalPerson
						(c.Customer.Type == CustomerType.NaturalPerson &&
							(
								EF.Functions.ILike(((NaturalPerson)c.Customer).FirstName, $"%{search}%") ||
								EF.Functions.ILike(((NaturalPerson)c.Customer).LastName, $"%{search}%") ||
								((NaturalPerson)c.Customer).Cpf != null && ((NaturalPerson)c.Customer).Cpf.Contains(search)
							)
						) ||

						// Busca em LegalPerson
						(c.Customer.Type == CustomerType.LegalCompany &&
							(
								EF.Functions.ILike(((LegalPerson)c.Customer).Company, $"%{search}%") ||
								((LegalPerson)c.Customer).Cnpj != null && ((LegalPerson)c.Customer).Cnpj.Contains(search)
							)
						) ||

						// Busca em email (comum a ambos)
						EF.Functions.ILike(c.Customer.Email!, $"%{search}%")
					))
				);
			}

			// Aplicar filtro de status
			if (statusFilter.Any())
			{
				query = query.Where(c => statusFilter.Contains(c.Status));
			}

			// Contar total de registros
			var totalCount = await query.CountAsync(cancellationToken);

			// Aplicar paginação
			var offset = (page - 1) * pageSize;
			var checklists = await query
				.Skip(offset)
				.Take(pageSize)
				.ToListAsync(cancellationToken);

			return (checklists, totalCount);
		}
	}
}
using Kickoffa.API.Data.EntityFramework.Mapping;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.AppUser;
using Kickoffa.API.Domain.Models.Components;
using Kickoffa.API.Domain.Models.Components.Base;
using Kickoffa.API.Domain.Models.FreelancerCustomer;
using Kickoffa.API.Domain.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kickoffa.API.Data.EntityFramework.Context
{
	public interface IKickoffaDbContext : IDisposable
	{
		DbSet<Customer> Customers { get; }
		DbSet<Checklist> Checklists { get; }
		DbSet<Section> Sections { get; }
		DbSet<Component> Components { get; }
		DbSet<TextComponent> TextComponents { get; }
		DbSet<UploadComponent> UploadComponents { get; }
		DbSet<ConfirmationComponent> ConfirmationComponents { get; }
		DbSet<CheckboxComponent> CheckboxComponents { get; }
		DbSet<SignatureComponent> SignatureComponents { get; }
		DbSet<ComponentStatus> ComponentStatuses { get; }
		DbSet<BriefingMedia> BriefingMedias { get; }
		DbSet<FileType> FileTypes { get; }
		DbSet<UploadComponentFile> UploadComponentFiles { get; }
		// Users é gerenciado pelo Identity, não precisamos expor aqui
		IDbContextTransaction BeginTransaction();
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
		int SaveChanges();
	}

	public class KickoffaDbContext : IdentityDbContext<User, Role, long>, IKickoffaDbContext
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly ICustomerEntityFrameworkMapping _customerEntityFrameworkMapping;
		private readonly IUserEntityFrameworkMapping _userEntityFrameworkMapping;
		private readonly IChecklistEntityFrameworkMapping _checklistEntityFrameworkMapping;
		private readonly ISectionEntityFrameworkMapping _sectionEntityFrameworkMapping;
		private readonly IComponentEntityFrameworkMapping _componentEntityFrameworkMapping;
		private readonly IComponentStatusEntityFrameworkMapping _componentStatusEntityFrameworkMapping;
		private readonly IBriefingMediaEntityFrameworkMapping _briefingMediaEntityFrameworkMapping;
		private readonly IFileTypeEntityFrameworkMapping _fileTypeEntityFrameworkMapping;
		private readonly IUploadComponentFileEntityFrameworkMapping _uploadComponentFileEntityFrameworkMapping;
		private readonly ITextComponentEntityFrameworkMapping _textComponentEntityFrameworkMapping;
		private readonly IUploadComponentEntityFrameworkMapping _uploadComponentEntityFrameworkMapping;
		private readonly IConfirmationComponentEntityFrameworkMapping _confirmationComponentEntityFrameworkMapping;
		private readonly ICheckboxComponentEntityFrameworkMapping _checkboxComponentEntityFrameworkMapping;
		private readonly ISignatureComponentEntityFrameworkMapping _signatureComponentEntityFrameworkMapping;


		public KickoffaDbContext(
			DbContextOptions<KickoffaDbContext> options,
			ICurrentUserService currentUserService,
			ICustomerEntityFrameworkMapping customerEntityFrameworkMapping,
			IUserEntityFrameworkMapping userEntityFrameworkMapping,
			IChecklistEntityFrameworkMapping checklistEntityFrameworkMapping,
			ISectionEntityFrameworkMapping sectionEntityFrameworkMapping,
			IComponentEntityFrameworkMapping componentEntityFrameworkMapping,
			IComponentStatusEntityFrameworkMapping componentStatusEntityFrameworkMapping,
			IBriefingMediaEntityFrameworkMapping briefingMediaEntityFrameworkMapping,
			IFileTypeEntityFrameworkMapping fileTypeEntityFrameworkMapping,
			IUploadComponentFileEntityFrameworkMapping uploadComponentFileEntityFrameworkMapping,
			ITextComponentEntityFrameworkMapping textComponentEntityFrameworkMapping,
			IUploadComponentEntityFrameworkMapping uploadComponentEntityFrameworkMapping,
			IConfirmationComponentEntityFrameworkMapping confirmationComponentEntityFrameworkMapping,
			ICheckboxComponentEntityFrameworkMapping checkboxComponentEntityFrameworkMapping,
			ISignatureComponentEntityFrameworkMapping signatureComponentEntityFrameworkMapping) : base(options)
		{
			_currentUserService = currentUserService;
			_customerEntityFrameworkMapping = customerEntityFrameworkMapping;
			_userEntityFrameworkMapping = userEntityFrameworkMapping;
			_checklistEntityFrameworkMapping = checklistEntityFrameworkMapping;
			_sectionEntityFrameworkMapping = sectionEntityFrameworkMapping;
			_componentEntityFrameworkMapping = componentEntityFrameworkMapping;
			_componentStatusEntityFrameworkMapping = componentStatusEntityFrameworkMapping;
			_briefingMediaEntityFrameworkMapping = briefingMediaEntityFrameworkMapping;
			_fileTypeEntityFrameworkMapping = fileTypeEntityFrameworkMapping;
			_uploadComponentFileEntityFrameworkMapping = uploadComponentFileEntityFrameworkMapping;
			_textComponentEntityFrameworkMapping = textComponentEntityFrameworkMapping;
			_uploadComponentEntityFrameworkMapping = uploadComponentEntityFrameworkMapping;
			_confirmationComponentEntityFrameworkMapping = confirmationComponentEntityFrameworkMapping;
			_checkboxComponentEntityFrameworkMapping = checkboxComponentEntityFrameworkMapping;
			_signatureComponentEntityFrameworkMapping = signatureComponentEntityFrameworkMapping;
		}

		public DbSet<Customer> Customers { get; private set; }
		public DbSet<Checklist> Checklists { get; private set; }
		public DbSet<Section> Sections { get; private set; }
		public DbSet<Component> Components { get; private set; }
		public DbSet<TextComponent> TextComponents { get; private set; }
		public DbSet<UploadComponent> UploadComponents { get; private set; }
		public DbSet<ConfirmationComponent> ConfirmationComponents { get; private set; }
		public DbSet<CheckboxComponent> CheckboxComponents { get; private set; }
		public DbSet<SignatureComponent> SignatureComponents { get; private set; }
		public DbSet<ComponentStatus> ComponentStatuses { get; private set; }
		public DbSet<BriefingMedia> BriefingMedias { get; private set; }
		public DbSet<FileType> FileTypes { get; private set; }
		public DbSet<UploadComponentFile> UploadComponentFiles { get; private set; }

		public IDbContextTransaction BeginTransaction()
		{
			return Database.BeginTransaction();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Mapeamentos sem filtro de usuário (dados globais)
			_customerEntityFrameworkMapping.Map(modelBuilder);
			_userEntityFrameworkMapping.Map(modelBuilder);
			_fileTypeEntityFrameworkMapping.Map(modelBuilder);
			_briefingMediaEntityFrameworkMapping.Map(modelBuilder);
			_uploadComponentFileEntityFrameworkMapping.Map(modelBuilder);

			// Mapeamentos com filtro de usuário (exceto Component base)
			_checklistEntityFrameworkMapping.Map(modelBuilder, _currentUserService);
			_sectionEntityFrameworkMapping.Map(modelBuilder, _currentUserService);
			_componentStatusEntityFrameworkMapping.Map(modelBuilder, _currentUserService);

			// ✅ MAPEAMENTO BASE COMPONENT POR ÚLTIMO (evita vazamento de configurações TPC)
			_componentEntityFrameworkMapping.Map(modelBuilder, _currentUserService);

			// ✅ MAPEAMENTOS ESPECÍFICOS DE COMPONENTES PRIMEIRO (para TPC funcionar corretamente)
			_textComponentEntityFrameworkMapping.Map(modelBuilder);
			_uploadComponentEntityFrameworkMapping.Map(modelBuilder);
			_confirmationComponentEntityFrameworkMapping.Map(modelBuilder);
			_checkboxComponentEntityFrameworkMapping.Map(modelBuilder);
			_signatureComponentEntityFrameworkMapping.Map(modelBuilder);
		}
	}
}
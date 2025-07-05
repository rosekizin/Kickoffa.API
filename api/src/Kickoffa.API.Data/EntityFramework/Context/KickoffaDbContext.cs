using Kickoffa.API.Data.EntityFramework.Mapping;
using Kickoffa.API.Domain.Models.FreelancerCustomer;
using Kickoffa.API.Domain.Models.AppUser;
using Kickoffa.API.Domain.Models.Items;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Kickoffa.API.Domain.Models.Items.Base;
using Kickoffa.API.Domain.Models;

namespace Kickoffa.API.Data.EntityFramework.Context
{
	public interface IKickoffaDbContext : IDisposable
	{
		DbSet<Customer> Customers { get; }
		DbSet<Checklist> Checklists { get; }
		DbSet<Section> Sections { get; }
		DbSet<Item> Items { get; }
		DbSet<TextItem> TextItems { get; }
		DbSet<UploadItem> UploadItems { get; }
		DbSet<ConfirmationItem> ConfirmationItems { get; }
		DbSet<CheckboxItem> CheckboxItems { get; }
		DbSet<SignatureItem> SignatureItems { get; }
		DbSet<ItemStatus> ItemStatuses { get; }
		DbSet<BriefingMedia> BriefingMedias { get; }
		DbSet<FileType> FileTypes { get; }
		DbSet<UploadItemFile> UploadItemFiles { get; }
		// Users é gerenciado pelo Identity, não precisamos expor aqui
		IDbContextTransaction BeginTransaction();
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
		int SaveChanges();
	}

	public class KickoffaDbContext : IdentityDbContext<User, Role, long>, IKickoffaDbContext
	{
		private readonly ICustomerEntityFrameworkMapping _customerEntityFrameworkMapping;
		private readonly IUserEntityFrameworkMapping _userEntityFrameworkMapping;
		private readonly IChecklistEntityFrameworkMapping _checklistEntityFrameworkMapping;
		private readonly ISectionEntityFrameworkMapping _sectionEntityFrameworkMapping;
		private readonly IItemEntityFrameworkMapping _itemEntityFrameworkMapping;
		private readonly IItemStatusEntityFrameworkMapping _itemStatusEntityFrameworkMapping;
		private readonly IBriefingMediaEntityFrameworkMapping _briefingMediaEntityFrameworkMapping;
		private readonly IFileTypeEntityFrameworkMapping _fileTypeEntityFrameworkMapping;
		private readonly IUploadItemFileEntityFrameworkMapping _uploadItemFileEntityFrameworkMapping;
		private readonly ITextItemEntityFrameworkMapping _textItemEntityFrameworkMapping;
		private readonly IUploadItemEntityFrameworkMapping _uploadItemEntityFrameworkMapping;
		private readonly IConfirmationItemEntityFrameworkMapping _confirmationItemEntityFrameworkMapping;
		private readonly ICheckboxItemEntityFrameworkMapping _checkboxItemEntityFrameworkMapping;
		private readonly ISignatureItemEntityFrameworkMapping _signatureItemEntityFrameworkMapping;


		public KickoffaDbContext(
			DbContextOptions<KickoffaDbContext> options,
			ICustomerEntityFrameworkMapping customerEntityFrameworkMapping,
			IUserEntityFrameworkMapping userEntityFrameworkMapping,
			IChecklistEntityFrameworkMapping checklistEntityFrameworkMapping,
			ISectionEntityFrameworkMapping sectionEntityFrameworkMapping,
			IItemEntityFrameworkMapping itemEntityFrameworkMapping,
			IItemStatusEntityFrameworkMapping itemStatusEntityFrameworkMapping,
			IBriefingMediaEntityFrameworkMapping briefingMediaEntityFrameworkMapping,
			IFileTypeEntityFrameworkMapping fileTypeEntityFrameworkMapping,
			IUploadItemFileEntityFrameworkMapping uploadItemFileEntityFrameworkMapping,
			ITextItemEntityFrameworkMapping textItemEntityFrameworkMapping,
			IUploadItemEntityFrameworkMapping uploadItemEntityFrameworkMapping,
			IConfirmationItemEntityFrameworkMapping confirmationItemEntityFrameworkMapping,
			ICheckboxItemEntityFrameworkMapping checkboxItemEntityFrameworkMapping,
			ISignatureItemEntityFrameworkMapping signatureItemEntityFrameworkMapping) : base(options)
		{
			_customerEntityFrameworkMapping = customerEntityFrameworkMapping;
			_userEntityFrameworkMapping = userEntityFrameworkMapping;
			_checklistEntityFrameworkMapping = checklistEntityFrameworkMapping;
			_sectionEntityFrameworkMapping = sectionEntityFrameworkMapping;
			_itemEntityFrameworkMapping = itemEntityFrameworkMapping;
			_itemStatusEntityFrameworkMapping = itemStatusEntityFrameworkMapping;
			_briefingMediaEntityFrameworkMapping = briefingMediaEntityFrameworkMapping;
			_fileTypeEntityFrameworkMapping = fileTypeEntityFrameworkMapping;
			_uploadItemFileEntityFrameworkMapping = uploadItemFileEntityFrameworkMapping;
			_textItemEntityFrameworkMapping = textItemEntityFrameworkMapping;
			_uploadItemEntityFrameworkMapping = uploadItemEntityFrameworkMapping;
			_confirmationItemEntityFrameworkMapping = confirmationItemEntityFrameworkMapping;
			_checkboxItemEntityFrameworkMapping = checkboxItemEntityFrameworkMapping;
			_signatureItemEntityFrameworkMapping = signatureItemEntityFrameworkMapping;
		}

		public DbSet<Customer> Customers { get; private set; }
		public DbSet<Checklist> Checklists { get; private set; }
		public DbSet<Section> Sections { get; private set; }
		public DbSet<Item> Items { get; private set; }
		public DbSet<TextItem> TextItems { get; private set; }
		public DbSet<UploadItem> UploadItems { get; private set; }
		public DbSet<ConfirmationItem> ConfirmationItems { get; private set; }
		public DbSet<CheckboxItem> CheckboxItems { get; private set; }
		public DbSet<SignatureItem> SignatureItems { get; private set; }
		public DbSet<ItemStatus> ItemStatuses { get; private set; }
		public DbSet<BriefingMedia> BriefingMedias { get; private set; }
		public DbSet<FileType> FileTypes { get; private set; }
		public DbSet<UploadItemFile> UploadItemFiles { get; private set; }

		public IDbContextTransaction BeginTransaction()
		{
			return Database.BeginTransaction();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			_customerEntityFrameworkMapping.Map(modelBuilder);
			_userEntityFrameworkMapping.Map(modelBuilder);
			_checklistEntityFrameworkMapping.Map(modelBuilder);
			_sectionEntityFrameworkMapping.Map(modelBuilder);
			_itemEntityFrameworkMapping.Map(modelBuilder);
			_itemStatusEntityFrameworkMapping.Map(modelBuilder);
			_briefingMediaEntityFrameworkMapping.Map(modelBuilder);
			_fileTypeEntityFrameworkMapping.Map(modelBuilder);
			_uploadItemFileEntityFrameworkMapping.Map(modelBuilder);
			_textItemEntityFrameworkMapping.Map(modelBuilder);
			_uploadItemEntityFrameworkMapping.Map(modelBuilder);
			_confirmationItemEntityFrameworkMapping.Map(modelBuilder);
			_checkboxItemEntityFrameworkMapping.Map(modelBuilder);
			_signatureItemEntityFrameworkMapping.Map(modelBuilder);
		}
	}
}
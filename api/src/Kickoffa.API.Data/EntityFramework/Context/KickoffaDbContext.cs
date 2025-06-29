using Kickoffa.API.Data.EntityFramework.Mapping;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.FreelancerCustomer;
using Kickoffa.API.Domain.Models.AppUser;
using Kickoffa.API.Domain.Models.Items;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Kickoffa.API.Domain.Models.Items.Base;

namespace Kickoffa.API.Data.EntityFramework.Context
{
	public interface IKickoffaDbContext : IDisposable
	{
		//DbSet<Customer> Customers { get; }
		//DbSet<Checklist> Checklists { get; }
		//DbSet<Section> Sections { get; }
		//DbSet<Item> Items { get; }
		//DbSet<ItemStatus> ItemStatuses { get; }
		//DbSet<BriefingMedia> BriefingMedias { get; }
		DbSet<FileType> FileTypes { get; }
		//DbSet<UploadItemFileType> UploadItemFileTypes { get; }
		// Users é gerenciado pelo Identity, não precisamos expor aqui
		IDbContextTransaction BeginTransaction();
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
		int SaveChanges();
	}

	public class KickoffaDbContext : IdentityDbContext<User, Role, long>, IKickoffaDbContext
	{
		private readonly ICustomerEntityFrameworkMapping _customerEntityFrameworkMapping;
		private readonly IUserEntityFrameworkMapping _userEntityFrameworkMapping;
		//private readonly IChecklistEntityFrameworkMapping _checklistEntityFrameworkMapping;
		//private readonly ISectionEntityFrameworkMapping _sectionEntityFrameworkMapping;
		//private readonly IItemEntityFrameworkMapping _itemEntityFrameworkMapping;
		//private readonly IItemStatusEntityFrameworkMapping _itemStatusEntityFrameworkMapping;
		//private readonly IBriefingMediaEntityFrameworkMapping _briefingMediaEntityFrameworkMapping;
		private readonly IFileTypeEntityFrameworkMapping _fileTypeEntityFrameworkMapping;
		//private readonly IUploadItemFileTypeEntityFrameworkMapping _uploadItemFileTypeEntityFrameworkMapping;

		public KickoffaDbContext(
			DbContextOptions<KickoffaDbContext> options,
			ICustomerEntityFrameworkMapping customerEntityFrameworkMapping,
			IUserEntityFrameworkMapping userEntityFrameworkMapping,
			IFileTypeEntityFrameworkMapping fileTypeEntityFrameworkMapping) : base(options)
		{
			_customerEntityFrameworkMapping = customerEntityFrameworkMapping;
			_userEntityFrameworkMapping = userEntityFrameworkMapping;
			_fileTypeEntityFrameworkMapping = fileTypeEntityFrameworkMapping;
		}
		/*
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
			IUploadItemFileTypeEntityFrameworkMapping uploadItemFileTypeEntityFrameworkMapping) : base(options)
		{
			_customerEntityFrameworkMapping = customerEntityFrameworkMapping;
			_userEntityFrameworkMapping = userEntityFrameworkMapping;
			_checklistEntityFrameworkMapping = checklistEntityFrameworkMapping;
			_sectionEntityFrameworkMapping = sectionEntityFrameworkMapping;
			_itemEntityFrameworkMapping = itemEntityFrameworkMapping;
			_itemStatusEntityFrameworkMapping = itemStatusEntityFrameworkMapping;
			_briefingMediaEntityFrameworkMapping = briefingMediaEntityFrameworkMapping;
			_fileTypeEntityFrameworkMapping = fileTypeEntityFrameworkMapping;
			_uploadItemFileTypeEntityFrameworkMapping = uploadItemFileTypeEntityFrameworkMapping;
		}*/

		//public DbSet<Customer> Customers { get; private set; }
		//public DbSet<Checklist> Checklists { get; private set; }
		//public DbSet<Section> Sections { get; private set; }
		//public DbSet<Item> Items { get; private set; }
		//public DbSet<ItemStatus> ItemStatuses { get; private set; }
		//public DbSet<BriefingMedia> BriefingMedias { get; private set; }
		public DbSet<FileType> FileTypes { get; private set; }
		//public DbSet<UploadItemFileType> UploadItemFileTypes { get; private set; }

		public IDbContextTransaction BeginTransaction()
		{
			return Database.BeginTransaction();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			_customerEntityFrameworkMapping.Map(modelBuilder);
			_userEntityFrameworkMapping.Map(modelBuilder);
			//_checklistEntityFrameworkMapping.Map(modelBuilder);
			//_sectionEntityFrameworkMapping.Map(modelBuilder);
			//_itemEntityFrameworkMapping.Map(modelBuilder);
			//_itemStatusEntityFrameworkMapping.Map(modelBuilder);
			//_briefingMediaEntityFrameworkMapping.Map(modelBuilder);
			_fileTypeEntityFrameworkMapping.Map(modelBuilder);
			//_uploadItemFileTypeEntityFrameworkMapping.Map(modelBuilder);
		}
	}
}
using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.EntityFramework.Mapping;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Kickoffa.API.Data.UnitTests.Repositories.DbContext
{
	public class KickoffaDbContextFixture : IDisposable
	{
		private readonly KickoffaDbContext _dbContext;
		private readonly ICustomerEntityFrameworkMapping _customerEntityFrameworkMapping;
		private readonly IUserEntityFrameworkMapping _userEntityFrameworkMapping;
		private readonly IChecklistEntityFrameworkMapping _checklistEntityFrameworkMapping;
		private readonly ISectionEntityFrameworkMapping _sectionEntityFrameworkMapping;
		private readonly IItemEntityFrameworkMapping _itemEntityFrameworkMapping;
		private readonly IItemStatusEntityFrameworkMapping _itemStatusEntityFrameworkMapping;
		private readonly IBriefingMediaEntityFrameworkMapping _briefingMediaEntityFrameworkMapping;
		private readonly IFileTypeEntityFrameworkMapping _fileTypeEntityFrameworkMapping;
		private readonly IUploadItemFileTypeEntityFrameworkMapping _uploadItemFileTypeEntityFrameworkMapping;
		private readonly IUploadItemFileEntityFrameworkMapping _uploadItemFileEntityFrameworkMapping;

		public KickoffaDbContextFixture()
		{
			_customerEntityFrameworkMapping = Substitute.For<ICustomerEntityFrameworkMapping>();
			_userEntityFrameworkMapping = Substitute.For<IUserEntityFrameworkMapping>();
			_checklistEntityFrameworkMapping = Substitute.For<IChecklistEntityFrameworkMapping>();
			_sectionEntityFrameworkMapping = Substitute.For<ISectionEntityFrameworkMapping>();
			_itemEntityFrameworkMapping = Substitute.For<IItemEntityFrameworkMapping>();
			_itemStatusEntityFrameworkMapping = Substitute.For<IItemStatusEntityFrameworkMapping>();
			_briefingMediaEntityFrameworkMapping = Substitute.For<IBriefingMediaEntityFrameworkMapping>();
			_fileTypeEntityFrameworkMapping = Substitute.For<IFileTypeEntityFrameworkMapping>();
			_uploadItemFileTypeEntityFrameworkMapping = Substitute.For<IUploadItemFileTypeEntityFrameworkMapping>();
			_uploadItemFileEntityFrameworkMapping = Substitute.For<IUploadItemFileEntityFrameworkMapping>();

			_dbContext = GetNewDbContext();
		}

		public KickoffaDbContext KickoffaDbContext => _dbContext;

		public KickoffaDbContext GetNewDbContext()
		{
			var options = new DbContextOptionsBuilder<KickoffaDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

			var newDbContext = new KickoffaDbContext(
				options,
				_customerEntityFrameworkMapping,
				_userEntityFrameworkMapping,
				_checklistEntityFrameworkMapping,
				_sectionEntityFrameworkMapping,
				_itemEntityFrameworkMapping,
				_itemStatusEntityFrameworkMapping,
				_briefingMediaEntityFrameworkMapping,
				_fileTypeEntityFrameworkMapping,
				_uploadItemFileTypeEntityFrameworkMapping,
				_uploadItemFileEntityFrameworkMapping);

			return newDbContext;
		}

		public void Dispose()
		{
			_dbContext.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
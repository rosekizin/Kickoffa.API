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
		private readonly IUploadComponentFileTypeSizeEntityFrameworkMapping _uploadComponentFileTypeSizeEntityFrameworkMapping;

		public KickoffaDbContextFixture()
		{
			_customerEntityFrameworkMapping = Substitute.For<ICustomerEntityFrameworkMapping>();
			_userEntityFrameworkMapping = Substitute.For<IUserEntityFrameworkMapping>();
			_checklistEntityFrameworkMapping = Substitute.For<IChecklistEntityFrameworkMapping>();
			_sectionEntityFrameworkMapping = Substitute.For<ISectionEntityFrameworkMapping>();
			_componentEntityFrameworkMapping = Substitute.For<IComponentEntityFrameworkMapping>();
			_componentStatusEntityFrameworkMapping = Substitute.For<IComponentStatusEntityFrameworkMapping>();
			_briefingMediaEntityFrameworkMapping = Substitute.For<IBriefingMediaEntityFrameworkMapping>();
			_fileTypeEntityFrameworkMapping = Substitute.For<IFileTypeEntityFrameworkMapping>();
			_uploadComponentFileEntityFrameworkMapping = Substitute.For<IUploadComponentFileEntityFrameworkMapping>();
			_textComponentEntityFrameworkMapping = Substitute.For<ITextComponentEntityFrameworkMapping>();
			_uploadComponentEntityFrameworkMapping = Substitute.For<IUploadComponentEntityFrameworkMapping>();
			_confirmationComponentEntityFrameworkMapping = Substitute.For<IConfirmationComponentEntityFrameworkMapping>();
			_checkboxComponentEntityFrameworkMapping = Substitute.For<ICheckboxComponentEntityFrameworkMapping>();
			_signatureComponentEntityFrameworkMapping = Substitute.For<ISignatureComponentEntityFrameworkMapping>();
			_uploadComponentFileTypeSizeEntityFrameworkMapping = Substitute.For<IUploadComponentFileTypeSizeEntityFrameworkMapping>();

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
				_componentEntityFrameworkMapping,
				_componentStatusEntityFrameworkMapping,
				_briefingMediaEntityFrameworkMapping,
				_fileTypeEntityFrameworkMapping,
				_uploadComponentFileEntityFrameworkMapping,
				_uploadComponentFileTypeSizeEntityFrameworkMapping,
				_textComponentEntityFrameworkMapping,
				_uploadComponentEntityFrameworkMapping,
				_confirmationComponentEntityFrameworkMapping,
				_checkboxComponentEntityFrameworkMapping,
				_signatureComponentEntityFrameworkMapping);

			return newDbContext;
		}

		public void Dispose()
		{
			_dbContext.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
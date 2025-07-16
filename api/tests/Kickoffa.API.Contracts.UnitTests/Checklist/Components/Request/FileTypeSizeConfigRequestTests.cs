using Kickoffa.API.Contracts.Checklist.Components.Request;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components.Request
{
	public class FileTypeSizeConfigRequestTests
	{
		[Fact]
		public void FileTypeSizeConfigRequest_WithValidData_ShouldPassValidation()
		{
			// Arrange
			var request = new FileTypeSizeConfigRequest
			{
				FileTypeId = 1L,
				MaxSizeMB = 10
			};

			// Act
			var validationResults = ValidateModel(request);

			// Assert
			Assert.Empty(validationResults);
		}

		[Fact]
		public void FileTypeSizeConfigRequest_WithZeroFileTypeId_ShouldFailValidation()
		{
			// Arrange
			var request = new FileTypeSizeConfigRequest
			{
				FileTypeId = 0L,
				MaxSizeMB = 10
			};

			// Act
			var validationResults = ValidateModel(request);

			// Assert
			Assert.Single(validationResults);
			Assert.Contains(validationResults, v => v.ErrorMessage == "O ID do tipo de arquivo deve ser maior que zero" && v.MemberNames.Contains("FileTypeId"));
		}

		[Fact]
		public void FileTypeSizeConfigRequest_WithZeroMaxSize_ShouldFailValidation()
		{
			// Arrange
			var request = new FileTypeSizeConfigRequest
			{
				FileTypeId = 1L,
				MaxSizeMB = 0
			};

			// Act
			var validationResults = ValidateModel(request);

			// Assert
			Assert.Single(validationResults);
			Assert.Contains(validationResults, v => v.ErrorMessage == "O tamanho máximo deve ser maior que zero" && v.MemberNames.Contains("MaxSizeMB"));
		}

		[Fact]
		public void FileTypeSizeConfigRequest_WithTooLargeMaxSize_ShouldFailValidation()
		{
			// Arrange
			var request = new FileTypeSizeConfigRequest
			{
				FileTypeId = 1L,
				MaxSizeMB = 1001
			};

			// Act
			var validationResults = ValidateModel(request);

			// Assert
			Assert.Single(validationResults);
			Assert.Contains(validationResults, v => v.ErrorMessage == "O tamanho máximo não pode exceder 1000 MB" && v.MemberNames.Contains("MaxSizeMB"));
		}

		[Theory]
		[InlineData(1)]
		[InlineData(50)]
		[InlineData(100)]
		[InlineData(500)]
		[InlineData(1000)]
		public void FileTypeSizeConfigRequest_WithValidMaxSizes_ShouldPassValidation(int maxSizeMB)
		{
			// Arrange
			var request = new FileTypeSizeConfigRequest
			{
				FileTypeId = 1L,
				MaxSizeMB = maxSizeMB
			};

			// Act
			var validationResults = ValidateModel(request);

			// Assert
			Assert.Empty(validationResults);
			Assert.Equal(maxSizeMB, request.MaxSizeMB);
		}

		[Fact]
		public void FileTypeSizeConfigRequest_AsRecord_ShouldSupportEquality()
		{
			// Arrange
			var request1 = new FileTypeSizeConfigRequest
			{
				FileTypeId = 1L,
				MaxSizeMB = 10
			};

			var request2 = new FileTypeSizeConfigRequest
			{
				FileTypeId = 1L,
				MaxSizeMB = 10
			};

			// Act & Assert
			Assert.Equal(request1, request2);
			Assert.True(request1 == request2);
			Assert.False(request1 != request2);
		}

		[Fact]
		public void FileTypeSizeConfigRequest_ShouldBeSealed()
		{
			// Assert
			var type = typeof(FileTypeSizeConfigRequest);
			Assert.True(type.IsSealed);
		}

		#region Helper Methods

		private static List<ValidationResult> ValidateModel(object model)
		{
			var validationResults = new List<ValidationResult>();
			var validationContext = new ValidationContext(model);
			Validator.TryValidateObject(model, validationContext, validationResults, true);
			return validationResults;
		}

		#endregion
	}
}
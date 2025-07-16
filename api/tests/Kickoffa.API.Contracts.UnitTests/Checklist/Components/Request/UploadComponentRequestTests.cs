using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components.Request;

public class UploadComponentRequestTests
{
    [Fact]
    public void UploadComponentRequest_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new UploadComponentRequest
        {
            Id = 1,
            Title = "Test Upload Component",
            Description = "Test description",
            IsRequired = true,
            Order = 1,
            Placeholder = "Drop files here",
            AllowedFileTypeIds = new List<long> { 1, 2, 3 }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void UploadComponentRequest_WithMinimalData_ShouldPassValidation()
    {
        // Arrange
        var request = new UploadComponentRequest
        {
            Id = 1,
            Title = "UP", // Minimum length
            IsRequired = false,
            Order = 1,
            AllowedFileTypeIds = new List<long> { 1 }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void UploadComponentRequest_WithEmptyAllowedFileTypeIds_ShouldFailValidation()
    {
        // Arrange
        var request = new UploadComponentRequest
        {
            Id = 1,
            Title = "Test Upload Component",
            IsRequired = true,
            Order = 1,
            AllowedFileTypeIds = new List<long>()
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Componentes de upload devem ter pelo menos um tipo de arquivo permitido" && v.MemberNames.Contains("AllowedFileTypeIds"));
    }

    [Fact]
    public void UploadComponentRequest_WithValidFileTypeSizeConfigs_ShouldPassValidation()
    {
        // Arrange
        var request = new UploadComponentRequest
        {
            Id = 1,
            Title = "Test Upload Component",
            IsRequired = true,
            Order = 1,
            AllowedFileTypeIds = new List<long> { 1, 2 },
            FileTypeSizeConfigs = new List<FileTypeSizeConfigRequest>
            {
                new FileTypeSizeConfigRequest { FileTypeId = 1, MaxSizeMB = 10 },
                new FileTypeSizeConfigRequest { FileTypeId = 2, MaxSizeMB = 20 }
            }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void UploadComponentRequest_WithInvalidFileTypeSizeConfig_ShouldFailValidation()
    {
        // Arrange
        var request = new UploadComponentRequest
        {
            Id = 1,
            Title = "Test Upload Component",
            IsRequired = true,
            Order = 1,
            AllowedFileTypeIds = new List<long> { 1 },
            FileTypeSizeConfigs = new List<FileTypeSizeConfigRequest>
            {
                new FileTypeSizeConfigRequest { FileTypeId = 2, MaxSizeMB = 10 } // FileTypeId 2 not in allowed list
            }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Configuração de tamanho para tipo de arquivo 2 não está na lista de tipos permitidos" && v.MemberNames.Contains("FileTypeSizeConfigs"));
    }

    [Fact]
    public void UploadComponentRequest_WithDuplicateFileTypeSizeConfigs_ShouldFailValidation()
    {
        // Arrange
        var request = new UploadComponentRequest
        {
            Id = 1,
            Title = "Test Upload Component",
            IsRequired = true,
            Order = 1,
            AllowedFileTypeIds = new List<long> { 1 },
            FileTypeSizeConfigs = new List<FileTypeSizeConfigRequest>
            {
                new FileTypeSizeConfigRequest { FileTypeId = 1, MaxSizeMB = 10 },
                new FileTypeSizeConfigRequest { FileTypeId = 1, MaxSizeMB = 20 } // Duplicate
            }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Configuração duplicada encontrada para tipo de arquivo 1" && v.MemberNames.Contains("FileTypeSizeConfigs"));
    }

    [Fact]
    public void UploadComponentRequest_WithComponentFiles_ShouldPassValidation()
    {
        // Arrange
        var request = new UploadComponentRequest
        {
            Id = 1,
            Title = "Test Upload Component",
            IsRequired = true,
            Order = 1,
            AllowedFileTypeIds = new List<long> { 1 },
            ComponentFiles = new List<UploadComponentFileRequest>
            {
                new UploadComponentFileRequest
                {
                    FileName = "test.jpg",
                    StoragePath = "/uploads/test.jpg",
                    FileSize = 1024,
                    ContentType = "image/jpeg",
                    Sha256Hash = "abc123"
                }
            }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData("Drop files here")]
    [InlineData("Upload your documents")]
    [InlineData("")]
    [InlineData(null)]
    public void UploadComponentRequest_WithDifferentPlaceholders_ShouldPassValidation(string? placeholder)
    {
        // Arrange
        var request = new UploadComponentRequest
        {
            Id = 1,
            Title = "Test Upload Component",
            IsRequired = true,
            Order = 1,
            Placeholder = placeholder,
            AllowedFileTypeIds = new List<long> { 1 }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
        Assert.Equal(placeholder, request.Placeholder);
    }

    [Fact]
    public void UploadComponentRequest_InheritsFromComponentRequest_ShouldValidateBaseProperties()
    {
        // Arrange
        var request = new UploadComponentRequest
        {
            Id = 1,
            Title = "", // Invalid - empty title
            IsRequired = true,
            Order = 0, // Invalid - zero order
            AllowedFileTypeIds = new List<long> { 1 }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Equal(2, validationResults.Count);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título do componente é obrigatório" && v.MemberNames.Contains("Title"));
        Assert.Contains(validationResults, v => v.ErrorMessage == "A ordem do componente deve ser maior que zero" && v.MemberNames.Contains("Order"));
    }

    [Fact]
    public void UploadComponentRequest_WithMultipleValidationErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var request = new UploadComponentRequest
        {
            Id = 1,
            Title = "A", // Too short
            IsRequired = true,
            Order = -1, // Invalid
            AllowedFileTypeIds = new List<long>() // Empty
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Equal(3, validationResults.Count);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título do componente deve ter entre 2 e 200 caracteres");
        Assert.Contains(validationResults, v => v.ErrorMessage == "A ordem do componente deve ser maior que zero");
        Assert.Contains(validationResults, v => v.ErrorMessage == "Componentes de upload devem ter pelo menos um tipo de arquivo permitido");
    }

    [Fact]
    public void UploadComponentRequest_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var allowedFileTypeIds = new List<long> { 1, 2 };
        
        var request1 = new UploadComponentRequest
        {
            Id = 1,
            Title = "Test Upload Component",
            IsRequired = true,
            Order = 1,
            Placeholder = "Drop files",
            AllowedFileTypeIds = allowedFileTypeIds
        };

        var request2 = new UploadComponentRequest
        {
            Id = 1,
            Title = "Test Upload Component",
            IsRequired = true,
            Order = 1,
            Placeholder = "Drop files",
            AllowedFileTypeIds = allowedFileTypeIds
        };

        // Act & Assert
        Assert.Equal(request1, request2);
        Assert.True(request1 == request2);
        Assert.False(request1 != request2);
    }

    [Fact]
    public void UploadComponentRequest_ShouldInheritFromComponentRequest()
    {
        // Arrange
        var request = new UploadComponentRequest
        {
            Id = 1,
            Title = "Test Upload Component",
            IsRequired = true,
            Order = 1,
            AllowedFileTypeIds = new List<long> { 1 }
        };

        // Assert
        Assert.IsAssignableFrom<ComponentRequest>(request);
    }

    [Fact]
    public void UploadComponentRequest_ShouldBeSealed()
    {
        // Assert
        var type = typeof(UploadComponentRequest);
        Assert.True(type.IsSealed);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void UploadComponentRequest_WithMultipleAllowedFileTypes_ShouldPassValidation(int fileTypeCount)
    {
        // Arrange
        var allowedFileTypeIds = Enumerable.Range(1, fileTypeCount).Select(i => (long)i).ToList();
        
        var request = new UploadComponentRequest
        {
            Id = 1,
            Title = "Test Upload Component",
            IsRequired = true,
            Order = 1,
            AllowedFileTypeIds = allowedFileTypeIds
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
        Assert.Equal(fileTypeCount, request.AllowedFileTypeIds.Count());
	}

	[Fact]
	public void UploadComponentRequest_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(UploadComponentRequest);

		// Act & Assert
		type.GetProperty(nameof(UploadComponentRequest.Id))!.AssertPropertyName("id").AssertRequired(Required.Default);
		type.GetProperty(nameof(UploadComponentRequest.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
		type.GetProperty(nameof(UploadComponentRequest.Description))!.AssertPropertyName("description").AssertRequired(Required.Default);
		type.GetProperty(nameof(UploadComponentRequest.IsRequired))!.AssertPropertyName("isRequired").AssertRequired(Required.Always);
		type.GetProperty(nameof(UploadComponentRequest.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
		type.GetProperty(nameof(UploadComponentRequest.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
		type.GetProperty(nameof(UploadComponentRequest.Placeholder))!.AssertPropertyName("placeholder").AssertRequired(Required.Default);
		type.GetProperty(nameof(UploadComponentRequest.AllowedFileTypeIds))!.AssertPropertyName("allowedFileTypeIds").AssertRequired(Required.Always);
		type.GetProperty(nameof(UploadComponentRequest.FileTypeSizeConfigs))!.AssertPropertyName("fileTypeSizeConfigs").AssertRequired(Required.Default);
		type.GetProperty(nameof(UploadComponentRequest.ComponentFiles))!.AssertPropertyName("componentFiles").AssertRequired(Required.Default);
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
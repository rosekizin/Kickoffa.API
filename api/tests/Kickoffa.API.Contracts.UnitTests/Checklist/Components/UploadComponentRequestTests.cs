using Kickoffa.API.Contracts.Checklist.Components;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components;

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

public class UploadComponentFileRequestTests
{
    [Fact]
    public void UploadComponentFileRequest_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var fileName = "document.pdf";
        var storagePath = "/uploads/2024/01/document.pdf";
        var fileSize = 1024L;
        var contentType = "application/pdf";
        var sha256Hash = "abc123def456";

        // Act
        var request = new UploadComponentFileRequest
        {
            FileName = fileName,
            StoragePath = storagePath,
            FileSize = fileSize,
            ContentType = contentType,
            Sha256Hash = sha256Hash
        };

        // Assert
        Assert.Equal(fileName, request.FileName);
        Assert.Equal(storagePath, request.StoragePath);
        Assert.Equal(fileSize, request.FileSize);
        Assert.Equal(contentType, request.ContentType);
        Assert.Equal(sha256Hash, request.Sha256Hash);
    }

    [Fact]
    public void UploadComponentFileRequest_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var request1 = new UploadComponentFileRequest
        {
            FileName = "test.jpg",
            StoragePath = "/uploads/test.jpg",
            FileSize = 2048L,
            ContentType = "image/jpeg",
            Sha256Hash = "hash123"
        };

        var request2 = new UploadComponentFileRequest
        {
            FileName = "test.jpg",
            StoragePath = "/uploads/test.jpg",
            FileSize = 2048L,
            ContentType = "image/jpeg",
            Sha256Hash = "hash123"
        };

        // Act & Assert
        Assert.Equal(request1, request2);
        Assert.True(request1 == request2);
        Assert.False(request1 != request2);
    }

    [Fact]
    public void UploadComponentFileRequest_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var request1 = new UploadComponentFileRequest
        {
            FileName = "test1.jpg",
            StoragePath = "/uploads/test1.jpg",
            FileSize = 1024L,
            ContentType = "image/jpeg",
            Sha256Hash = "hash1"
        };

        var request2 = new UploadComponentFileRequest
        {
            FileName = "test2.jpg", // Different filename
            StoragePath = "/uploads/test1.jpg",
            FileSize = 1024L,
            ContentType = "image/jpeg",
            Sha256Hash = "hash1"
        };

        // Act & Assert
        Assert.NotEqual(request1, request2);
        Assert.False(request1 == request2);
        Assert.True(request1 != request2);
    }

    [Theory]
    [InlineData("document.pdf", "application/pdf")]
    [InlineData("image.jpg", "image/jpeg")]
    [InlineData("video.mp4", "video/mp4")]
    [InlineData("text.txt", "text/plain")]
    public void UploadComponentFileRequest_WithDifferentFileTypes_ShouldSetCorrectly(string fileName, string contentType)
    {
        // Arrange & Act
        var request = new UploadComponentFileRequest
        {
            FileName = fileName,
            StoragePath = $"/uploads/{fileName}",
            FileSize = 1024L,
            ContentType = contentType,
            Sha256Hash = "hash123"
        };

        // Assert
        Assert.Equal(fileName, request.FileName);
        Assert.Equal(contentType, request.ContentType);
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(1024L)]
    [InlineData(1048576L)] // 1MB
    [InlineData(long.MaxValue)]
    public void UploadComponentFileRequest_WithDifferentFileSizes_ShouldSetCorrectly(long fileSize)
    {
        // Arrange & Act
        var request = new UploadComponentFileRequest
        {
            FileName = "test.txt",
            StoragePath = "/uploads/test.txt",
            FileSize = fileSize,
            ContentType = "text/plain",
            Sha256Hash = "hash123"
        };

        // Assert
        Assert.Equal(fileSize, request.FileSize);
    }

    [Fact]
    public void UploadComponentFileRequest_ShouldBeSealed()
    {
        // Assert
        var type = typeof(UploadComponentFileRequest);
        Assert.True(type.IsSealed);
    }
}

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

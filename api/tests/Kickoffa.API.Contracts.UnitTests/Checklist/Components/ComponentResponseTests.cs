using Kickoffa.API.Contracts.Checklist.Components;
using Kickoffa.API.Contracts.FileType;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components;

public class ComponentResponseTests
{
    // Since ComponentResponse is abstract, we'll use a concrete implementation for testing
    private sealed record TestComponentResponse : ComponentResponse
    {
        public TestComponentResponse()
        {
            Type = "test";
        }
    }

    [Fact]
    public void ComponentResponse_WithRequiredProperties_ShouldCreateSuccessfully()
    {
        // Arrange
        var id = 1L;
        var sectionId = 2L;
        var title = "Test Component";
        var type = "test";
        var isRequired = true;
        var order = 1;
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;

        // Act
        var response = new TestComponentResponse
        {
            Id = id,
            SectionId = sectionId,
            Title = title,
            Type = type,
            IsRequired = isRequired,
            Order = order,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal(sectionId, response.SectionId);
        Assert.Equal(title, response.Title);
        Assert.Equal(type, response.Type);
        Assert.Equal(isRequired, response.IsRequired);
        Assert.Equal(order, response.Order);
        Assert.Equal(createdDate, response.CreatedDateUtc);
        Assert.Equal(lastUpdatedDate, response.LastUpdatedDateUtc);
        Assert.Null(response.Description);
        Assert.Null(response.Status);
    }

    [Fact]
    public void ComponentResponse_WithAllProperties_ShouldSetCorrectly()
    {
        // Arrange
        var id = 1L;
        var sectionId = 2L;
        var title = "Test Component";
        var description = "Test description";
        var type = "test";
        var isRequired = false;
        var order = 3;
        var status = CreateComponentStatusResponse();
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;

        // Act
        var response = new TestComponentResponse
        {
            Id = id,
            SectionId = sectionId,
            Title = title,
            Description = description,
            Type = type,
            IsRequired = isRequired,
            Order = order,
            Status = status,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal(sectionId, response.SectionId);
        Assert.Equal(title, response.Title);
        Assert.Equal(description, response.Description);
        Assert.Equal(type, response.Type);
        Assert.Equal(isRequired, response.IsRequired);
        Assert.Equal(order, response.Order);
        Assert.Equal(status, response.Status);
        Assert.Equal(createdDate, response.CreatedDateUtc);
        Assert.Equal(lastUpdatedDate, response.LastUpdatedDateUtc);
    }

    [Fact]
    public void ComponentResponse_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;

        var response1 = new TestComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Component",
            Type = "test",
            IsRequired = true,
            Order = 1,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        var response2 = new TestComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Component",
            Type = "test",
            IsRequired = true,
            Order = 1,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Act & Assert
        Assert.Equal(response1, response2);
        Assert.True(response1 == response2);
        Assert.False(response1 != response2);
    }

    [Fact]
    public void ComponentResponse_JsonSerialization_ShouldUseCorrectPropertyNames()
    {
        // Arrange
        var response = new TestComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Component",
            Description = "Test description",
            Type = "test",
            IsRequired = true,
            Order = 1,
            CreatedDateUtc = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            LastUpdatedDateUtc = new DateTime(2024, 1, 2, 12, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var json = JsonConvert.SerializeObject(response);

        // Assert
        Assert.Contains("\"id\":", json);
        Assert.Contains("\"sectionId\":", json);
        Assert.Contains("\"title\":", json);
        Assert.Contains("\"description\":", json);
        Assert.Contains("\"type\":", json);
        Assert.Contains("\"isRequired\":", json);
        Assert.Contains("\"order\":", json);
        Assert.Contains("\"createdDateUtc\":", json);
        Assert.Contains("\"lastUpdatedDateUtc\":", json);
    }

    #region Helper Methods

    private static ComponentStatusResponse CreateComponentStatusResponse()
    {
        return new ComponentStatusResponse
        {
            Id = 1L,
            ComponentId = 1L,
            IsCompleted = true,
            CompletedAt = DateTime.UtcNow,
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };
    }

    #endregion
}

public class CheckboxComponentResponseTests
{
    [Fact]
    public void CheckboxComponentResponse_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var response = new CheckboxComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Checkbox",
            Type = "checkbox",
            IsRequired = true,
            Order = 1,
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.Equal("checkbox", response.Type);
        Assert.Equal(1L, response.Id);
        Assert.Equal("Test Checkbox", response.Title);
    }

    [Fact]
    public void CheckboxComponentResponse_ShouldInheritFromComponentResponse()
    {
        // Arrange
        var response = new CheckboxComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Checkbox",
            IsRequired = true,
            Type = "checkbox",
			Order = 1,
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.IsType<ComponentResponse>(response, exactMatch: false);
    }

    [Fact]
    public void CheckboxComponentResponse_ShouldBeSealed()
    {
        // Assert
        var type = typeof(CheckboxComponentResponse);
        Assert.True(type.IsSealed);
    }
}

public class TextComponentResponseTests
{
    [Fact]
    public void TextComponentResponse_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var response = new TextComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Text Component",
            Type = "text",
            IsRequired = true,
            Order = 1,
            Placeholder = "Enter text here",
            MaxLength = 100,
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.Equal("text", response.Type);
        Assert.Equal("Enter text here", response.Placeholder);
        Assert.Equal(100, response.MaxLength);
    }

    [Fact]
    public void TextComponentResponse_WithNullOptionalProperties_ShouldSetCorrectly()
    {
        // Arrange & Act
        var response = new TextComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Text Component",
            IsRequired = true,
            Order = 1,
			Type = "text",
			Placeholder = null,
            MaxLength = null,
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.Null(response.Placeholder);
        Assert.Null(response.MaxLength);
    }

    [Fact]
    public void TextComponentResponse_ShouldInheritFromComponentResponse()
    {
        // Arrange
        var response = new TextComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Text Component",
            IsRequired = true,
            Order = 1,
			Type = "text",
			CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.IsType<ComponentResponse>(response, exactMatch: false);
    }
}

public class UploadComponentResponseTests
{
    [Fact]
    public void UploadComponentResponse_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var allowedFileTypes = new List<FileTypeResponse>
        {
            new FileTypeResponse
            {
                Id = 1L,
                MimeType = "image/jpeg",
                Extension = ".jpg",
                DisplayName = "JPEG Image"
            }
        };

        // Act
        var response = new UploadComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Upload Component",
            Type = "upload",
            IsRequired = true,
            Order = 1,
            Placeholder = "Drop files here",
            AllowedFileTypes = allowedFileTypes,
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.Equal("upload", response.Type);
        Assert.Equal("Drop files here", response.Placeholder);
        Assert.Equal(allowedFileTypes, response.AllowedFileTypes);
    }

    [Fact]
    public void UploadComponentResponse_TypeShouldBeSetAutomatically()
    {
        // Arrange & Act
        var response = new UploadComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Upload Component",
            IsRequired = true,
			Order = 1,
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.Equal("upload", response.Type);
    }

    [Fact]
    public void UploadComponentResponse_ShouldInheritFromComponentResponse()
    {
        // Arrange
        var response = new UploadComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Upload Component",
            IsRequired = true,
            Order = 1,
			Type = "upload",
			CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.IsType<ComponentResponse>(response, exactMatch: false);
    }
}

public class SignatureComponentResponseTests
{
    [Fact]
    public void SignatureComponentResponse_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var response = new SignatureComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Signature Component",
            IsRequired = true,
            Order = 1,
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.Equal("signature", response.Type);
        Assert.Equal(1L, response.Id);
        Assert.Equal("Test Signature Component", response.Title);
    }


	[Fact]
	public void UploadComponentResponse_TypeShouldBeSetAutomatically()
	{
		// Arrange & Act
		var response = new SignatureComponentResponse
		{
			Id = 1L,
			SectionId = 2L,
			Title = "Test Signature Component",
			IsRequired = true,
			Order = 1,
			CreatedDateUtc = DateTime.UtcNow,
			LastUpdatedDateUtc = DateTime.UtcNow
		};

		// Assert
		Assert.Equal("signature", response.Type);
	}

	[Fact]
    public void SignatureComponentResponse_ShouldInheritFromComponentResponse()
    {
        // Arrange
        var response = new SignatureComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Signature Component",
            IsRequired = true,
            Order = 1,
			Type = "signature",
			CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.IsType<ComponentResponse>(response, exactMatch: false);
    }
}

public class ConfirmationComponentResponseTests
{
    [Fact]
    public void ConfirmationComponentResponse_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var response = new ConfirmationComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Confirmation Component",
            Type = "confirmation",
            IsRequired = true,
            Order = 1,
            ConfirmationText = "I agree to the terms",
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.Equal("confirmation", response.Type);
        Assert.Equal("I agree to the terms", response.ConfirmationText);
    }

    [Fact]
    public void ConfirmationComponentResponse_WithNullConfirmationText_ShouldSetCorrectly()
    {
        // Arrange & Act
        var response = new ConfirmationComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Confirmation Component",
            IsRequired = true,
            Order = 1,
			Type = "confirmation",
			ConfirmationText = null,
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.Null(response.ConfirmationText);
    }

    [Fact]
    public void ConfirmationComponentResponse_ShouldInheritFromComponentResponse()
    {
        // Arrange
        var response = new ConfirmationComponentResponse
        {
            Id = 1L,
            SectionId = 2L,
            Title = "Test Confirmation Component",
            IsRequired = true,
            Order = 1,
			Type = "confirmation",
			CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.IsType<ComponentResponse>(response, exactMatch: false);
    }
}

public class ComponentStatusResponseTests
{
    [Fact]
    public void ComponentStatusResponse_WithRequiredProperties_ShouldCreateSuccessfully()
    {
        // Arrange
        var id = 1L;
        var componentId = 2L;
        var isCompleted = true;
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;

        // Act
        var response = new ComponentStatusResponse
        {
            Id = id,
            ComponentId = componentId,
            IsCompleted = isCompleted,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal(componentId, response.ComponentId);
        Assert.Equal(isCompleted, response.IsCompleted);
        Assert.Equal(createdDate, response.CreatedDateUtc);
        Assert.Equal(lastUpdatedDate, response.LastUpdatedDateUtc);
        Assert.Null(response.CompletedAt);
        Assert.Null(response.TextResponse);
        Assert.Null(response.SignatureData);
        Assert.Null(response.UploadedFiles);
    }

    [Fact]
    public void ComponentStatusResponse_WithAllProperties_ShouldSetCorrectly()
    {
        // Arrange
        var id = 1L;
        var componentId = 2L;
        var isCompleted = true;
        var completedAt = DateTime.UtcNow;
        var textResponse = "User response text";
        var signatureData = "signature_data_base64";
        var uploadedFiles = new List<UploadedFileResponse>
        {
            new UploadedFileResponse
            {
                Id = 1L,
                ComponentStatusId = 1L,
                FileName = "test.pdf",
                OriginalName = "original_test.pdf",
                MimeType = "application/pdf",
                Size = 1024L,
                Url = "/uploads/test.pdf",
                CreatedDateUtc = DateTime.UtcNow
            }
        };
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;

        // Act
        var response = new ComponentStatusResponse
        {
            Id = id,
            ComponentId = componentId,
            IsCompleted = isCompleted,
            CompletedAt = completedAt,
            TextResponse = textResponse,
            SignatureData = signatureData,
            UploadedFiles = uploadedFiles,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal(componentId, response.ComponentId);
        Assert.Equal(isCompleted, response.IsCompleted);
        Assert.Equal(completedAt, response.CompletedAt);
        Assert.Equal(textResponse, response.TextResponse);
        Assert.Equal(signatureData, response.SignatureData);
        Assert.Equal(uploadedFiles, response.UploadedFiles);
        Assert.Equal(createdDate, response.CreatedDateUtc);
        Assert.Equal(lastUpdatedDate, response.LastUpdatedDateUtc);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComponentStatusResponse_WithDifferentCompletionStates_ShouldSetCorrectly(bool isCompleted)
    {
        // Arrange & Act
        var response = new ComponentStatusResponse
        {
            Id = 1L,
            ComponentId = 2L,
            IsCompleted = isCompleted,
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(isCompleted, response.IsCompleted);
    }

    [Fact]
    public void ComponentStatusResponse_ShouldBeSealed()
    {
        // Assert
        var type = typeof(ComponentStatusResponse);
        Assert.True(type.IsSealed);
    }
}
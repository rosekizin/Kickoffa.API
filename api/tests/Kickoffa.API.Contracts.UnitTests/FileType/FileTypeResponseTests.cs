using Kickoffa.API.Contracts.FileType;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.UnitTests.FileType;

public class FileTypeResponseTests
{
    [Fact]
    public void FileTypeResponse_WithRequiredProperties_ShouldCreateSuccessfully()
    {
        // Arrange
        var id = 1L;
        var mimeType = "image/jpeg";
        var extension = ".jpg";
        var displayName = "JPEG Image";

        // Act
        var response = new FileTypeResponse
        {
            Id = id,
            MimeType = mimeType,
            Extension = extension,
            DisplayName = displayName
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal(mimeType, response.MimeType);
        Assert.Equal(extension, response.Extension);
        Assert.Equal(displayName, response.DisplayName);
        Assert.Null(response.Description);
        Assert.Null(response.Category);
        Assert.Null(response.RecommendedMaxSizeMB);
    }

    [Fact]
    public void FileTypeResponse_WithAllProperties_ShouldSetCorrectly()
    {
        // Arrange
        var id = 1L;
        var mimeType = "application/pdf";
        var extension = ".pdf";
        var displayName = "PDF Document";
        var description = "Portable Document Format";
        var category = "Documents";
        var recommendedMaxSizeMB = 10;

        // Act
        var response = new FileTypeResponse
        {
            Id = id,
            MimeType = mimeType,
            Extension = extension,
            DisplayName = displayName,
            Description = description,
            Category = category,
            RecommendedMaxSizeMB = recommendedMaxSizeMB
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal(mimeType, response.MimeType);
        Assert.Equal(extension, response.Extension);
        Assert.Equal(displayName, response.DisplayName);
        Assert.Equal(description, response.Description);
        Assert.Equal(category, response.Category);
        Assert.Equal(recommendedMaxSizeMB, response.RecommendedMaxSizeMB);
    }

    [Fact]
    public void FileTypeResponse_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var response1 = new FileTypeResponse
        {
            Id = 1L,
            MimeType = "image/png",
            Extension = ".png",
            DisplayName = "PNG Image",
            Description = "Portable Network Graphics",
            Category = "Images",
            RecommendedMaxSizeMB = 5
        };

        var response2 = new FileTypeResponse
        {
            Id = 1L,
            MimeType = "image/png",
            Extension = ".png",
            DisplayName = "PNG Image",
            Description = "Portable Network Graphics",
            Category = "Images",
            RecommendedMaxSizeMB = 5
        };

        // Act & Assert
        Assert.Equal(response1, response2);
        Assert.True(response1 == response2);
        Assert.False(response1 != response2);
    }

    [Fact]
    public void FileTypeResponse_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var response1 = new FileTypeResponse
        {
            Id = 1L,
            MimeType = "image/png",
            Extension = ".png",
            DisplayName = "PNG Image"
        };

        var response2 = new FileTypeResponse
        {
            Id = 2L, // Different ID
            MimeType = "image/png",
            Extension = ".png",
            DisplayName = "PNG Image"
        };

        // Act & Assert
        Assert.NotEqual(response1, response2);
        Assert.False(response1 == response2);
        Assert.True(response1 != response2);
    }

    [Fact]
    public void FileTypeResponse_JsonSerialization_ShouldUseCorrectPropertyNames()
    {
        // Arrange
        var response = new FileTypeResponse
        {
            Id = 1L,
            MimeType = "text/plain",
            Extension = ".txt",
            DisplayName = "Text File",
            Description = "Plain text document",
            Category = "Documents",
            RecommendedMaxSizeMB = 1
        };

        // Act
        var json = JsonConvert.SerializeObject(response);

        // Assert
        Assert.Contains("\"id\":", json);
        Assert.Contains("\"mimeType\":", json);
        Assert.Contains("\"extension\":", json);
        Assert.Contains("\"displayName\":", json);
        Assert.Contains("\"description\":", json);
        Assert.Contains("\"category\":", json);
        Assert.Contains("\"recommendedMaxSizeMB\":", json);
    }

    [Fact]
    public void FileTypeResponse_JsonDeserialization_ShouldWorkCorrectly()
    {
        // Arrange
        var json = @"{
            ""id"": 1,
            ""mimeType"": ""image/gif"",
            ""extension"": "".gif"",
            ""displayName"": ""GIF Image"",
            ""description"": ""Graphics Interchange Format"",
            ""category"": ""Images"",
            ""recommendedMaxSizeMB"": 2
        }";

        // Act
        var response = JsonConvert.DeserializeObject<FileTypeResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(1L, response.Id);
        Assert.Equal("image/gif", response.MimeType);
        Assert.Equal(".gif", response.Extension);
        Assert.Equal("GIF Image", response.DisplayName);
        Assert.Equal("Graphics Interchange Format", response.Description);
        Assert.Equal("Images", response.Category);
        Assert.Equal(2, response.RecommendedMaxSizeMB);
    }

    [Fact]
    public void FileTypeResponse_JsonDeserialization_WithNullOptionalProperties_ShouldWorkCorrectly()
    {
        // Arrange
        var json = @"{
            ""id"": 2,
            ""mimeType"": ""video/mp4"",
            ""extension"": "".mp4"",
            ""displayName"": ""MP4 Video""
        }";

        // Act
        var response = JsonConvert.DeserializeObject<FileTypeResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2L, response.Id);
        Assert.Equal("video/mp4", response.MimeType);
        Assert.Equal(".mp4", response.Extension);
        Assert.Equal("MP4 Video", response.DisplayName);
        Assert.Null(response.Description);
        Assert.Null(response.Category);
        Assert.Null(response.RecommendedMaxSizeMB);
    }

    [Theory]
    [InlineData("image/jpeg", ".jpg", "JPEG Image")]
    [InlineData("image/png", ".png", "PNG Image")]
    [InlineData("application/pdf", ".pdf", "PDF Document")]
    [InlineData("text/plain", ".txt", "Text File")]
    [InlineData("video/mp4", ".mp4", "MP4 Video")]
    public void FileTypeResponse_WithDifferentFileTypes_ShouldSetCorrectly(string mimeType, string extension, string displayName)
    {
        // Arrange & Act
        var response = new FileTypeResponse
        {
            Id = 1L,
            MimeType = mimeType,
            Extension = extension,
            DisplayName = displayName
        };

        // Assert
        Assert.Equal(mimeType, response.MimeType);
        Assert.Equal(extension, response.Extension);
        Assert.Equal(displayName, response.DisplayName);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void FileTypeResponse_WithDifferentMaxSizes_ShouldSetCorrectly(int maxSizeMB)
    {
        // Arrange & Act
        var response = new FileTypeResponse
        {
            Id = 1L,
            MimeType = "application/octet-stream",
            Extension = ".bin",
            DisplayName = "Binary File",
            RecommendedMaxSizeMB = maxSizeMB
        };

        // Assert
        Assert.Equal(maxSizeMB, response.RecommendedMaxSizeMB);
    }

    [Theory]
    [InlineData("Images")]
    [InlineData("Documents")]
    [InlineData("Videos")]
    [InlineData("Audio")]
    [InlineData("Archives")]
    public void FileTypeResponse_WithDifferentCategories_ShouldSetCorrectly(string category)
    {
        // Arrange & Act
        var response = new FileTypeResponse
        {
            Id = 1L,
            MimeType = "application/octet-stream",
            Extension = ".bin",
            DisplayName = "Binary File",
            Category = category
        };

        // Assert
        Assert.Equal(category, response.Category);
    }

    [Fact]
    public void FileTypeResponse_ShouldBeSealed()
    {
        // Assert
        var type = typeof(FileTypeResponse);
        Assert.True(type.IsSealed);
    }

    [Fact]
    public void FileTypeResponse_ShouldBeRecord()
    {
        // Assert
        var type = typeof(FileTypeResponse);
        Assert.True(type.IsClass);
        
        // Records have compiler-generated methods
        var equalsMethod = type.GetMethod("Equals", new[] { typeof(FileTypeResponse) });
        Assert.NotNull(equalsMethod);
        
        var getHashCodeMethod = type.GetMethod("GetHashCode", Type.EmptyTypes);
        Assert.NotNull(getHashCodeMethod);
    }

    [Fact]
    public void FileTypeResponse_WithZeroRecommendedMaxSize_ShouldSetCorrectly()
    {
        // Arrange & Act
        var response = new FileTypeResponse
        {
            Id = 1L,
            MimeType = "text/plain",
            Extension = ".txt",
            DisplayName = "Text File",
            RecommendedMaxSizeMB = 0
        };

        // Assert
        Assert.Equal(0, response.RecommendedMaxSizeMB);
    }

    [Fact]
    public void FileTypeResponse_WithNegativeId_ShouldSetCorrectly()
    {
        // Arrange & Act
        var response = new FileTypeResponse
        {
            Id = -1L,
            MimeType = "text/plain",
            Extension = ".txt",
            DisplayName = "Text File"
        };

        // Assert
        Assert.Equal(-1L, response.Id);
    }
}

public class FileTypesSearchResponseTests
{
    [Fact]
    public void FileTypesSearchResponse_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var fileTypes = new List<FileTypeResponse>
        {
            new FileTypeResponse
            {
                Id = 1L,
                MimeType = "image/jpeg",
                Extension = ".jpg",
                DisplayName = "JPEG Image"
            },
            new FileTypeResponse
            {
                Id = 2L,
                MimeType = "image/png",
                Extension = ".png",
                DisplayName = "PNG Image"
            }
        };
        var totalCount = 10;

        // Act
        var response = new FileTypesSearchResponse(fileTypes, totalCount);

        // Assert
        Assert.Equal(fileTypes, response.FileTypes);
        Assert.Equal(totalCount, response.TotalCount);
    }

    [Fact]
    public void FileTypesSearchResponse_WithEmptyFileTypes_ShouldCreateSuccessfully()
    {
        // Arrange
        var fileTypes = new List<FileTypeResponse>();
        var totalCount = 0;

        // Act
        var response = new FileTypesSearchResponse(fileTypes, totalCount);

        // Assert
        Assert.Equal(fileTypes, response.FileTypes);
        Assert.Equal(totalCount, response.TotalCount);
        Assert.Empty(response.FileTypes);
    }

    [Fact]
    public void FileTypesSearchResponse_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var fileTypes = new List<FileTypeResponse>
        {
            new FileTypeResponse
            {
                Id = 1L,
                MimeType = "text/plain",
                Extension = ".txt",
                DisplayName = "Text File"
            }
        };

        var response1 = new FileTypesSearchResponse(fileTypes, 5);
        var response2 = new FileTypesSearchResponse(fileTypes, 5);

        // Act & Assert
        Assert.Equal(response1, response2);
        Assert.True(response1 == response2);
        Assert.False(response1 != response2);
    }

    [Fact]
    public void FileTypesSearchResponse_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var fileTypes = new List<FileTypeResponse>();
        var response1 = new FileTypesSearchResponse(fileTypes, 5);
        var response2 = new FileTypesSearchResponse(fileTypes, 10); // Different total count

        // Act & Assert
        Assert.NotEqual(response1, response2);
        Assert.False(response1 == response2);
        Assert.True(response1 != response2);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void FileTypesSearchResponse_WithDifferentTotalCounts_ShouldSetCorrectly(int totalCount)
    {
        // Arrange
        var fileTypes = new List<FileTypeResponse>();

        // Act
        var response = new FileTypesSearchResponse(fileTypes, totalCount);

        // Assert
        Assert.Equal(totalCount, response.TotalCount);
    }

    [Fact]
    public void FileTypesSearchResponse_ShouldBeRecord()
    {
        // Assert
        var type = typeof(FileTypesSearchResponse);
        Assert.True(type.IsClass);
        
        // Records have compiler-generated methods
        var equalsMethod = type.GetMethod("Equals", new[] { typeof(FileTypesSearchResponse) });
        Assert.NotNull(equalsMethod);
        
        var getHashCodeMethod = type.GetMethod("GetHashCode", Type.EmptyTypes);
        Assert.NotNull(getHashCodeMethod);
	}

	[Fact]
	public void FileTypeResponse_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(FileTypeResponse);

		// Act & Assert
		type.GetProperty(nameof(FileTypeResponse.Id))!.AssertPropertyName("id").AssertRequired(Required.Always);
		type.GetProperty(nameof(FileTypeResponse.MimeType))!.AssertPropertyName("mimeType").AssertRequired(Required.Always);
		type.GetProperty(nameof(FileTypeResponse.Extension))!.AssertPropertyName("extension").AssertRequired(Required.Always);
		type.GetProperty(nameof(FileTypeResponse.DisplayName))!.AssertPropertyName("displayName").AssertRequired(Required.Always);
		type.GetProperty(nameof(FileTypeResponse.Description))!.AssertPropertyName("description").AssertRequired(Required.Default);
		type.GetProperty(nameof(FileTypeResponse.Category))!.AssertPropertyName("category").AssertRequired(Required.Default);
		type.GetProperty(nameof(FileTypeResponse.RecommendedMaxSizeMB))!.AssertPropertyName("recommendedMaxSizeMB").AssertRequired(Required.Default);
	}
}
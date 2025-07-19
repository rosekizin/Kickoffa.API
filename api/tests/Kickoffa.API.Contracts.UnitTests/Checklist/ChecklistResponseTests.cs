using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.UnitTests.Checklist;

public class ChecklistResponseTests
{
    [Fact]
    public void ChecklistResponse_WithRequiredProperties_ShouldCreateSuccessfully()
    {
        // Arrange
        var id = 1L;
        var ownerId = 2L;
        var customerId = 3L;
        var title = "Test Checklist";
        var slug = "test-checklist";
        var status = ChecklistStatus.Active;
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;
        var sections = new List<SectionResponse>();

        // Act
        var response = new ChecklistResponse
        {
            Id = id,
            OwnerId = ownerId,
            CustomerId = customerId,
            Title = title,
            Slug = slug,
            Status =  status,
            Sections = sections,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal(ownerId, response.OwnerId);
        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal(title, response.Title);
        Assert.Equal(slug, response.Slug);
        Assert.Equal(status, response.Status);
        Assert.Equal(sections, response.Sections);
        Assert.Equal(createdDate, response.CreatedDateUtc);
        Assert.Equal(lastUpdatedDate, response.LastUpdatedDateUtc);
        Assert.Null(response.Customer);
        Assert.Null(response.Description);
        Assert.Null(response.Deadline);
        Assert.Null(response.AccessToken);
    }

    [Fact]
    public void ChecklistResponse_WithAllProperties_ShouldSetCorrectly()
    {
        // Arrange
        var id = 1L;
        var ownerId = 2L;
        var customerId = 3L;
        var customer = CreateCustomerResponse(customerId);
        var title = "Test Checklist";
        var slug = "test-checklist";
        var description = "Test description";
        var deadline = DateTime.UtcNow.AddDays(7);
        var accessToken = "test-access-token";
		var status = ChecklistStatus.Active;
		var sections = new List<SectionResponse> { CreateSectionResponse(1) };
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;

        // Act
        var response = new ChecklistResponse
        {
            Id = id,
            OwnerId = ownerId,
            CustomerId = customerId,
            Customer = customer,
            Title = title,
            Slug = slug,
            Description = description,
            Deadline = deadline,
            AccessToken = accessToken,
            Status = status,
            Sections = sections,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal(ownerId, response.OwnerId);
        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal(customer, response.Customer);
        Assert.Equal(title, response.Title);
        Assert.Equal(slug, response.Slug);
        Assert.Equal(description, response.Description);
        Assert.Equal(deadline, response.Deadline);
        Assert.Equal(accessToken, response.AccessToken);
        Assert.Equal(status, response.Status);
        Assert.Equal(sections, response.Sections);
        Assert.Equal(createdDate, response.CreatedDateUtc);
        Assert.Equal(lastUpdatedDate, response.LastUpdatedDateUtc);
    }

    [Fact]
    public void ChecklistResponse_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;
        var sections = new List<SectionResponse>();

        var response1 = new ChecklistResponse
        {
            Id = 1L,
            OwnerId = 2L,
            CustomerId = 3L,
            Title = "Test Checklist",
            Slug = "test-checklist",
            Status = ChecklistStatus.Active,
            Sections = sections,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        var response2 = new ChecklistResponse
        {
            Id = 1L,
            OwnerId = 2L,
            CustomerId = 3L,
            Title = "Test Checklist",
            Slug = "test-checklist",
            Status = ChecklistStatus.Active,
            Sections = sections,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Act & Assert
        Assert.Equal(response1, response2);
        Assert.True(response1 == response2);
        Assert.False(response1 != response2);
    }

    [Fact]
    public void ChecklistResponse_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;
        var sections = new List<SectionResponse>();

        var response1 = new ChecklistResponse
        {
            Id = 1L,
            OwnerId = 2L,
            CustomerId = 3L,
            Title = "Test Checklist",
            Slug = "test-checklist",
            Status = ChecklistStatus.Active,
            Sections = sections,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        var response2 = new ChecklistResponse
        {
            Id = 2L, // Different ID
            OwnerId = 2L,
            CustomerId = 3L,
            Title = "Test Checklist",
            Slug = "test-checklist",
            Status = ChecklistStatus.Active,
            Sections = sections,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Act & Assert
        Assert.NotEqual(response1, response2);
        Assert.False(response1 == response2);
        Assert.True(response1 != response2);
    }

    [Fact]
    public void ChecklistResponse_JsonSerialization_ShouldUseCorrectPropertyNames()
    {
        // Arrange
        var response = new ChecklistResponse
        {
            Id = 1L,
            OwnerId = 2L,
            CustomerId = 3L,
            Title = "Test Checklist",
            Slug = "test-checklist",
            Description = "Test description",
            Deadline = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            AccessToken = "test-token",
            Status = ChecklistStatus.Active,
            Sections = [],
            CreatedDateUtc = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            LastUpdatedDateUtc = new DateTime(2024, 1, 2, 12, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var json = JsonConvert.SerializeObject(response);

        // Assert
        Assert.Contains("\"id\":", json);
        Assert.Contains("\"ownerId\":", json);
        Assert.Contains("\"customerId\":", json);
        Assert.Contains("\"title\":", json);
        Assert.Contains("\"slug\":", json);
        Assert.Contains("\"description\":", json);
        Assert.Contains("\"deadline\":", json);
        Assert.Contains("\"accessToken\":", json);
        Assert.Contains("\"status\":", json);
        Assert.Contains("\"sections\":", json);
        Assert.Contains("\"createdDateUtc\":", json);
        Assert.Contains("\"lastUpdatedDateUtc\":", json);
    }

    [Fact]
    public void ChecklistResponse_JsonDeserialization_ShouldWorkCorrectly()
    {
        // Arrange
        var json = @"{
            ""id"": 1,
            ""ownerId"": 2,
            ""customerId"": 3,
            ""title"": ""Test Checklist"",
            ""slug"": ""test-checklist"",
            ""description"": ""Test description"",
            ""deadline"": ""2024-12-31T23:59:59Z"",
            ""accessToken"": ""test-token"",
            ""status"": 1,
            ""sections"": [],
            ""createdDateUtc"": ""2024-01-01T12:00:00Z"",
            ""lastUpdatedDateUtc"": ""2024-01-02T12:00:00Z""
        }";

        // Act
        var response = JsonConvert.DeserializeObject<ChecklistResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(1L, response.Id);
        Assert.Equal(2L, response.OwnerId);
        Assert.Equal(3L, response.CustomerId);
        Assert.Equal("Test Checklist", response.Title);
        Assert.Equal("test-checklist", response.Slug);
        Assert.Equal("Test description", response.Description);
        Assert.Equal(new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc), response.Deadline);
        Assert.Equal("test-token", response.AccessToken);
        Assert.Equal(ChecklistStatus.Active, response.Status);
        Assert.NotNull(response.Sections);
        Assert.Empty(response.Sections);
        Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc), response.CreatedDateUtc);
        Assert.Equal(new DateTime(2024, 1, 2, 12, 0, 0, DateTimeKind.Utc), response.LastUpdatedDateUtc);
    }

    [Fact]
    public void ChecklistResponse_WithCustomer_ShouldSerializeCorrectly()
    {
        // Arrange
        var customer = CreateCustomerResponse(3L);
        var response = new ChecklistResponse
        {
            Id = 1L,
            OwnerId = 2L,
            CustomerId = 3L,
            Customer = customer,
            Title = "Test Checklist",
            Slug = "test-checklist",
            Status = ChecklistStatus.Draft,
            Sections = new List<SectionResponse>(),
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Act
        var json = JsonConvert.SerializeObject(response);

        // Assert
        Assert.Contains("\"customer\":", json);
        Assert.Contains("\"firstName\":", json);
        Assert.Contains("\"lastName\":", json);
    }

    [Theory]
    [InlineData(ChecklistStatus.Draft)]
    [InlineData(ChecklistStatus.Active)]
    [InlineData(ChecklistStatus.Archived)]
    [InlineData(ChecklistStatus.Completed)]
    public void ChecklistResponse_WithDifferentPublishedStates_ShouldSetCorrectly(ChecklistStatus status)
    {
        // Arrange & Act
        var response = new ChecklistResponse
        {
            Id = 1L,
            OwnerId = 2L,
            CustomerId = 3L,
            Title = "Test Checklist",
            Slug = "test-checklist",
            Status = status,
            Sections = [],
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(status, response.Status);
    }

    [Fact]
    public void ChecklistResponse_WithSections_ShouldMaintainSectionOrder()
    {
        // Arrange
        var sections = new List<SectionResponse>
        {
            CreateSectionResponse(1),
            CreateSectionResponse(2),
            CreateSectionResponse(3)
        };

        var response = new ChecklistResponse
        {
            Id = 1L,
            OwnerId = 2L,
            CustomerId = 3L,
            Title = "Test Checklist",
            Slug = "test-checklist",
            Status = ChecklistStatus.Active,
            Sections = sections,
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Act & Assert
        Assert.Equal(3, response.Sections.Count);
        Assert.Equal(sections, response.Sections);
    }

    [Fact]
    public void ChecklistResponse_WithNullOptionalProperties_ShouldBeValid()
    {
        // Arrange & Act
        var response = new ChecklistResponse
        {
            Id = 1L,
            OwnerId = 2L,
            CustomerId = 3L,
            Customer = null,
            Title = "Test Checklist",
            Slug = "test-checklist",
            Description = null,
            Deadline = null,
            AccessToken = null,
            Status = ChecklistStatus.Draft,
            Sections = [],
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(1L, response.Id);
        Assert.Null(response.Customer);
        Assert.Null(response.Description);
        Assert.Null(response.Deadline);
        Assert.Null(response.AccessToken);
	}

	[Fact]
	public void ChecklistResponse_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(ChecklistResponse);

		// Act & Assert
		type.GetProperty(nameof(ChecklistResponse.Id))!.AssertPropertyName("id").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistResponse.OwnerId))!.AssertPropertyName("ownerId").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistResponse.CustomerId))!.AssertPropertyName("customerId").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistResponse.Customer))!.AssertPropertyName("customer").AssertRequired(Required.Default);
		type.GetProperty(nameof(ChecklistResponse.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistResponse.Slug))!.AssertPropertyName("slug").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistResponse.Description))!.AssertPropertyName("description").AssertRequired(Required.Default);
		type.GetProperty(nameof(ChecklistResponse.Deadline))!.AssertPropertyName("deadline").AssertRequired(Required.Default);
		type.GetProperty(nameof(ChecklistResponse.AccessToken))!.AssertPropertyName("accessToken").AssertRequired(Required.Default);
		type.GetProperty(nameof(ChecklistResponse.Status))!.AssertPropertyName("status").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistResponse.Sections))!.AssertPropertyName("sections").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistResponse.CreatedDateUtc))!.AssertPropertyName("createdDateUtc").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistResponse.LastUpdatedDateUtc))!.AssertPropertyName("lastUpdatedDateUtc").AssertRequired(Required.Always);
	}

	[Fact]
	public void ChecklistSectionResponse_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(ChecklistSectionResponse);

		// Act & Assert
		type.GetProperty(nameof(ChecklistSectionResponse.Id))!.AssertPropertyName("id").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistSectionResponse.ChecklistId))!.AssertPropertyName("checklistId").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistSectionResponse.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistSectionResponse.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistSectionResponse.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistSectionResponse.CreatedDateUtc))!.AssertPropertyName("createdDateUtc").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistSectionResponse.LastUpdatedDateUtc))!.AssertPropertyName("lastUpdatedDateUtc").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistSectionResponse.Components))!.AssertPropertyName("components").AssertRequired(Required.Always);
	}

	#region Helper Methods

	private static CustomerResponse CreateCustomerResponse(long id)
    {
        return new CustomerResponse
        {
            Id = id,
            Type = CustomerType.NaturalPerson,
            FirstName = "João",
            LastName = "Silva",
            Email = "joao@example.com",
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };
    }

    private static BriefingSectionResponse CreateSectionResponse(long id)
    {
        return new BriefingSectionResponse
        {
            Id = id,
            ChecklistId = 1L,
            Title = $"Section {id}",
            Type = "Briefing",
            Order = (int)id,
            ContentHtml = "<p>Test content</p>",
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow
        };
    }

    #endregion
}
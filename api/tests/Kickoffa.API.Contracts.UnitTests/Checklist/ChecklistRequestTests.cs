using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.UnitTests.Checklist;

public class ChecklistRequestTests
{
    [Fact]
    public void ChecklistRequest_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 1,
            Title = "Test Checklist",
            Description = "Test description",
            Deadline = DateTime.UtcNow.AddDays(7),
            Sections = new List<SectionRequest>
            {
                CreateValidBriefingSection(1, 1),
                CreateValidChecklistSection(2, 2)
            }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ChecklistRequest_WithZeroCustomerId_ShouldFailValidation()
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 0,
            Title = "Test Checklist",
            Sections = new List<SectionRequest> { CreateValidChecklistSection(1, 1) }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O cliente é obrigatório" && v.MemberNames.Contains("CustomerId"));
    }

    [Fact]
    public void ChecklistRequest_WithNegativeCustomerId_ShouldFailValidation()
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = -1,
            Title = "Test Checklist",
            Sections = new List<SectionRequest> { CreateValidChecklistSection(1, 1) }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O cliente é obrigatório" && v.MemberNames.Contains("CustomerId"));
    }

    [Fact]
    public void ChecklistRequest_WithEmptyTitle_ShouldFailValidation()
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 1,
            Title = "",
            Sections = new List<SectionRequest> { CreateValidChecklistSection(1, 1) }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título é obrigatório" && v.MemberNames.Contains("Title"));
    }

    [Fact]
    public void ChecklistRequest_WithWhitespaceTitle_ShouldFailValidation()
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 1,
            Title = "   ",
            Sections = new List<SectionRequest> { CreateValidChecklistSection(1, 1) }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título é obrigatório" && v.MemberNames.Contains("Title"));
    }

    [Fact]
    public void ChecklistRequest_WithTooShortTitle_ShouldFailValidation()
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 1,
            Title = "AB", // 2 characters
            Sections = new List<SectionRequest> { CreateValidChecklistSection(1, 1) }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título deve ter entre 3 e 200 caracteres" && v.MemberNames.Contains("Title"));
    }

    [Fact]
    public void ChecklistRequest_WithTooLongTitle_ShouldFailValidation()
    {
        // Arrange
        var longTitle = new string('A', 201); // 201 characters
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 1,
            Title = longTitle,
            Sections = new List<SectionRequest> { CreateValidChecklistSection(1, 1) }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título deve ter entre 3 e 200 caracteres" && v.MemberNames.Contains("Title"));
    }

    [Fact]
    public void ChecklistRequest_WithTooLongDescription_ShouldFailValidation()
    {
        // Arrange
        var longDescription = new string('A', 1001); // 1001 characters
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 1,
            Title = "Test Checklist",
            Description = longDescription,
            Sections = new List<SectionRequest> { CreateValidChecklistSection(1, 1) }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "A descrição deve ter no máximo 1000 caracteres" && v.MemberNames.Contains("Description"));
    }

    [Fact]
    public void ChecklistRequest_WithNullDescription_ShouldPassValidation()
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 1,
            Title = "Test Checklist",
            Description = null,
            Sections = new List<SectionRequest> { CreateValidChecklistSection(1, 1) }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ChecklistRequest_WithPastDeadline_ShouldFailValidation()
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 1,
            Title = "Test Checklist",
            Deadline = DateTime.UtcNow.AddDays(-1), // Past date
            Sections = new List<SectionRequest> { CreateValidChecklistSection(1, 1) }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "A data limite deve ser no futuro" && v.MemberNames.Contains("Deadline"));
    }

    [Fact]
    public void ChecklistRequest_WithNullDeadline_ShouldPassValidation()
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 1,
            Title = "Test Checklist",
            Deadline = null,
            Sections = new List<SectionRequest> { CreateValidChecklistSection(1, 1) }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ChecklistRequest_WithEmptySections_ShouldFailValidation()
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 1,
            Title = "Test Checklist",
            Sections = new List<SectionRequest>()
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O checklist deve ter pelo menos uma seção" && v.MemberNames.Contains("Sections"));
    }

    [Fact]
    public void ChecklistRequest_WithDuplicateOrders_ShouldFailValidation()
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 1,
            Title = "Test Checklist",
            Sections = new List<SectionRequest>
            {
                CreateValidChecklistSection(1, 1), // Same order
                CreateValidChecklistSection(2, 1)  // Same order
            }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "As seções devem ter ordens únicas" && v.MemberNames.Contains("Sections"));
    }

    [Fact]
    public void ChecklistRequest_WithoutChecklistSection_ShouldFailValidation()
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 1,
            Title = "Test Checklist",
            Sections = new List<SectionRequest>
            {
                CreateValidBriefingSection(1, 1) // Only briefing section
            }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O checklist deve ter pelo menos uma seção de checklist" && v.MemberNames.Contains("Sections"));
    }

    [Fact]
    public void ChecklistRequest_WithValidMixedSections_ShouldPassValidation()
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 1,
            Title = "Test Checklist",
            Sections = new List<SectionRequest>
            {
                CreateValidBriefingSection(1, 1),
                CreateValidChecklistSection(2, 2),
                CreateValidBriefingSection(3, 3)
            }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ChecklistRequest_WithMultipleValidationErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 0, // Invalid
            Title = "AB", // Too short
            Description = new string('A', 1001), // Too long
            Deadline = DateTime.UtcNow.AddDays(-1), // Past date
            Sections = new List<SectionRequest>() // Empty
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Equal(5, validationResults.Count);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O cliente é obrigatório");
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título deve ter entre 3 e 200 caracteres");
        Assert.Contains(validationResults, v => v.ErrorMessage == "A descrição deve ter no máximo 1000 caracteres");
        Assert.Contains(validationResults, v => v.ErrorMessage == "A data limite deve ser no futuro");
        Assert.Contains(validationResults, v => v.ErrorMessage == "O checklist deve ter pelo menos uma seção");
    }

    [Theory]
    [InlineData("ABC")] // 3 characters (minimum)
    [InlineData("Test Checklist Title")]
    [InlineData("A very long checklist title that is still within the 200 character limit and should pass validation without any issues")]
    public void ChecklistRequest_WithValidTitleLengths_ShouldPassValidation(string title)
    {
        // Arrange
        var request = new ChecklistRequest
        {
            Id = 1,
            CustomerId = 1,
            Title = title,
            Sections = new List<SectionRequest> { CreateValidChecklistSection(1, 1) }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
	}

	[Fact]
	public void ChecklistRequest_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(ChecklistRequest);

		// Act & Assert
		type.GetProperty(nameof(ChecklistRequest.Id))!.AssertPropertyName("id").AssertRequired(Required.Default);
		type.GetProperty(nameof(ChecklistRequest.CustomerId))!.AssertPropertyName("customerId").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistRequest.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistRequest.Description))!.AssertPropertyName("description").AssertRequired(Required.Default);
		type.GetProperty(nameof(ChecklistRequest.Deadline))!.AssertPropertyName("deadline").AssertRequired(Required.Default);
		type.GetProperty(nameof(ChecklistRequest.Sections))!.AssertPropertyName("sections").AssertRequired(Required.Always);
	}

	[Fact]
	public void ChecklistSectionRequest_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(ChecklistSectionRequest);

		// Act & Assert
		type.GetProperty(nameof(ChecklistSectionRequest.Id))!.AssertPropertyName("id").AssertRequired(Required.Default);
		type.GetProperty(nameof(ChecklistSectionRequest.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistSectionRequest.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistSectionRequest.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
		type.GetProperty(nameof(ChecklistSectionRequest.Components))!.AssertPropertyName("components").AssertRequired(Required.Always);
	}

	#region Helper Methods

	private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    private static BriefingSectionRequest CreateValidBriefingSection(long id, int order)
    {
        return new BriefingSectionRequest
        {
            Id = id,
            Title = $"Briefing Section {id}",
            Type = SectionTypeRequest.Briefing,
            Order = order,
            ContentHtml = "<p>Test content</p>"
        };
    }

    private static ChecklistSectionRequest CreateValidChecklistSection(long id, int order)
    {
        return new ChecklistSectionRequest
        {
            Id = id,
            Title = $"Checklist Section {id}",
            Type = SectionTypeRequest.Checklist,
            Order = order,
            Components = new List<ComponentRequest>()
        };
    }

    #endregion
}
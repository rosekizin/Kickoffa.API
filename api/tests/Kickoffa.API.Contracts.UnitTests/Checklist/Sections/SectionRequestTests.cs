using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.Contracts.Checklist.Components;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Sections;

public class SectionRequestTests
{
    [Fact]
    public void BriefingSectionRequest_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new BriefingSectionRequest
        {
            Id = 1,
            Title = "Test Briefing Section",
            Type = SectionTypeRequest.Briefing,
            Order = 1,
            ContentHtml = "<p>Test content</p>"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ChecklistSectionRequest_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new ChecklistSectionRequest
        {
            Id = 1,
            Title = "Test Checklist Section",
            Type = SectionTypeRequest.Checklist,
            Order = 1,
            Components = new List<ComponentRequest>
            {
                new CheckboxComponentRequest
                {
                    Id = 1,
                    Title = "Test Checkbox",
                    IsRequired = true,
                    Order = 1
                }
            }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void SectionRequest_WithEmptyTitle_ShouldFailValidation()
    {
        // Arrange
        var request = new BriefingSectionRequest
        {
            Id = 1,
            Title = "",
            Type = SectionTypeRequest.Briefing,
            Order = 1,
            ContentHtml = "<p>Test content</p>"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título da seção é obrigatório" && v.MemberNames.Contains("Title"));
    }

    [Fact]
    public void SectionRequest_WithTooShortTitle_ShouldFailValidation()
    {
        // Arrange
        var request = new BriefingSectionRequest
        {
            Id = 1,
            Title = "A", // 1 character
            Type = SectionTypeRequest.Briefing,
            Order = 1,
            ContentHtml = "<p>Test content</p>"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título da seção deve ter entre 2 e 200 caracteres" && v.MemberNames.Contains("Title"));
    }

    [Fact]
    public void SectionRequest_WithTooLongTitle_ShouldFailValidation()
    {
        // Arrange
        var longTitle = new string('A', 201); // 201 characters
        var request = new BriefingSectionRequest
        {
            Id = 1,
            Title = longTitle,
            Type = SectionTypeRequest.Briefing,
            Order = 1,
            ContentHtml = "<p>Test content</p>"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título da seção deve ter entre 2 e 200 caracteres" && v.MemberNames.Contains("Title"));
    }

    [Fact]
    public void SectionRequest_WithZeroOrder_ShouldFailValidation()
    {
        // Arrange
        var request = new BriefingSectionRequest
        {
            Id = 1,
            Title = "Test Section",
            Type = SectionTypeRequest.Briefing,
            Order = 0,
            ContentHtml = "<p>Test content</p>"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "A ordem da seção deve ser maior que zero" && v.MemberNames.Contains("Order"));
    }

    [Fact]
    public void BriefingSectionRequest_WithEmptyContentHtml_ShouldFailValidation()
    {
        // Arrange
        var request = new BriefingSectionRequest
        {
            Id = 1,
            Title = "Test Briefing Section",
            Type = SectionTypeRequest.Briefing,
            Order = 1,
            ContentHtml = ""
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O conteúdo HTML é obrigatório" && v.MemberNames.Contains("ContentHtml"));
    }

    [Fact]
    public void BriefingSectionRequest_WithNullContentHtml_ShouldFailValidation()
    {
        // Arrange
        var request = new BriefingSectionRequest
        {
            Id = 1,
            Title = "Test Briefing Section",
            Type = SectionTypeRequest.Briefing,
            Order = 1,
            ContentHtml = null!
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O conteúdo HTML é obrigatório" && v.MemberNames.Contains("ContentHtml"));
    }

    [Fact]
    public void ChecklistSectionRequest_WithEmptyComponents_ShouldPassValidation()
    {
        // Arrange
        var request = new ChecklistSectionRequest
        {
            Id = 1,
            Title = "Test Checklist Section",
            Type = SectionTypeRequest.Checklist,
            Order = 1,
            Components = new List<ComponentRequest>()
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ChecklistSectionRequest_WithNullComponents_ShouldPassValidation()
    {
        // Arrange
        var request = new ChecklistSectionRequest
        {
            Id = 1,
            Title = "Test Checklist Section",
            Type = SectionTypeRequest.Checklist,
            Order = 1,
            Components = null!
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ChecklistSectionRequest_WithMultipleComponents_ShouldPassValidation()
    {
        // Arrange
        var request = new ChecklistSectionRequest
        {
            Id = 1,
            Title = "Test Checklist Section",
            Type = SectionTypeRequest.Checklist,
            Order = 1,
            Components = new List<ComponentRequest>
            {
                new CheckboxComponentRequest
                {
                    Id = 1,
                    Title = "Checkbox 1",
                    IsRequired = true,
                    Order = 1
                },
                new TextComponentRequest
                {
                    Id = 2,
                    Title = "Text Input",
                    IsRequired = false,
                    Order = 2
                }
            }
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData(SectionTypeRequest.Briefing)]
    [InlineData(SectionTypeRequest.Checklist)]
    public void SectionRequest_WithValidSectionTypes_ShouldPassValidation(SectionTypeRequest sectionType)
    {
        // Arrange
        SectionRequest request = sectionType switch
        {
            SectionTypeRequest.Briefing => new BriefingSectionRequest
            {
                Id = 1,
                Title = "Test Section",
                Type = sectionType,
                Order = 1,
                ContentHtml = "<p>Test</p>"
            },
            SectionTypeRequest.Checklist => new ChecklistSectionRequest
            {
                Id = 1,
                Title = "Test Section",
                Type = sectionType,
                Order = 1,
                Components = new List<ComponentRequest>()
            },
            _ => throw new ArgumentException("Invalid section type")
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void BriefingSectionRequest_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var request1 = new BriefingSectionRequest
        {
            Id = 1,
            Title = "Test Section",
            Type = SectionTypeRequest.Briefing,
            Order = 1,
            ContentHtml = "<p>Test content</p>"
        };

        var request2 = new BriefingSectionRequest
        {
            Id = 1,
            Title = "Test Section",
            Type = SectionTypeRequest.Briefing,
            Order = 1,
            ContentHtml = "<p>Test content</p>"
        };

        // Act & Assert
        Assert.Equal(request1, request2);
        Assert.True(request1 == request2);
        Assert.False(request1 != request2);
    }

    [Fact]
    public void ChecklistSectionRequest_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var components = new List<ComponentRequest>();
        
        var request1 = new ChecklistSectionRequest
        {
            Id = 1,
            Title = "Test Section",
            Type = SectionTypeRequest.Checklist,
            Order = 1,
            Components = components
        };

        var request2 = new ChecklistSectionRequest
        {
            Id = 1,
            Title = "Test Section",
            Type = SectionTypeRequest.Checklist,
            Order = 1,
            Components = components
        };

        // Act & Assert
        Assert.Equal(request1, request2);
        Assert.True(request1 == request2);
        Assert.False(request1 != request2);
    }

    [Fact]
    public void SectionRequest_ShouldInheritCorrectly()
    {
        // Arrange
        var briefingSection = new BriefingSectionRequest
        {
            Id = 1,
            Title = "Test Section",
            Type = SectionTypeRequest.Briefing,
            Order = 1,
            ContentHtml = "<p>Test</p>"
        };

        var checklistSection = new ChecklistSectionRequest
        {
            Id = 1,
            Title = "Test Section",
            Type = SectionTypeRequest.Checklist,
            Order = 1,
            Components = new List<ComponentRequest>()
        };

        // Assert
        Assert.IsAssignableFrom<SectionRequest>(briefingSection);
        Assert.IsAssignableFrom<SectionRequest>(checklistSection);
    }

    [Fact]
    public void SectionRequest_WithMultipleValidationErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var request = new BriefingSectionRequest
        {
            Id = 1,
            Title = "A", // Too short
            Type = SectionTypeRequest.Briefing,
            Order = 0, // Invalid
            ContentHtml = "" // Empty
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Equal(3, validationResults.Count);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título da seção deve ter entre 2 e 200 caracteres");
        Assert.Contains(validationResults, v => v.ErrorMessage == "A ordem da seção deve ser maior que zero");
        Assert.Contains(validationResults, v => v.ErrorMessage == "O conteúdo HTML é obrigatório");
    }

    [Theory]
    [InlineData("AB")] // 2 characters (minimum)
    [InlineData("Test Section Title")]
    [InlineData("A very long section title that is still within the 200 character limit and should pass validation")]
    public void SectionRequest_WithValidTitleLengths_ShouldPassValidation(string title)
    {
        // Arrange
        var request = new BriefingSectionRequest
        {
            Id = 1,
            Title = title,
            Type = SectionTypeRequest.Briefing,
            Order = 1,
            ContentHtml = "<p>Test content</p>"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
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

using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components.Request;

public class TextComponentRequestTests
{
    [Fact]
    public void TextComponentRequest_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new TextComponentRequest
        {
            Id = 1,
            Title = "Test Text Component",
            Description = "Test description",
            IsRequired = true,
            Order = 1,
            Placeholder = "Enter text here",
            MaxLength = 100
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void TextComponentRequest_WithMinimalData_ShouldPassValidation()
    {
        // Arrange
        var request = new TextComponentRequest
        {
            Id = 1,
            Title = "TX", // Minimum length
            IsRequired = false,
            Order = 1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void TextComponentRequest_WithNullOptionalProperties_ShouldPassValidation()
    {
        // Arrange
        var request = new TextComponentRequest
        {
            Id = 1,
            Title = "Test Text Component",
            Description = null,
            IsRequired = true,
            Order = 1,
            Placeholder = null,
            MaxLength = null
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void TextComponentRequest_WithEmptyPlaceholder_ShouldPassValidation()
    {
        // Arrange
        var request = new TextComponentRequest
        {
            Id = 1,
            Title = "Test Text Component",
            IsRequired = true,
            Order = 1,
            Placeholder = ""
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void TextComponentRequest_WithZeroMaxLength_ShouldFailValidation()
    {
        // Arrange
        var request = new TextComponentRequest
        {
            Id = 1,
            Title = "Test Text Component",
            IsRequired = true,
            Order = 1,
            MaxLength = 0
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O comprimento máximo deve ser maior que zero" && v.MemberNames.Contains("MaxLength"));
    }

    [Fact]
    public void TextComponentRequest_WithNegativeMaxLength_ShouldFailValidation()
    {
        // Arrange
        var request = new TextComponentRequest
        {
            Id = 1,
            Title = "Test Text Component",
            IsRequired = true,
            Order = 1,
            MaxLength = -1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O comprimento máximo deve ser maior que zero" && v.MemberNames.Contains("MaxLength"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(int.MaxValue)]
    public void TextComponentRequest_WithValidMaxLength_ShouldPassValidation(int maxLength)
    {
        // Arrange
        var request = new TextComponentRequest
        {
            Id = 1,
            Title = "Test Text Component",
            IsRequired = true,
            Order = 1,
            MaxLength = maxLength
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
        Assert.Equal(maxLength, request.MaxLength);
    }

    [Theory]
    [InlineData("Enter your name")]
    [InlineData("Type here...")]
    [InlineData("")]
    [InlineData("A very long placeholder text that provides detailed instructions to the user")]
    public void TextComponentRequest_WithValidPlaceholders_ShouldPassValidation(string placeholder)
    {
        // Arrange
        var request = new TextComponentRequest
        {
            Id = 1,
            Title = "Test Text Component",
            IsRequired = true,
            Order = 1,
            Placeholder = placeholder
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
        Assert.Equal(placeholder, request.Placeholder);
    }

    [Fact]
    public void TextComponentRequest_InheritsFromComponentRequest_ShouldValidateBaseProperties()
    {
        // Arrange
        var request = new TextComponentRequest
        {
            Id = 1,
            Title = "", // Invalid - empty title
            IsRequired = true,
            Order = 0 // Invalid - zero order
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Equal(2, validationResults.Count);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título do componente é obrigatório" && v.MemberNames.Contains("Title"));
        Assert.Contains(validationResults, v => v.ErrorMessage == "A ordem do componente deve ser maior que zero" && v.MemberNames.Contains("Order"));
    }

    [Fact]
    public void TextComponentRequest_WithMultipleValidationErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var request = new TextComponentRequest
        {
            Id = 1,
            Title = "A", // Too short
            Description = new string('A', 501), // Too long
            IsRequired = true,
            Order = -1, // Invalid
            MaxLength = 0 // Invalid
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Equal(4, validationResults.Count);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título do componente deve ter entre 2 e 200 caracteres");
        Assert.Contains(validationResults, v => v.ErrorMessage == "A descrição do componente deve ter no máximo 500 caracteres");
        Assert.Contains(validationResults, v => v.ErrorMessage == "A ordem do componente deve ser maior que zero");
        Assert.Contains(validationResults, v => v.ErrorMessage == "O comprimento máximo deve ser maior que zero");
    }

    [Fact]
    public void TextComponentRequest_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var request1 = new TextComponentRequest
        {
            Id = 1,
            Title = "Test Text Component",
            Description = "Test description",
            IsRequired = true,
            Order = 1,
            Placeholder = "Enter text",
            MaxLength = 100
        };

        var request2 = new TextComponentRequest
        {
            Id = 1,
            Title = "Test Text Component",
            Description = "Test description",
            IsRequired = true,
            Order = 1,
            Placeholder = "Enter text",
            MaxLength = 100
        };

        // Act & Assert
        Assert.Equal(request1, request2);
        Assert.True(request1 == request2);
        Assert.False(request1 != request2);
    }

    [Fact]
    public void TextComponentRequest_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var request1 = new TextComponentRequest
        {
            Id = 1,
            Title = "Test Text Component",
            IsRequired = true,
            Order = 1,
            MaxLength = 100
        };

        var request2 = new TextComponentRequest
        {
            Id = 1,
            Title = "Test Text Component",
            IsRequired = true,
            Order = 1,
            MaxLength = 200 // Different MaxLength
        };

        // Act & Assert
        Assert.NotEqual(request1, request2);
        Assert.False(request1 == request2);
        Assert.True(request1 != request2);
    }

    [Fact]
    public void TextComponentRequest_ShouldInheritFromComponentRequest()
    {
        // Arrange
        var request = new TextComponentRequest
        {
            Id = 1,
            Title = "Test Text Component",
            IsRequired = true,
            Order = 1
        };

        // Assert
        Assert.IsAssignableFrom<ComponentRequest>(request);
    }

    [Fact]
    public void TextComponentRequest_ShouldBeSealed()
    {
        // Assert
        var type = typeof(TextComponentRequest);
        Assert.True(type.IsSealed);
    }

    [Fact]
    public void TextComponentRequest_ShouldOverrideValidateMethod()
    {
        // Arrange
        var type = typeof(TextComponentRequest);
        var validateMethod = type.GetMethod("Validate");

        // Assert
        Assert.NotNull(validateMethod);
        Assert.True(validateMethod.IsVirtual);
        Assert.Equal(typeof(TextComponentRequest), validateMethod.DeclaringType);
    }

    [Fact]
    public void TextComponentRequest_WithMaximumValidValues_ShouldPassValidation()
    {
        // Arrange
        var maxTitle = new string('A', 200); // 200 characters (maximum)
        var maxDescription = new string('B', 500); // 500 characters (maximum)
        var longPlaceholder = new string('C', 1000); // Long placeholder
        
        var request = new TextComponentRequest
        {
            Id = long.MaxValue,
            Title = maxTitle,
            Description = maxDescription,
            IsRequired = true,
            Order = int.MaxValue,
            Placeholder = longPlaceholder,
            MaxLength = int.MaxValue
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TextComponentRequest_WithDifferentIsRequiredValues_ShouldSetCorrectly(bool isRequired)
    {
        // Arrange
        var request = new TextComponentRequest
        {
            Id = 1,
            Title = "Test Text Component",
            IsRequired = isRequired,
            Order = 1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
        Assert.Equal(isRequired, request.IsRequired);
    }

    [Fact]
    public void TextComponentRequest_WithBothPlaceholderAndMaxLength_ShouldPassValidation()
    {
        // Arrange
        var request = new TextComponentRequest
        {
            Id = 1,
            Title = "Test Text Component",
            IsRequired = true,
            Order = 1,
            Placeholder = "Enter up to 50 characters",
            MaxLength = 50
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
        Assert.Equal("Enter up to 50 characters", request.Placeholder);
        Assert.Equal(50, request.MaxLength);
    }

    [Fact]
    public void TextComponentRequest_CallsBaseValidation_ShouldIncludeBaseValidationResults()
    {
        // Arrange
        var request = new TextComponentRequest
        {
            Id = 1,
            Title = "A", // Invalid base validation
            IsRequired = true,
            Order = 1,
            MaxLength = -1 // Invalid specific validation
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Equal(2, validationResults.Count);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título do componente deve ter entre 2 e 200 caracteres"); // Base validation
        Assert.Contains(validationResults, v => v.ErrorMessage == "O comprimento máximo deve ser maior que zero"); // Specific validation
	}

	[Fact]
	public void TextComponentRequest_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(TextComponentRequest);

		// Act & Assert
		type.GetProperty(nameof(TextComponentRequest.Id))!.AssertPropertyName("id").AssertRequired(Required.Default);
		type.GetProperty(nameof(TextComponentRequest.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
		type.GetProperty(nameof(TextComponentRequest.Description))!.AssertPropertyName("description").AssertRequired(Required.Default);
		type.GetProperty(nameof(TextComponentRequest.IsRequired))!.AssertPropertyName("isRequired").AssertRequired(Required.Always);
		type.GetProperty(nameof(TextComponentRequest.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
		type.GetProperty(nameof(TextComponentRequest.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
		type.GetProperty(nameof(TextComponentRequest.Placeholder))!.AssertPropertyName("placeholder").AssertRequired(Required.Default);
		type.GetProperty(nameof(TextComponentRequest.MaxLength))!.AssertPropertyName("maxLength").AssertRequired(Required.Default);
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

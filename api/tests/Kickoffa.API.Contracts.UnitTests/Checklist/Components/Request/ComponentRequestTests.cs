using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components.Request;

public class ComponentRequestTests
{
    // Since ComponentRequest is abstract, we'll use a concrete implementation for testing
    private sealed record TestComponentRequest : ComponentRequest
    {
        public TestComponentRequest()
        {
            Type = ComponentTypeRequest.Checkbox;
        }
    }

    [Fact]
    public void ComponentRequest_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = "Test Component",
            Description = "Test description",
            IsRequired = true,
            Order = 1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ComponentRequest_WithEmptyTitle_ShouldFailValidation()
    {
        // Arrange
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = "",
            IsRequired = true,
            Order = 1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título do componente é obrigatório" && v.MemberNames.Contains("Title"));
    }

    [Fact]
    public void ComponentRequest_WithWhitespaceTitle_ShouldFailValidation()
    {
        // Arrange
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = "   ",
            IsRequired = true,
            Order = 1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título do componente é obrigatório" && v.MemberNames.Contains("Title"));
    }

    [Fact]
    public void ComponentRequest_WithTooShortTitle_ShouldFailValidation()
    {
        // Arrange
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = "A", // 1 character
            IsRequired = true,
            Order = 1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título do componente deve ter entre 2 e 200 caracteres" && v.MemberNames.Contains("Title"));
    }

    [Fact]
    public void ComponentRequest_WithTooLongTitle_ShouldFailValidation()
    {
        // Arrange
        var longTitle = new string('A', 201); // 201 characters
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = longTitle,
            IsRequired = true,
            Order = 1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título do componente deve ter entre 2 e 200 caracteres" && v.MemberNames.Contains("Title"));
    }

    [Fact]
    public void ComponentRequest_WithTooLongDescription_ShouldFailValidation()
    {
        // Arrange
        var longDescription = new string('A', 501); // 501 characters
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = "Test Component",
            Description = longDescription,
            IsRequired = true,
            Order = 1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "A descrição do componente deve ter no máximo 500 caracteres" && v.MemberNames.Contains("Description"));
    }

    [Fact]
    public void ComponentRequest_WithNullDescription_ShouldPassValidation()
    {
        // Arrange
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = "Test Component",
            Description = null,
            IsRequired = true,
            Order = 1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ComponentRequest_WithZeroOrder_ShouldFailValidation()
    {
        // Arrange
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = "Test Component",
            IsRequired = true,
            Order = 0
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "A ordem do componente deve ser maior que zero" && v.MemberNames.Contains("Order"));
    }

    [Fact]
    public void ComponentRequest_WithNegativeOrder_ShouldFailValidation()
    {
        // Arrange
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = "Test Component",
            IsRequired = true,
            Order = -1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "A ordem do componente deve ser maior que zero" && v.MemberNames.Contains("Order"));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ComponentRequest_WithDifferentIsRequiredValues_ShouldSetCorrectly(bool isRequired)
    {
        // Arrange
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = "Test Component",
            IsRequired = isRequired,
            Order = 1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
        Assert.Equal(isRequired, request.IsRequired);
    }

    [Theory]
    [InlineData("AB")] // 2 characters (minimum)
    [InlineData("Test Component Title")]
    [InlineData("A very long component title that is still within the 200 character limit and should pass validation without any issues whatsoever")]
    public void ComponentRequest_WithValidTitleLengths_ShouldPassValidation(string title)
    {
        // Arrange
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = title,
            IsRequired = true,
            Order = 1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ComponentRequest_WithValidDescriptionLength_ShouldPassValidation()
    {
        // Arrange
        var description = new string('A', 500); // 500 characters (maximum)
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = "Test Component",
            Description = description,
            IsRequired = true,
            Order = 1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ComponentRequest_WithMultipleValidationErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = "A", // Too short
            Description = new string('A', 501), // Too long
            IsRequired = true,
            Order = 0 // Invalid
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Equal(3, validationResults.Count);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título do componente deve ter entre 2 e 200 caracteres");
        Assert.Contains(validationResults, v => v.ErrorMessage == "A descrição do componente deve ter no máximo 500 caracteres");
        Assert.Contains(validationResults, v => v.ErrorMessage == "A ordem do componente deve ser maior que zero");
    }

    [Theory]
    [InlineData(ComponentTypeRequest.Checkbox)]
    [InlineData(ComponentTypeRequest.Text)]
    [InlineData(ComponentTypeRequest.Upload)]
    [InlineData(ComponentTypeRequest.Signature)]
    [InlineData(ComponentTypeRequest.Confirmation)]
    public void ComponentRequest_WithValidComponentTypes_ShouldPassValidation(ComponentTypeRequest componentType)
    {
        // Arrange
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = "Test Component",
            IsRequired = true,
            Order = 1
        };
        
        // Use reflection to set the Type property since it's init-only
        var typeProperty = typeof(ComponentRequest).GetProperty(nameof(ComponentRequest.Type));
        typeProperty?.SetValue(request, componentType);

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ComponentRequest_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var request1 = new TestComponentRequest
        {
            Id = 1,
            Title = "Test Component",
            Description = "Test description",
            IsRequired = true,
            Order = 1
        };

        var request2 = new TestComponentRequest
        {
            Id = 1,
            Title = "Test Component",
            Description = "Test description",
            IsRequired = true,
            Order = 1
        };

        // Act & Assert
        Assert.Equal(request1, request2);
        Assert.True(request1 == request2);
        Assert.False(request1 != request2);
    }

    [Fact]
    public void ComponentRequest_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var request1 = new TestComponentRequest
        {
            Id = 1,
            Title = "Test Component",
            IsRequired = true,
            Order = 1
        };

        var request2 = new TestComponentRequest
        {
            Id = 2, // Different ID
            Title = "Test Component",
            IsRequired = true,
            Order = 1
        };

        // Act & Assert
        Assert.NotEqual(request1, request2);
        Assert.False(request1 == request2);
        Assert.True(request1 != request2);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void ComponentRequest_WithValidOrders_ShouldPassValidation(int order)
    {
        // Arrange
        var request = new TestComponentRequest
        {
            Id = 1,
            Title = "Test Component",
            IsRequired = true,
            Order = order
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
	}

	[Fact]
	public void ComponentRequest_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(ComponentRequest);

		// Act & Assert
		type.GetProperty(nameof(ComponentRequest.Id))!.AssertPropertyName("id").AssertRequired(Required.Default);
		type.GetProperty(nameof(ComponentRequest.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
		type.GetProperty(nameof(ComponentRequest.Description))!.AssertPropertyName("description").AssertRequired(Required.Default);
		type.GetProperty(nameof(ComponentRequest.IsRequired))!.AssertPropertyName("isRequired").AssertRequired(Required.Always);
		type.GetProperty(nameof(ComponentRequest.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
		type.GetProperty(nameof(ComponentRequest.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
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

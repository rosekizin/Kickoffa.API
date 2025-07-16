using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components.Request;

public class ConfirmationComponentRequestTests
{
    [Fact]
    public void ConfirmationComponentRequest_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "Test Confirmation Component",
            Description = "Test description",
            IsRequired = true,
            Order = 1,
            ConfirmationText = "I agree to the terms and conditions"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ConfirmationComponentRequest_WithMinimalData_ShouldPassValidation()
    {
        // Arrange
        var request = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "CF", // Minimum length
            IsRequired = false,
            Order = 1,
            ConfirmationText = "I confirm"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ConfirmationComponentRequest_WithEmptyConfirmationText_ShouldFailValidation()
    {
        // Arrange
        var request = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "Test Confirmation Component",
            IsRequired = true,
            Order = 1,
            ConfirmationText = ""
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Componentes de confirmação devem ter texto de confirmação" && v.MemberNames.Contains("ConfirmationText"));
    }

    [Fact]
    public void ConfirmationComponentRequest_WithWhitespaceConfirmationText_ShouldFailValidation()
    {
        // Arrange
        var request = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "Test Confirmation Component",
            IsRequired = true,
            Order = 1,
            ConfirmationText = "   "
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Componentes de confirmação devem ter texto de confirmação" && v.MemberNames.Contains("ConfirmationText"));
    }

    [Theory]
    [InlineData("I agree")]
    [InlineData("I confirm that I have read and understood the terms")]
    [InlineData("Yes, I accept")]
    [InlineData("I acknowledge receipt of this document")]
    public void ConfirmationComponentRequest_WithValidConfirmationTexts_ShouldPassValidation(string confirmationText)
    {
        // Arrange
        var request = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "Test Confirmation Component",
            IsRequired = true,
            Order = 1,
            ConfirmationText = confirmationText
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
        Assert.Equal(confirmationText, request.ConfirmationText);
    }

    [Fact]
    public void ConfirmationComponentRequest_InheritsFromComponentRequest_ShouldValidateBaseProperties()
    {
        // Arrange
        var request = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "", // Invalid - empty title
            IsRequired = true,
            Order = 0, // Invalid - zero order
            ConfirmationText = "I agree"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Equal(2, validationResults.Count);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título do componente é obrigatório" && v.MemberNames.Contains("Title"));
        Assert.Contains(validationResults, v => v.ErrorMessage == "A ordem do componente deve ser maior que zero" && v.MemberNames.Contains("Order"));
    }

    [Fact]
    public void ConfirmationComponentRequest_WithMultipleValidationErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var request = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "A", // Too short
            Description = new string('A', 501), // Too long
            IsRequired = true,
            Order = -1, // Invalid
            ConfirmationText = "" // Empty
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Equal(4, validationResults.Count);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título do componente deve ter entre 2 e 200 caracteres");
        Assert.Contains(validationResults, v => v.ErrorMessage == "A descrição do componente deve ter no máximo 500 caracteres");
        Assert.Contains(validationResults, v => v.ErrorMessage == "A ordem do componente deve ser maior que zero");
        Assert.Contains(validationResults, v => v.ErrorMessage == "Componentes de confirmação devem ter texto de confirmação");
    }

    [Fact]
    public void ConfirmationComponentRequest_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var request1 = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "Test Confirmation Component",
            Description = "Test description",
            IsRequired = true,
            Order = 1,
            ConfirmationText = "I agree to the terms"
        };

        var request2 = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "Test Confirmation Component",
            Description = "Test description",
            IsRequired = true,
            Order = 1,
            ConfirmationText = "I agree to the terms"
        };

        // Act & Assert
        Assert.Equal(request1, request2);
        Assert.True(request1 == request2);
        Assert.False(request1 != request2);
    }

    [Fact]
    public void ConfirmationComponentRequest_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var request1 = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "Test Confirmation Component",
            IsRequired = true,
            Order = 1,
            ConfirmationText = "I agree"
        };

        var request2 = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "Test Confirmation Component",
            IsRequired = true,
            Order = 1,
            ConfirmationText = "I disagree" // Different confirmation text
        };

        // Act & Assert
        Assert.NotEqual(request1, request2);
        Assert.False(request1 == request2);
        Assert.True(request1 != request2);
    }

    [Fact]
    public void ConfirmationComponentRequest_ShouldInheritFromComponentRequest()
    {
        // Arrange
        var request = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "Test Confirmation Component",
            IsRequired = true,
            Order = 1,
            ConfirmationText = "I agree"
        };

        // Assert
        Assert.IsAssignableFrom<ComponentRequest>(request);
    }

    [Fact]
    public void ConfirmationComponentRequest_ShouldBeSealed()
    {
        // Assert
        var type = typeof(ConfirmationComponentRequest);
        Assert.True(type.IsSealed);
    }

    [Fact]
    public void ConfirmationComponentRequest_ShouldOverrideValidateMethod()
    {
        // Arrange
        var type = typeof(ConfirmationComponentRequest);
        var validateMethod = type.GetMethod("Validate");

        // Assert
        Assert.NotNull(validateMethod);
        Assert.True(validateMethod.IsVirtual);
        Assert.Equal(typeof(ConfirmationComponentRequest), validateMethod.DeclaringType);
    }

    [Fact]
    public void ConfirmationComponentRequest_WithLongConfirmationText_ShouldPassValidation()
    {
        // Arrange
        var longConfirmationText = new string('A', 1000); // Very long text
        var request = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "Test Confirmation Component",
            IsRequired = true,
            Order = 1,
            ConfirmationText = longConfirmationText
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
        Assert.Equal(longConfirmationText, request.ConfirmationText);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConfirmationComponentRequest_WithDifferentIsRequiredValues_ShouldSetCorrectly(bool isRequired)
    {
        // Arrange
        var request = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "Test Confirmation Component",
            IsRequired = isRequired,
            Order = 1,
            ConfirmationText = "I agree"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
        Assert.Equal(isRequired, request.IsRequired);
    }

    [Fact]
    public void ConfirmationComponentRequest_WithMaximumValidValues_ShouldPassValidation()
    {
        // Arrange
        var maxTitle = new string('A', 200); // 200 characters (maximum)
        var maxDescription = new string('B', 500); // 500 characters (maximum)
        var confirmationText = "I agree to all terms and conditions";
        
        var request = new ConfirmationComponentRequest
        {
            Id = long.MaxValue,
            Title = maxTitle,
            Description = maxDescription,
            IsRequired = true,
            Order = int.MaxValue,
            ConfirmationText = confirmationText
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ConfirmationComponentRequest_CallsBaseValidation_ShouldIncludeBaseValidationResults()
    {
        // Arrange
        var request = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "A", // Invalid base validation
            IsRequired = true,
            Order = 1,
            ConfirmationText = "" // Invalid specific validation
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Equal(2, validationResults.Count);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O título do componente deve ter entre 2 e 200 caracteres"); // Base validation
        Assert.Contains(validationResults, v => v.ErrorMessage == "Componentes de confirmação devem ter texto de confirmação"); // Specific validation
    }

    [Theory]
    [InlineData(0)]
    [InlineData(long.MaxValue)]
    [InlineData(-1)]
    [InlineData(12345)]
    public void ConfirmationComponentRequest_WithDifferentIds_ShouldSetCorrectly(long id)
    {
        // Arrange
        var request = new ConfirmationComponentRequest
        {
            Id = id,
            Title = "Test Confirmation Component",
            IsRequired = true,
            Order = 1,
            ConfirmationText = "I agree"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
        Assert.Equal(id, request.Id);
    }

    [Fact]
    public void ConfirmationComponentRequest_WithSpecialCharactersInConfirmationText_ShouldPassValidation()
    {
        // Arrange
        var confirmationText = "I agree to the terms & conditions (including special chars: @#$%^&*())";
        var request = new ConfirmationComponentRequest
        {
            Id = 1,
            Title = "Test Confirmation Component",
            IsRequired = true,
            Order = 1,
            ConfirmationText = confirmationText
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
        Assert.Equal(confirmationText, request.ConfirmationText);
	}

	[Fact]
	public void ConfirmationComponentRequest_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(ConfirmationComponentRequest);

		// Act & Assert
		type.GetProperty(nameof(ConfirmationComponentRequest.Id))!.AssertPropertyName("id").AssertRequired(Required.Default);
		type.GetProperty(nameof(ConfirmationComponentRequest.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
		type.GetProperty(nameof(ConfirmationComponentRequest.Description))!.AssertPropertyName("description").AssertRequired(Required.Default);
		type.GetProperty(nameof(ConfirmationComponentRequest.IsRequired))!.AssertPropertyName("isRequired").AssertRequired(Required.Always);
		type.GetProperty(nameof(ConfirmationComponentRequest.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
		type.GetProperty(nameof(ConfirmationComponentRequest.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
		type.GetProperty(nameof(ConfirmationComponentRequest.ConfirmationText))!.AssertPropertyName("confirmationText").AssertRequired(Required.Always);
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
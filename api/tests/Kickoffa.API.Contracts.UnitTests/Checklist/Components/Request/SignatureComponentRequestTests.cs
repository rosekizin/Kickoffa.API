using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components.Request;

public class SignatureComponentRequestTests
{
    [Fact]
    public void SignatureComponentRequest_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new SignatureComponentRequest
        {
            Id = 1,
            Title = "Test Signature Component",
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
    public void SignatureComponentRequest_WithMinimalData_ShouldPassValidation()
    {
        // Arrange
        var request = new SignatureComponentRequest
        {
            Id = 1,
            Title = "SG", // Minimum length
            IsRequired = false,
            Order = 1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void SignatureComponentRequest_WithNullDescription_ShouldPassValidation()
    {
        // Arrange
        var request = new SignatureComponentRequest
        {
            Id = 1,
            Title = "Test Signature Component",
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
    public void SignatureComponentRequest_InheritsFromComponentRequest_ShouldValidateBaseProperties()
    {
        // Arrange
        var request = new SignatureComponentRequest
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
    public void SignatureComponentRequest_WithTooLongTitle_ShouldFailValidation()
    {
        // Arrange
        var longTitle = new string('A', 201); // 201 characters
        var request = new SignatureComponentRequest
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
    public void SignatureComponentRequest_WithTooLongDescription_ShouldFailValidation()
    {
        // Arrange
        var longDescription = new string('A', 501); // 501 characters
        var request = new SignatureComponentRequest
        {
            Id = 1,
            Title = "Test Signature Component",
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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SignatureComponentRequest_WithDifferentIsRequiredValues_ShouldSetCorrectly(bool isRequired)
    {
        // Arrange
        var request = new SignatureComponentRequest
        {
            Id = 1,
            Title = "Test Signature Component",
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
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void SignatureComponentRequest_WithValidOrders_ShouldPassValidation(int order)
    {
        // Arrange
        var request = new SignatureComponentRequest
        {
            Id = 1,
            Title = "Test Signature Component",
            IsRequired = true,
            Order = order
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
        Assert.Equal(order, request.Order);
    }

    [Fact]
    public void SignatureComponentRequest_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var request1 = new SignatureComponentRequest
        {
            Id = 1,
            Title = "Test Signature Component",
            Description = "Test description",
            IsRequired = true,
            Order = 1
        };

        var request2 = new SignatureComponentRequest
        {
            Id = 1,
            Title = "Test Signature Component",
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
    public void SignatureComponentRequest_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var request1 = new SignatureComponentRequest
        {
            Id = 1,
            Title = "Test Signature Component",
            IsRequired = true,
            Order = 1
        };

        var request2 = new SignatureComponentRequest
        {
            Id = 1,
            Title = "Different Signature Component", // Different title
            IsRequired = true,
            Order = 1
        };

        // Act & Assert
        Assert.NotEqual(request1, request2);
        Assert.False(request1 == request2);
        Assert.True(request1 != request2);
    }

    [Fact]
    public void SignatureComponentRequest_ShouldInheritFromComponentRequest()
    {
        // Arrange
        var request = new SignatureComponentRequest
        {
            Id = 1,
            Title = "Test Signature Component",
            IsRequired = true,
            Order = 1
        };

        // Assert
        Assert.IsAssignableFrom<ComponentRequest>(request);
    }

    [Fact]
    public void SignatureComponentRequest_ShouldBeSealed()
    {
        // Assert
        var type = typeof(SignatureComponentRequest);
        Assert.True(type.IsSealed);
    }

    [Fact]
    public void SignatureComponentRequest_ShouldBeRecord()
    {
        // Assert
        var type = typeof(SignatureComponentRequest);
        Assert.True(type.IsClass);
        
        // Records have compiler-generated methods
        var equalsMethod = type.GetMethod("Equals", new[] { typeof(SignatureComponentRequest) });
        Assert.NotNull(equalsMethod);
        
        var getHashCodeMethod = type.GetMethod("GetHashCode", Type.EmptyTypes);
        Assert.NotNull(getHashCodeMethod);
    }

    [Fact]
    public void SignatureComponentRequest_WithMaximumValidValues_ShouldPassValidation()
    {
        // Arrange
        var maxTitle = new string('A', 200); // 200 characters (maximum)
        var maxDescription = new string('B', 500); // 500 characters (maximum)
        
        var request = new SignatureComponentRequest
        {
            Id = long.MaxValue,
            Title = maxTitle,
            Description = maxDescription,
            IsRequired = true,
            Order = int.MaxValue
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void SignatureComponentRequest_WithMultipleValidationErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var request = new SignatureComponentRequest
        {
            Id = 1,
            Title = "A", // Too short
            Description = new string('A', 501), // Too long
            IsRequired = true,
            Order = -1 // Invalid
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
    [InlineData(0)]
    [InlineData(long.MaxValue)]
    [InlineData(-1)]
    [InlineData(12345)]
    public void SignatureComponentRequest_WithDifferentIds_ShouldSetCorrectly(long id)
    {
        // Arrange
        var request = new SignatureComponentRequest
        {
            Id = id,
            Title = "Test Signature Component",
            IsRequired = true,
            Order = 1
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
        Assert.Equal(id, request.Id);
    }

    [Fact]
    public void SignatureComponentRequest_HasNoAdditionalProperties_ShouldOnlyInheritFromBase()
    {
        // Arrange
        var request = new SignatureComponentRequest
        {
            Id = 1,
            Title = "Test Signature Component",
            IsRequired = true,
            Order = 1
        };

        // Act
        var type = typeof(SignatureComponentRequest);
        var declaredProperties = type.GetProperties(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        // Assert - SignatureComponentRequest should not declare any additional properties beyond ComponentRequest
        Assert.DoesNotContain(declaredProperties, p => !p.Name.Contains("EqualityContract"));
	}

	[Fact]
	public void SignatureComponentRequest_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(SignatureComponentRequest);

		// Act & Assert
		type.GetProperty(nameof(SignatureComponentRequest.Id))!.AssertPropertyName("id").AssertRequired(Required.Default);
		type.GetProperty(nameof(SignatureComponentRequest.Title))!.AssertPropertyName("title").AssertRequired(Required.Always);
		type.GetProperty(nameof(SignatureComponentRequest.Description))!.AssertPropertyName("description").AssertRequired(Required.Default);
		type.GetProperty(nameof(SignatureComponentRequest.IsRequired))!.AssertPropertyName("isRequired").AssertRequired(Required.Always);
		type.GetProperty(nameof(SignatureComponentRequest.Order))!.AssertPropertyName("order").AssertRequired(Required.Always);
		type.GetProperty(nameof(SignatureComponentRequest.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
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

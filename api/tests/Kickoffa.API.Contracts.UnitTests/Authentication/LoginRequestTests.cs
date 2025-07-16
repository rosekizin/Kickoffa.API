using Kickoffa.API.Contracts.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.UnitTests.Authentication;

public class LoginRequestTests
{
    [Fact]
    public void LoginRequest_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123",
            RememberMe = true
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void LoginRequest_WithEmptyEmail_ShouldFailValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "",
            Password = "password123",
            RememberMe = false
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O email é obrigatório" && v.MemberNames.Contains("Email"));
    }

    [Fact]
    public void LoginRequest_WithNullEmail_ShouldFailValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = null!,
            Password = "password123",
            RememberMe = false
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O email é obrigatório" && v.MemberNames.Contains("Email"));
    }

    [Fact]
    public void LoginRequest_WithWhitespaceEmail_ShouldFailValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "   ",
            Password = "password123",
            RememberMe = false
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O email é obrigatório" && v.MemberNames.Contains("Email"));
    }

    [Fact]
    public void LoginRequest_WithTooLongEmail_ShouldFailValidation()
    {
        // Arrange
        var longEmail = new string('a', 250) + "@example.com"; // 261 characters
        var request = new LoginRequest
        {
            Email = longEmail,
            Password = "password123",
            RememberMe = false
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "O email deve ter no máximo 255 caracteres" && v.MemberNames.Contains("Email"));
    }

    [Fact]
    public void LoginRequest_WithInvalidEmailFormat_ShouldFailValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "invalid-email",
            Password = "password123",
            RememberMe = false
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Email deve ter um formato válido" && v.MemberNames.Contains("Email"));
    }

    [Fact]
    public void LoginRequest_WithEmptyPassword_ShouldFailValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "",
            RememberMe = false
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "A senha é obrigatória" && v.MemberNames.Contains("Password"));
    }

    [Fact]
    public void LoginRequest_WithNullPassword_ShouldFailValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = null!,
            RememberMe = false
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "A senha é obrigatória" && v.MemberNames.Contains("Password"));
    }

    [Fact]
    public void LoginRequest_WithTooShortPassword_ShouldFailValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "12345", // 5 characters
            RememberMe = false
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "A senha deve ter pelo menos 6 caracteres" && v.MemberNames.Contains("Password"));
    }

    [Fact]
    public void LoginRequest_WithTooLongPassword_ShouldFailValidation()
    {
        // Arrange
        var longPassword = new string('a', 101); // 101 characters
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = longPassword,
            RememberMe = false
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "A senha deve ter no máximo 100 caracteres" && v.MemberNames.Contains("Password"));
    }

    [Fact]
    public void LoginRequest_WithValidPasswordLength_ShouldPassValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password", // 8 characters
            RememberMe = false
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void LoginRequest_RememberMeDefaultValue_ShouldBeFalse()
    {
        // Arrange & Act
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        // Assert
        Assert.False(request.RememberMe);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("test+tag@example.org")]
    [InlineData("123@456.com")]
    public void LoginRequest_WithValidEmailFormats_ShouldPassValidation(string email)
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = email,
            Password = "password123",
            RememberMe = false
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test..test@example.com")]
    [InlineData("test@example")]
    public void LoginRequest_WithInvalidEmailFormats_ShouldFailValidation(string email)
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = email,
            Password = "password123",
            RememberMe = false
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Email deve ter um formato válido" && v.MemberNames.Contains("Email"));
    }

    [Fact]
    public void LoginRequest_WithMultipleValidationErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "invalid-email",
            Password = "123", // Too short
            RememberMe = false
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Equal(2, validationResults.Count);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Email deve ter um formato válido" && v.MemberNames.Contains("Email"));
        Assert.Contains(validationResults, v => v.ErrorMessage == "A senha deve ter pelo menos 6 caracteres" && v.MemberNames.Contains("Password"));
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

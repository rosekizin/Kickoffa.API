using Kickoffa.API.Contracts.User;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.UnitTests.User;

public class ChangeEmailRequestTests
{
    [Fact]
    public void ChangeEmailRequest_WithValidEmail_ShouldPassValidation()
    {
        // Arrange
        var request = new ChangeEmailRequest
        {
            NewEmail = "newemail@example.com"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ChangeEmailRequest_WithEmptyEmail_ShouldFailValidation()
    {
        // Arrange
        var request = new ChangeEmailRequest
        {
            NewEmail = ""
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Email é obrigatório" && v.MemberNames.Contains("NewEmail"));
    }

    [Fact]
    public void ChangeEmailRequest_WithInvalidEmailFormat_ShouldFailValidation()
    {
        // Arrange
        var request = new ChangeEmailRequest
        {
            NewEmail = "invalid-email"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Email deve ter um formato válido" && v.MemberNames.Contains("NewEmail"));
    }

    [Fact]
    public void ChangeEmailRequest_WithTooLongEmail_ShouldFailValidation()
    {
        // Arrange
        var longEmail = new string('a', 250) + "@example.com"; // 261 characters
        var request = new ChangeEmailRequest
        {
            NewEmail = longEmail
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Email deve ter no máximo 255 caracteres" && v.MemberNames.Contains("NewEmail"));
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("test+tag@example.org")]
    [InlineData("123@456.com")]
    public void ChangeEmailRequest_WithValidEmailFormats_ShouldPassValidation(string email)
    {
        // Arrange
        var request = new ChangeEmailRequest
        {
            NewEmail = email
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
    public void ChangeEmailRequest_WithInvalidEmailFormats_ShouldFailValidation(string email)
    {
        // Arrange
        var request = new ChangeEmailRequest
        {
            NewEmail = email
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Email deve ter um formato válido" && v.MemberNames.Contains("NewEmail"));
    }

    [Fact]
    public void ChangeEmailRequest_DefaultValue_ShouldBeEmptyString()
    {
        // Arrange & Act
        var request = new ChangeEmailRequest();

        // Assert
        Assert.Equal(string.Empty, request.NewEmail);
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

public class ConfirmEmailChangeRequestTests
{
    [Fact]
    public void ConfirmEmailChangeRequest_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new ConfirmEmailChangeRequest
        {
            NewEmail = "newemail@example.com",
            ConfirmationToken = "valid-token-123"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ConfirmEmailChangeRequest_WithEmptyEmail_ShouldFailValidation()
    {
        // Arrange
        var request = new ConfirmEmailChangeRequest
        {
            NewEmail = "",
            ConfirmationToken = "valid-token-123"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Email é obrigatório" && v.MemberNames.Contains("NewEmail"));
    }

    [Fact]
    public void ConfirmEmailChangeRequest_WithEmptyToken_ShouldFailValidation()
    {
        // Arrange
        var request = new ConfirmEmailChangeRequest
        {
            NewEmail = "newemail@example.com",
            ConfirmationToken = ""
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Token de confirmação é obrigatório" && v.MemberNames.Contains("ConfirmationToken"));
    }

    [Fact]
    public void ConfirmEmailChangeRequest_WithInvalidEmailFormat_ShouldFailValidation()
    {
        // Arrange
        var request = new ConfirmEmailChangeRequest
        {
            NewEmail = "invalid-email",
            ConfirmationToken = "valid-token-123"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Email deve ter um formato válido" && v.MemberNames.Contains("NewEmail"));
    }

    [Fact]
    public void ConfirmEmailChangeRequest_WithTooLongEmail_ShouldFailValidation()
    {
        // Arrange
        var longEmail = new string('a', 250) + "@example.com"; // 261 characters
        var request = new ConfirmEmailChangeRequest
        {
            NewEmail = longEmail,
            ConfirmationToken = "valid-token-123"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Email deve ter no máximo 255 caracteres" && v.MemberNames.Contains("NewEmail"));
    }

    [Fact]
    public void ConfirmEmailChangeRequest_WithMultipleValidationErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var request = new ConfirmEmailChangeRequest
        {
            NewEmail = "invalid-email", // Invalid format
            ConfirmationToken = "" // Empty token
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Equal(2, validationResults.Count);
        Assert.Contains(validationResults, v => v.ErrorMessage == "Email deve ter um formato válido" && v.MemberNames.Contains("NewEmail"));
        Assert.Contains(validationResults, v => v.ErrorMessage == "Token de confirmação é obrigatório" && v.MemberNames.Contains("ConfirmationToken"));
    }

    [Theory]
    [InlineData("token123")]
    [InlineData("very-long-token-with-special-characters-123456789")]
    [InlineData("ABC123")]
    [InlineData("token-with-dashes-and-numbers-456")]
    public void ConfirmEmailChangeRequest_WithValidTokens_ShouldPassValidation(string token)
    {
        // Arrange
        var request = new ConfirmEmailChangeRequest
        {
            NewEmail = "test@example.com",
            ConfirmationToken = token
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ConfirmEmailChangeRequest_DefaultValues_ShouldBeEmptyStrings()
    {
        // Arrange & Act
        var request = new ConfirmEmailChangeRequest();

        // Assert
        Assert.Equal(string.Empty, request.NewEmail);
        Assert.Equal(string.Empty, request.ConfirmationToken);
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

public class ChangeEmailResponseTests
{
    [Fact]
    public void ChangeEmailResponse_WithSuccessfulOperation_ShouldSetCorrectly()
    {
        // Arrange
        var success = true;
        var message = "Email change initiated successfully";
        var emailSentTo = "newemail@example.com";

        // Act
        var response = new ChangeEmailResponse
        {
            Success = success,
            Message = message,
            EmailSentTo = emailSentTo
        };

        // Assert
        Assert.Equal(success, response.Success);
        Assert.Equal(message, response.Message);
        Assert.Equal(emailSentTo, response.EmailSentTo);
    }

    [Fact]
    public void ChangeEmailResponse_WithFailedOperation_ShouldSetCorrectly()
    {
        // Arrange
        var success = false;
        var message = "Email change failed";

        // Act
        var response = new ChangeEmailResponse
        {
            Success = success,
            Message = message,
            EmailSentTo = null
        };

        // Assert
        Assert.Equal(success, response.Success);
        Assert.Equal(message, response.Message);
        Assert.Null(response.EmailSentTo);
    }

    [Fact]
    public void ChangeEmailResponse_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var response = new ChangeEmailResponse();

        // Assert
        Assert.False(response.Success);
        Assert.Equal(string.Empty, response.Message);
        Assert.Null(response.EmailSentTo);
    }

    [Theory]
    [InlineData(true, "Operation successful")]
    [InlineData(false, "Operation failed")]
    [InlineData(true, "")]
    [InlineData(false, "")]
    public void ChangeEmailResponse_WithDifferentSuccessAndMessages_ShouldSetCorrectly(bool success, string message)
    {
        // Arrange & Act
        var response = new ChangeEmailResponse
        {
            Success = success,
            Message = message
        };

        // Assert
        Assert.Equal(success, response.Success);
        Assert.Equal(message, response.Message);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user@domain.org")]
    [InlineData(null)]
    public void ChangeEmailResponse_WithDifferentEmailSentTo_ShouldSetCorrectly(string? emailSentTo)
    {
        // Arrange & Act
        var response = new ChangeEmailResponse
        {
            Success = true,
            Message = "Test message",
            EmailSentTo = emailSentTo
        };

        // Assert
        Assert.Equal(emailSentTo, response.EmailSentTo);
    }

    [Fact]
    public void ChangeEmailResponse_ForInitiateEmailChange_ShouldIncludeEmailSentTo()
    {
        // Arrange & Act
        var response = new ChangeEmailResponse
        {
            Success = true,
            Message = "Confirmation email sent",
            EmailSentTo = "newemail@example.com"
        };

        // Assert
        Assert.True(response.Success);
        Assert.Equal("Confirmation email sent", response.Message);
        Assert.Equal("newemail@example.com", response.EmailSentTo);
    }

    [Fact]
    public void ChangeEmailResponse_ForConfirmEmailChange_ShouldNotIncludeEmailSentTo()
    {
        // Arrange & Act
        var response = new ChangeEmailResponse
        {
            Success = true,
            Message = "Email changed successfully",
            EmailSentTo = null
        };

        // Assert
        Assert.True(response.Success);
        Assert.Equal("Email changed successfully", response.Message);
        Assert.Null(response.EmailSentTo);
    }
}

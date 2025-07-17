using Kickoffa.API.Domain.ProcessResult;
using System.Net;

namespace Kickoffa.API.Domain.UnitTests.ProcessResult;

/// <summary>
/// Testes unitários para Result e Result<T>
/// </summary>
public class ResultTests
{
	#region Result (non-generic) Tests

	[Fact]
	public void Result_Success_ShouldCreateSuccessfulResult()
	{
		// Act
		var result = Result.Success();

		// Assert
		Assert.True(result.IsSuccess);
		Assert.False(result.IsFailure);
		Assert.Equal(string.Empty, result.Error);
		Assert.Null(result.ErrorObject);
	}

	[Fact]
	public void Result_Failure_WithMessage_ShouldCreateFailureResult()
	{
		// Arrange
		var errorMessage = "Operação falhou";

		// Act
		var result = Result.Failure(errorMessage);

		// Assert
		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Equal(errorMessage, result.Error);
		Assert.Null(result.ErrorObject);
	}

	[Fact]
	public void Result_Failure_WithErrorObject_ShouldCreateFailureResultWithErrorObject()
	{
		// Arrange
		var error = new Error("TEST_ERROR", "Erro de teste", HttpStatusCode.BadRequest);

		// Act
		var result = Result.Failure(error);

		// Assert
		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Equal("TEST_ERROR: Erro de teste", result.Error);
		Assert.Equal(error, result.ErrorObject);
	}

	[Fact]
	public void Result_Failure_WithNullErrorMessage_ShouldUseDefaultMessage()
	{
		// Act
		var result = Result.Failure((string)null!);

		// Assert
		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Equal("Erro não especificado", result.Error);
		Assert.Null(result.ErrorObject);
	}

	[Fact]
	public void Result_Failure_WithEmptyErrorMessage_ShouldUseDefaultMessage()
	{
		// Act
		var result = Result.Failure(string.Empty);

		// Assert
		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Equal("Erro não especificado", result.Error);
		Assert.Null(result.ErrorObject);
	}

	[Fact]
	public void Result_Failure_WithWhitespaceErrorMessage_ShouldUseDefaultMessage()
	{
		// Act
		var result = Result.Failure("   ");

		// Assert
		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Equal("Erro não especificado", result.Error);
		Assert.Null(result.ErrorObject);
	}

	[Fact]
	public void Result_Failure_WithNullErrorObject_ShouldUseDefaultMessage()
	{
		// Act
		var result = Result.Failure((Error)null!);

		// Assert
		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Equal("Erro não especificado", result.Error);
		Assert.Null(result.ErrorObject);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Result_ImplicitConversion_FromBool_ShouldCreateCorrectResult(bool success)
	{
		// Act
		Result result = success;

		// Assert
		Assert.Equal(success, result.IsSuccess);
		Assert.Equal(!success, result.IsFailure);
		
		if (success)
		{
			Assert.Equal(string.Empty, result.Error);
			Assert.Null(result.ErrorObject);
		}
		else
		{
			Assert.Equal("Operação falhou", result.Error);
			Assert.Null(result.ErrorObject);
		}
	}

	#endregion

	#region Result<T> Tests

	[Fact]
	public void ResultT_Success_WithValue_ShouldCreateSuccessfulResult()
	{
		// Arrange
		var value = "Test Value";

		// Act
		var result = Result<string>.Success(value);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.False(result.IsFailure);
		Assert.Equal(value, result.Value);
		Assert.Equal(string.Empty, result.Error);
		Assert.Null(result.ErrorObject);
	}

	[Fact]
	public void ResultT_Success_WithNullValue_ShouldCreateSuccessfulResult()
	{
		// Act
		var result = Result<string>.Success(null!);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.False(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal(string.Empty, result.Error);
		Assert.Null(result.ErrorObject);
	}

	[Fact]
	public void ResultT_Failure_WithMessage_ShouldCreateFailureResult()
	{
		// Arrange
		var errorMessage = "Operação falhou";

		// Act
		var result = Result<string>.Failure(errorMessage);

		// Assert
		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal(errorMessage, result.Error);
		Assert.Null(result.ErrorObject);
	}

	[Fact]
	public void ResultT_Failure_WithErrorObject_ShouldCreateFailureResultWithErrorObject()
	{
		// Arrange
		var error = new Error("TEST_ERROR", "Erro de teste", HttpStatusCode.BadRequest);

		// Act
		var result = Result<string>.Failure(error);

		// Assert
		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal("TEST_ERROR: Erro de teste", result.Error);
		Assert.Equal(error, result.ErrorObject);
	}

	[Fact]
	public void ResultT_ImplicitConversion_FromValue_ShouldCreateSuccessfulResult()
	{
		// Arrange
		var value = 42;

		// Act
		Result<int> result = value;

		// Assert
		Assert.True(result.IsSuccess);
		Assert.False(result.IsFailure);
		Assert.Equal(value, result.Value);
		Assert.Equal(string.Empty, result.Error);
		Assert.Null(result.ErrorObject);
	}

	[Fact]
	public void ResultT_ImplicitConversion_FromError_ShouldCreateFailureResult()
	{
		// Arrange
		var error = new Error("TEST_ERROR", "Erro de teste", HttpStatusCode.BadRequest);

		// Act
		Result<string> result = error;

		// Assert
		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal("TEST_ERROR: Erro de teste", result.Error);
		Assert.Equal(error, result.ErrorObject);
	}

	#endregion

	#region ResultExtensions Tests

	[Fact]
	public void ToGeneric_WithSuccessfulResult_ShouldCreateSuccessfulGenericResult()
	{
		// Arrange
		var result = Result.Success();

		// Act
		var genericResult = result.ToGeneric<string>();

		// Assert
		Assert.True(genericResult.IsSuccess);
		Assert.False(genericResult.IsFailure);
		Assert.Null(genericResult.Value);
		Assert.Equal(string.Empty, genericResult.Error);
		Assert.Null(genericResult.ErrorObject);
	}

	[Fact]
	public void ToGeneric_WithFailureResult_ShouldCreateFailureGenericResult()
	{
		// Arrange
		var errorMessage = "Operação falhou";
		var result = Result.Failure(errorMessage);

		// Act
		var genericResult = result.ToGeneric<string>();

		// Assert
		Assert.False(genericResult.IsSuccess);
		Assert.True(genericResult.IsFailure);
		Assert.Null(genericResult.Value);
		Assert.Equal(errorMessage, genericResult.Error);
		Assert.Null(genericResult.ErrorObject);
	}

	[Fact]
	public void ToResult_WithValue_ShouldCreateSuccessfulResult()
	{
		// Arrange
		var value = "Test Value";

		// Act
		var result = value.ToResult();

		// Assert
		Assert.True(result.IsSuccess);
		Assert.False(result.IsFailure);
		Assert.Equal(value, result.Value);
		Assert.Equal(string.Empty, result.Error);
		Assert.Null(result.ErrorObject);
	}

	[Fact]
	public void ToFailure_WithError_ShouldCreateFailureResult()
	{
		// Arrange
		var error = new Error("TEST_ERROR", "Erro de teste", HttpStatusCode.BadRequest);

		// Act
		var result = error.ToFailure<string>();

		// Assert
		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal("TEST_ERROR: Erro de teste", result.Error);
		Assert.Equal(error, result.ErrorObject);
	}

	#endregion

	#region Complex Type Tests

	[Fact]
	public void ResultT_WithComplexType_ShouldWorkCorrectly()
	{
		// Arrange
		var complexObject = new { Id = 1, Name = "Test", Items = new[] { 1, 2, 3 } };

		// Act
		var result = Result<object>.Success(complexObject);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal(complexObject, result.Value);
	}

	[Fact]
	public void ResultT_WithValueType_ShouldWorkCorrectly()
	{
		// Arrange
		var dateTime = DateTime.Now;

		// Act
		var result = Result<DateTime>.Success(dateTime);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal(dateTime, result.Value);
	}

	#endregion
}

using Kickoffa.API.AspNet.Infrastructure.ErrorHandling;
using Kickoffa.API.Domain.ProcessResult;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Kickoffa.API.AspNet.Infrastructure.UnitTests.ErrorHandling;

/// <summary>
/// Testes unitários para ErrorFactory
/// </summary>
public class ErrorFactoryTests
{
	private readonly IErrorFactory _errorFactory;

	public ErrorFactoryTests()
	{
		_errorFactory = new ErrorFactory();
	}

	#region CreateNotFound Tests

	[Fact]
	public void CreateNotFound_WithValidError_ShouldReturnNotFoundProblemDetails()
	{
		// Arrange
		var error = new Error("CUSTOMER_NOT_FOUND", "Cliente não encontrado", HttpStatusCode.NotFound);

		// Act
		var result = _errorFactory.CreateNotFound(error);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Not Found", result.Title);
		Assert.Equal(404, result.Status);
		Assert.Equal("Cliente não encontrado", result.Detail);
	}

	[Fact]
	public void CreateNotFound_WithErrorWithoutMessage_ShouldReturnDefaultMessage()
	{
		// Arrange
		var error = new Error("CUSTOMER_NOT_FOUND", "", HttpStatusCode.NotFound);

		// Act
		var result = _errorFactory.CreateNotFound(error);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Not Found", result.Title);
		Assert.Equal(404, result.Status);
		Assert.Equal("O recurso não foi encontrado.", result.Detail);
	}

	[Fact]
	public void CreateNotFound_WithErrorWithMetadata_ShouldIncludeMetadataInExtensions()
	{
		// Arrange
		var metadata = new Dictionary<string, object>
		{
			{ "customerId", 123L },
			{ "operation", "update" }
		};
		var error = new Error("CUSTOMER_NOT_FOUND", "Cliente não encontrado", HttpStatusCode.NotFound, metadata);

		// Act
		var result = _errorFactory.CreateNotFound(error);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.Extensions.ContainsKey("metadata"));
		var resultMetadata = result.Extensions["metadata"] as IDictionary<string, object>;
		Assert.NotNull(resultMetadata);
		Assert.Equal(123L, resultMetadata["customerId"]);
		Assert.Equal("update", resultMetadata["operation"]);
	}

	[Fact]
	public void CreateNotFound_WithErrorWithoutMetadata_ShouldNotIncludeMetadataInExtensions()
	{
		// Arrange
		var error = new Error("CUSTOMER_NOT_FOUND", "Cliente não encontrado", HttpStatusCode.NotFound);

		// Act
		var result = _errorFactory.CreateNotFound(error);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.Extensions.ContainsKey("metadata"));
	}

	#endregion

	#region CreateBadRequest Tests

	[Fact]
	public void CreateBadRequest_WithValidError_ShouldReturnBadRequestProblemDetails()
	{
		// Arrange
		var error = new Error("INVALID_DATA", "Dados inválidos", HttpStatusCode.BadRequest);

		// Act
		var result = _errorFactory.CreateBadRequest(error);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Bad Request", result.Title);
		Assert.Equal(400, result.Status);
		Assert.Equal("Dados inválidos", result.Detail);
	}

	[Fact]
	public void CreateBadRequest_WithErrorWithoutMessage_ShouldReturnDefaultMessage()
	{
		// Arrange
		var error = new Error("INVALID_DATA", "", HttpStatusCode.BadRequest);

		// Act
		var result = _errorFactory.CreateBadRequest(error);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Bad Request", result.Title);
		Assert.Equal(400, result.Status);
		Assert.Equal("O recurso tem inconsistências.", result.Detail);
	}

	[Fact]
	public void CreateBadRequest_WithErrorWithMetadata_ShouldIncludeMetadataInExtensions()
	{
		// Arrange
		var metadata = new Dictionary<string, object>
		{
			{ "field", "email" },
			{ "value", "invalid-email" }
		};
		var error = new Error("INVALID_EMAIL", "Email inválido", HttpStatusCode.BadRequest, metadata);

		// Act
		var result = _errorFactory.CreateBadRequest(error);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.Extensions.ContainsKey("metadata"));
		var resultMetadata = result.Extensions["metadata"] as IDictionary<string, object>;
		Assert.NotNull(resultMetadata);
		Assert.Equal("email", resultMetadata["field"]);
		Assert.Equal("invalid-email", resultMetadata["value"]);
	}

	#endregion

	#region CreateInternalServerError Tests

	[Fact]
	public void CreateInternalServerError_WithValidError_ShouldReturnInternalServerErrorProblemDetails()
	{
		// Arrange
		var error = new Error("DATABASE_ERROR", "Erro de banco de dados", HttpStatusCode.InternalServerError);

		// Act
		var result = _errorFactory.CreateInternalServerError(error);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Internal Server Error", result.Title);
		Assert.Equal(500, result.Status);
		Assert.Equal("Erro de banco de dados", result.Detail);
	}

	[Fact]
	public void CreateInternalServerError_WithErrorWithoutMessage_ShouldReturnDefaultMessage()
	{
		// Arrange
		var error = new Error("DATABASE_ERROR", "", HttpStatusCode.InternalServerError);

		// Act
		var result = _errorFactory.CreateInternalServerError(error);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Internal Server Error", result.Title);
		Assert.Equal(500, result.Status);
		Assert.Equal("Ocorreu um erro inesperado. Contate o suporte.", result.Detail);
	}

	[Fact]
	public void CreateInternalServerError_WithErrorWithMetadata_ShouldIncludeMetadataInExtensions()
	{
		// Arrange
		var metadata = new Dictionary<string, object>
		{
			{ "operation", "save" },
			{ "table", "customers" }
		};
		var error = new Error("DATABASE_ERROR", "Erro de banco de dados", HttpStatusCode.InternalServerError, metadata);

		// Act
		var result = _errorFactory.CreateInternalServerError(error);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.Extensions.ContainsKey("metadata"));
		var resultMetadata = result.Extensions["metadata"] as IDictionary<string, object>;
		Assert.NotNull(resultMetadata);
		Assert.Equal("save", resultMetadata["operation"]);
		Assert.Equal("customers", resultMetadata["table"]);
	}

	#endregion

	#region Edge Cases Tests

	[Theory]
	[InlineData(HttpStatusCode.NotFound)]
	[InlineData(HttpStatusCode.BadRequest)]
	[InlineData(HttpStatusCode.InternalServerError)]
	public void CreateMethods_WithNullMessage_ShouldUseDefaultMessage(HttpStatusCode statusCode)
	{
		// Arrange
		var error = new Error("TEST_ERROR", null!, statusCode);

		// Act & Assert
		switch (statusCode)
		{
			case HttpStatusCode.NotFound:
				var notFoundResult = _errorFactory.CreateNotFound(error);
				Assert.Equal("O recurso não foi encontrado.", notFoundResult.Detail);
				break;
			case HttpStatusCode.BadRequest:
				var badRequestResult = _errorFactory.CreateBadRequest(error);
				Assert.Equal("O recurso tem inconsistências.", badRequestResult.Detail);
				break;
			case HttpStatusCode.InternalServerError:
				var internalServerErrorResult = _errorFactory.CreateInternalServerError(error);
				Assert.Equal("Ocorreu um erro inesperado. Contate o suporte.", internalServerErrorResult.Detail);
				break;
		}
	}

	[Fact]
	public void CreateMethods_WithEmptyMetadata_ShouldNotIncludeMetadataInExtensions()
	{
		// Arrange
		var emptyMetadata = new Dictionary<string, object>();
		var error = new Error("TEST_ERROR", "Test message", HttpStatusCode.BadRequest, emptyMetadata);

		// Act
		var result = _errorFactory.CreateBadRequest(error);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.Extensions.ContainsKey("metadata"));
		var resultMetadata = result.Extensions["metadata"] as IDictionary<string, object>;
		Assert.NotNull(resultMetadata);
		Assert.Empty(resultMetadata);
	}

	#endregion
}

using Kickoffa.API.AspNet.Infrastructure.ErrorHandling;
using Kickoffa.API.Domain.ProcessResult;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Net;

namespace Kickoffa.API.AspNet.Infrastructure.UnitTests.ErrorHandling;

/// <summary>
/// Testes unitários para ActionResultErrorHandler
/// </summary>
public class ActionResultErrorHandlerTests
{
	private readonly IActionResultErrorHandler _actionResultErrorHandler;
	private readonly IErrorFactory _errorFactory;

	public ActionResultErrorHandlerTests()
	{
		_errorFactory = Substitute.For<IErrorFactory>();
		_actionResultErrorHandler = new ActionResultErrorHandler(_errorFactory);
	}

	#region GetActionResultFromError Tests

	[Fact]
	public void GetActionResultFromError_WithBadRequestError_ShouldReturnBadRequestObjectResult()
	{
		// Arrange
		var error = new Error("INVALID_DATA", "Dados inválidos", HttpStatusCode.BadRequest);
		var expectedProblemDetails = new ProblemDetails
		{
			Title = "Bad Request",
			Status = 400,
			Detail = "Dados inválidos"
		};

		_errorFactory.CreateBadRequest(error).Returns(expectedProblemDetails);

		// Act
		var result = _actionResultErrorHandler.GetActionResultFromError(error);

		// Assert
		var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal(expectedProblemDetails, badRequestResult.Value);
		_errorFactory.Received(1).CreateBadRequest(error);
	}

	[Fact]
	public void GetActionResultFromError_WithNotFoundError_ShouldReturnNotFoundObjectResult()
	{
		// Arrange
		var error = new Error("CUSTOMER_NOT_FOUND", "Cliente não encontrado", HttpStatusCode.NotFound);
		var expectedProblemDetails = new ProblemDetails
		{
			Title = "Not Found",
			Status = 404,
			Detail = "Cliente não encontrado"
		};

		_errorFactory.CreateNotFound(error).Returns(expectedProblemDetails);

		// Act
		var result = _actionResultErrorHandler.GetActionResultFromError(error);

		// Assert
		var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
		Assert.Equal(expectedProblemDetails, notFoundResult.Value);
		_errorFactory.Received(1).CreateNotFound(error);
	}

	[Fact]
	public void GetActionResultFromError_WithInternalServerError_ShouldReturnObjectResultWith500StatusCode()
	{
		// Arrange
		var error = new Error("DATABASE_ERROR", "Erro de banco de dados", HttpStatusCode.InternalServerError);
		var expectedProblemDetails = new ProblemDetails
		{
			Title = "Internal Server Error",
			Status = 500,
			Detail = "Erro de banco de dados"
		};

		_errorFactory.CreateInternalServerError(error).Returns(expectedProblemDetails);

		// Act
		var result = _actionResultErrorHandler.GetActionResultFromError(error);

		// Assert
		var objectResult = Assert.IsType<ObjectResult>(result);
		Assert.Equal(500, objectResult.StatusCode);
		Assert.Equal(expectedProblemDetails, objectResult.Value);
		_errorFactory.Received(1).CreateInternalServerError(error);
	}

	[Theory]
	[InlineData(HttpStatusCode.Unauthorized)]
	[InlineData(HttpStatusCode.Forbidden)]
	[InlineData(HttpStatusCode.Conflict)]
	[InlineData(HttpStatusCode.UnprocessableEntity)]
	[InlineData(HttpStatusCode.ServiceUnavailable)]
	public void GetActionResultFromError_WithUnsupportedStatusCode_ShouldReturnInternalServerError(HttpStatusCode statusCode)
	{
		// Arrange
		var error = new Error("UNSUPPORTED_ERROR", "Erro não suportado", statusCode);
		var expectedProblemDetails = new ProblemDetails
		{
			Title = "Internal Server Error",
			Status = 500,
			Detail = "Erro não suportado"
		};

		_errorFactory.CreateInternalServerError(error).Returns(expectedProblemDetails);

		// Act
		var result = _actionResultErrorHandler.GetActionResultFromError(error);

		// Assert
		var objectResult = Assert.IsType<ObjectResult>(result);
		Assert.Equal(500, objectResult.StatusCode);
		Assert.Equal(expectedProblemDetails, objectResult.Value);
		_errorFactory.Received(1).CreateInternalServerError(error);
	}

	[Fact]
	public void GetActionResultFromError_WithErrorContainingMetadata_ShouldPassMetadataToErrorFactory()
	{
		// Arrange
		var metadata = new Dictionary<string, object>
		{
			{ "customerId", 123L },
			{ "operation", "update" }
		};
		var error = new Error("CUSTOMER_NOT_FOUND", "Cliente não encontrado", HttpStatusCode.NotFound, metadata);
		var expectedProblemDetails = new ProblemDetails
		{
			Title = "Not Found",
			Status = 404,
			Detail = "Cliente não encontrado"
		};
		expectedProblemDetails.Extensions.Add("metadata", metadata);

		_errorFactory.CreateNotFound(error).Returns(expectedProblemDetails);

		// Act
		var result = _actionResultErrorHandler.GetActionResultFromError(error);

		// Assert
		var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
		var problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);
		Assert.True(problemDetails.Extensions.ContainsKey("metadata"));
		_errorFactory.Received(1).CreateNotFound(error);
	}

	[Fact]
	public void GetActionResultFromError_WithMultipleCalls_ShouldCallCorrectFactoryMethod()
	{
		// Arrange
		var badRequestError = new Error("INVALID_DATA", "Dados inválidos", HttpStatusCode.BadRequest);
		var notFoundError = new Error("CUSTOMER_NOT_FOUND", "Cliente não encontrado", HttpStatusCode.NotFound);
		var internalServerError = new Error("DATABASE_ERROR", "Erro de banco", HttpStatusCode.InternalServerError);

		var badRequestProblemDetails = new ProblemDetails { Title = "Bad Request", Status = 400 };
		var notFoundProblemDetails = new ProblemDetails { Title = "Not Found", Status = 404 };
		var internalServerProblemDetails = new ProblemDetails { Title = "Internal Server Error", Status = 500 };

		_errorFactory.CreateBadRequest(badRequestError).Returns(badRequestProblemDetails);
		_errorFactory.CreateNotFound(notFoundError).Returns(notFoundProblemDetails);
		_errorFactory.CreateInternalServerError(internalServerError).Returns(internalServerProblemDetails);

		// Act
		var badRequestResult = _actionResultErrorHandler.GetActionResultFromError(badRequestError);
		var notFoundResult = _actionResultErrorHandler.GetActionResultFromError(notFoundError);
		var internalServerResult = _actionResultErrorHandler.GetActionResultFromError(internalServerError);

		// Assert
		Assert.IsType<BadRequestObjectResult>(badRequestResult);
		Assert.IsType<NotFoundObjectResult>(notFoundResult);
		Assert.IsType<ObjectResult>(internalServerResult);

		_errorFactory.Received(1).CreateBadRequest(badRequestError);
		_errorFactory.Received(1).CreateNotFound(notFoundError);
		_errorFactory.Received(1).CreateInternalServerError(internalServerError);
	}

	#endregion

	#region Edge Cases Tests

	[Fact]
	public void GetActionResultFromError_WithErrorFactoryReturningNull_ShouldHandleGracefully()
	{
		// Arrange
		var error = new Error("TEST_ERROR", "Test message", HttpStatusCode.BadRequest);
		_errorFactory.CreateBadRequest(error).Returns((ProblemDetails?)null);

		// Act
		var result = _actionResultErrorHandler.GetActionResultFromError(error);

		// Assert
		var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Null(badRequestResult.Value);
	}

	#endregion
}
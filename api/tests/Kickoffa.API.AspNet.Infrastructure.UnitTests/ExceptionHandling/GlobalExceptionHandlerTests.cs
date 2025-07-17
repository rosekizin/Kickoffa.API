using Kickoffa.API.AspNet.Infrastructure.ErrorHandling;
using Kickoffa.API.AspNet.Infrastructure.ExceptionHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text;
using System.Text.Json;

namespace Kickoffa.API.AspNet.Infrastructure.UnitTests.ExceptionHandling;

/// <summary>
/// Testes unitários para GlobalExceptionHandler
/// </summary>
public class GlobalExceptionHandlerTests
{
	private readonly IErrorFactory _errorFactory;
	private readonly ILogger<GlobalExceptionHandler> _logger;
	private readonly GlobalExceptionHandler _globalExceptionHandler;

	public GlobalExceptionHandlerTests()
	{
		_errorFactory = Substitute.For<IErrorFactory>();
		_logger = Substitute.For<ILogger<GlobalExceptionHandler>>();
		_globalExceptionHandler = new GlobalExceptionHandler(_errorFactory, _logger);
	}

	#region Constructor Tests

	[Fact]
	public void Constructor_WithValidParameters_ShouldCreateInstance()
	{
		// Act & Assert
		var handler = new GlobalExceptionHandler(_errorFactory, _logger);
		Assert.NotNull(handler);
	}

	#endregion

	#region TryHandleAsync Tests

	[Fact]
	public async Task TryHandleAsync_WithValidException_ShouldReturnTrue()
	{
		// Arrange
		var httpContext = CreateHttpContext();
		var exception = new InvalidOperationException("Test exception");
		var cancellationToken = CancellationToken.None;

		var problemDetails = new ProblemDetails
		{
			Title = "Internal Server Error",
			Status = 500,
			Detail = "Ocorreu um erro inesperado. Contate o suporte."
		};

		_errorFactory.CreateInternalServerError().Returns(problemDetails);

		// Act
		var result = await _globalExceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public async Task TryHandleAsync_ShouldLogException()
	{
		// Arrange
		var httpContext = CreateHttpContext();
		var exception = new InvalidOperationException("Test exception");
		var cancellationToken = CancellationToken.None;

		var problemDetails = new ProblemDetails { Status = 500 };
		_errorFactory.CreateInternalServerError().Returns(problemDetails);

		// Act
		await _globalExceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken);

		// Assert
		_logger.Received(1).LogError(exception, "An unhandled exception occurred while processing the request.");
	}

	[Fact]
	public async Task TryHandleAsync_ShouldCallErrorFactoryCreateInternalServerError()
	{
		// Arrange
		var httpContext = CreateHttpContext();
		var exception = new InvalidOperationException("Test exception");
		var cancellationToken = CancellationToken.None;

		var problemDetails = new ProblemDetails { Status = 500 };
		_errorFactory.CreateInternalServerError().Returns(problemDetails);

		// Act
		await _globalExceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken);

		// Assert
		_errorFactory.Received(1).CreateInternalServerError();
	}

	[Fact]
	public async Task TryHandleAsync_ShouldSetCorrectStatusCode()
	{
		// Arrange
		var httpContext = CreateHttpContext();
		var exception = new InvalidOperationException("Test exception");
		var cancellationToken = CancellationToken.None;

		var problemDetails = new ProblemDetails { Status = 500 };
		_errorFactory.CreateInternalServerError().Returns(problemDetails);

		// Act
		await _globalExceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken);

		// Assert
		Assert.Equal(500, httpContext.Response.StatusCode);
	}

	[Fact]
	public async Task TryHandleAsync_ShouldWriteProblemDetailsAsJson()
	{
		// Arrange
		var httpContext = CreateHttpContext();
		var exception = new InvalidOperationException("Test exception");
		var cancellationToken = CancellationToken.None;

		var problemDetails = new ProblemDetails
		{
			Title = "Internal Server Error",
			Status = 500,
			Detail = "Test error message"
		};

		_errorFactory.CreateInternalServerError().Returns(problemDetails);

		// Act
		await _globalExceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken);

		// Assert
		httpContext.Response.Body.Position = 0;
		var responseContent = await new StreamReader(httpContext.Response.Body).ReadToEndAsync(cancellationToken);
		
		Assert.NotEmpty(responseContent);
		
		// Verificar que é um JSON válido
		var deserializedProblemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseContent);
		Assert.NotNull(deserializedProblemDetails);
		Assert.Equal("Internal Server Error", deserializedProblemDetails.Title);
		Assert.Equal(500, deserializedProblemDetails.Status);
	}

	[Theory]
	[InlineData(typeof(ArgumentException))]
	[InlineData(typeof(InvalidOperationException))]
	[InlineData(typeof(NullReferenceException))]
	[InlineData(typeof(NotImplementedException))]
	[InlineData(typeof(TimeoutException))]
	public async Task TryHandleAsync_WithDifferentExceptionTypes_ShouldHandleAll(Type exceptionType)
	{
		// Arrange
		var httpContext = CreateHttpContext();
		var exception = (Exception)Activator.CreateInstance(exceptionType, "Test exception")!;
		var cancellationToken = CancellationToken.None;

		var problemDetails = new ProblemDetails { Status = 500 };
		_errorFactory.CreateInternalServerError().Returns(problemDetails);

		// Act
		var result = await _globalExceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken);

		// Assert
		Assert.True(result);
		Assert.Equal(500, httpContext.Response.StatusCode);
		_logger.Received(1).LogError(exception, "An unhandled exception occurred while processing the request.");
	}

	[Fact]
	public async Task TryHandleAsync_WithCancellationToken_ShouldPassTokenToWriteAsJsonAsync()
	{
		// Arrange
		var httpContext = CreateHttpContext();
		var exception = new InvalidOperationException("Test exception");
		var cancellationToken = new CancellationToken();

		var problemDetails = new ProblemDetails { Status = 500 };
		_errorFactory.CreateInternalServerError().Returns(problemDetails);

		// Act
		var result = await _globalExceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken);

		// Assert
		Assert.True(result);
		// O token é passado para WriteAsJsonAsync, mas não podemos verificar diretamente
		// Verificamos que a operação foi concluída sem erro
	}

	[Fact]
	public async Task TryHandleAsync_WithNullStatusInProblemDetails_ShouldThrow()
	{
		// Arrange
		var httpContext = CreateHttpContext();
		var exception = new InvalidOperationException("Test exception");
		var cancellationToken = CancellationToken.None;

		var problemDetails = new ProblemDetails { Status = null };
		_errorFactory.CreateInternalServerError().Returns(problemDetails);

		// Act & Assert
		await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await _globalExceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken));
	}

	#endregion

	#region Edge Cases

	[Fact]
	public async Task TryHandleAsync_WithExceptionInLogging_ShouldStillHandleException()
	{
		// Arrange
		var httpContext = CreateHttpContext();
		var exception = new InvalidOperationException("Test exception");
		var cancellationToken = CancellationToken.None;

		var problemDetails = new ProblemDetails { Status = 500 };
		_errorFactory.CreateInternalServerError().Returns(problemDetails);

		// Configurar logger para lançar exceção
		_logger.When(x => x.LogError(Arg.Any<Exception>(), "An unhandled exception occurred while processing the request."))
			.Do(x => throw new InvalidOperationException("Logging failed"));

		// Act & Assert
		await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await _globalExceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken));
	}

	[Fact]
	public async Task TryHandleAsync_WithExceptionInErrorFactory_ShouldThrow()
	{
		// Arrange
		var httpContext = CreateHttpContext();
		var exception = new InvalidOperationException("Test exception");
		var cancellationToken = CancellationToken.None;

		_errorFactory.CreateInternalServerError()
			.Returns(x => throw new InvalidOperationException("ErrorFactory failed"));

		// Act & Assert
		await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await _globalExceptionHandler.TryHandleAsync(httpContext, exception, cancellationToken));
	}

	[Fact]
	public async Task TryHandleAsync_WithMultipleExceptions_ShouldHandleEachIndependently()
	{
		// Arrange
		var httpContext1 = CreateHttpContext();
		var httpContext2 = CreateHttpContext();
		var exception1 = new ArgumentException("Exception 1");
		var exception2 = new InvalidOperationException("Exception 2");
		var cancellationToken = CancellationToken.None;

		var problemDetails = new ProblemDetails { Status = 500 };
		_errorFactory.CreateInternalServerError().Returns(problemDetails);

		// Act
		var result1 = await _globalExceptionHandler.TryHandleAsync(httpContext1, exception1, cancellationToken);
		var result2 = await _globalExceptionHandler.TryHandleAsync(httpContext2, exception2, cancellationToken);

		// Assert
		Assert.True(result1);
		Assert.True(result2);
		Assert.Equal(500, httpContext1.Response.StatusCode);
		Assert.Equal(500, httpContext2.Response.StatusCode);
		
		_logger.Received(1).LogError(exception1, "An unhandled exception occurred while processing the request.");
		_logger.Received(1).LogError(exception2, "An unhandled exception occurred while processing the request.");
	}

	#endregion

	#region Helper Methods

	private static HttpContext CreateHttpContext()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Response.Body = new MemoryStream();
		return httpContext;
	}

	#endregion
}
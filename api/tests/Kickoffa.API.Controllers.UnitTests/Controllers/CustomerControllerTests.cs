using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Application.MessageErrors;
using Kickoffa.API.AspNet.Infrastructure.ErrorHandling;
using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.Domain.ProcessResult;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Kickoffa.API.Controllers.UnitTests.Controllers;

/// <summary>
/// Testes unitários para CustomerController
/// </summary>
public class CustomerControllerTests
{
	private readonly CustomerController _controller;
	private readonly ICustomerService _customerService;
	private readonly IActionResultErrorHandler _actionResultErrorHandler;
	private readonly CancellationToken _cancellationToken;

	public CustomerControllerTests()
	{
		_customerService = Substitute.For<ICustomerService>();
		_actionResultErrorHandler = Substitute.For<IActionResultErrorHandler>();
		_controller = new CustomerController(_customerService, _actionResultErrorHandler);
		_cancellationToken = new CancellationToken();
	}

	#region UpdateAsync Tests

	[Fact]
	public async Task UpdateAsync_WithSuccessfulUpdate_ShouldReturnOkResult()
	{
		// Arrange
		var customerId = 1L;
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva",
			Email = "joao@email.com"
		};

		var expectedResponse = new CustomerResponse
		{
			Id = customerId,
			FirstName = "João",
			LastName = "Silva",
			Email = "joao@email.com",
			Type = CustomerType.NaturalPerson,
			CreatedDateUtc = DateTime.UtcNow,
			LastUpdatedDateUtc = DateTime.UtcNow,
		};

		var successResult = Result<CustomerResponse>.Success(expectedResponse);
		_customerService.UpdateAsync(customerId, request, _cancellationToken)
			.Returns(successResult);

		// Act
		var result = await _controller.UpdateAsync(customerId, request, _cancellationToken);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		var response = Assert.IsType<CustomerResponse>(okResult.Value);
		Assert.Equal(customerId, response.Id);
		Assert.Equal("João", response.FirstName);
		Assert.Equal("Silva", response.LastName);
	}

	[Fact]
	public async Task UpdateAsync_WithCustomerNotFound_ShouldReturnNotFound()
	{
		// Arrange
		var customerId = 999L;
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva"
		};

		var error = CustomerServiceErrors.CustomerNotFound(customerId);
		var failureResult = Result<CustomerResponse>.Failure(error);
		_customerService.UpdateAsync(customerId, request, _cancellationToken)
			.Returns(failureResult);

		var notFoundResult = new NotFoundObjectResult("Customer not found");
		_actionResultErrorHandler.GetActionResultFromError(error)
			.Returns(notFoundResult);

		// Act
		var result = await _controller.UpdateAsync(customerId, request, _cancellationToken);

		// Assert
		var actualNotFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
		Assert.Equal("Customer not found", actualNotFoundResult.Value);
	}

	[Fact]
	public async Task UpdateAsync_WithCustomerTypeCannotBeChanged_ShouldReturnBadRequest()
	{
		// Arrange
		var customerId = 1L;
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva"
		};

		var error = CustomerServiceErrors.CustomerTypeCannotBeChanged(customerId);
		var failureResult = Result<CustomerResponse>.Failure(error);
		_customerService.UpdateAsync(customerId, request, _cancellationToken)
			.Returns(failureResult);

		var badRequestResult = new BadRequestObjectResult("Customer type cannot be changed");
		_actionResultErrorHandler.GetActionResultFromError(error)
			.Returns(badRequestResult);

		// Act
		var result = await _controller.UpdateAsync(customerId, request, _cancellationToken);

		// Assert
		var actualBadRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
		Assert.Equal("Customer type cannot be changed", actualBadRequestResult.Value);
	}

	[Fact]
	public async Task UpdateAsync_WithIncompatibleRequestType_ShouldReturnBadRequest()
	{
		// Arrange
		var customerId = 1L;
		var request = new CreateLegalPersonRequest
		{
			Company = "Empresa ABC"
		};

		var error = CustomerServiceErrors.IncompatibleRequestType(customerId);
		var failureResult = Result<CustomerResponse>.Failure(error);
		_customerService.UpdateAsync(customerId, request, _cancellationToken)
			.Returns(failureResult);

		var badRequestResult = new BadRequestObjectResult("Incompatible request type");
		_actionResultErrorHandler.GetActionResultFromError(error)
			.Returns(badRequestResult);

		// Act
		var result = await _controller.UpdateAsync(customerId, request, _cancellationToken);

		// Assert
		var actualBadRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
		Assert.Equal("Incompatible request type", actualBadRequestResult.Value);
	}

	[Fact]
	public async Task UpdateAsync_WithInternalError_ShouldReturnInternalServerError()
	{
		// Arrange
		var customerId = 1L;
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva"
		};

		var error = new Error("INTERNAL_ERROR", "Erro interno do servidor", System.Net.HttpStatusCode.InternalServerError);
		var failureResult = Result<CustomerResponse>.Failure(error);
		_customerService.UpdateAsync(customerId, request, _cancellationToken)
			.Returns(failureResult);

		var internalServerErrorResult = new ObjectResult("Internal server error") { StatusCode = 500 };
		_actionResultErrorHandler.GetActionResultFromError(error)
			.Returns(internalServerErrorResult);

		// Act
		var result = await _controller.UpdateAsync(customerId, request, _cancellationToken);

		// Assert
		var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
		Assert.Equal(500, statusCodeResult.StatusCode);
		Assert.Equal("Internal server error", statusCodeResult.Value);
	}

	#endregion
}
using Kickoffa.API.Application.MessageErrors;
using System.Net;

namespace Kickoffa.API.Application.UnitTests.MessageErrors;

/// <summary>
/// Testes unitários para CustomerServiceErrors
/// </summary>
public class CustomerServiceErrorsTests
{
	#region CustomerNotFound Tests

	[Fact]
	public void CustomerNotFound_WithValidCustomerId_ShouldReturnCorrectError()
	{
		// Arrange
		var customerId = 123L;

		// Act
		var error = CustomerServiceErrors.CustomerNotFound(customerId);

		// Assert
		Assert.NotNull(error);
		Assert.Equal("CUSTOMER_NOT_FOUND", error.Code);
		Assert.Equal("Cliente com id 123 não encontrado", error.Message);
		Assert.Equal(HttpStatusCode.NotFound, error.HttpStatusCode);
		Assert.NotNull(error.Metadata);
		Assert.True(error.Metadata.ContainsKey("customerId"));
		Assert.Equal(customerId, error.Metadata["customerId"]);
	}

	[Theory]
	[InlineData(1L)]
	[InlineData(999L)]
	[InlineData(long.MaxValue)]
	[InlineData(long.MinValue)]
	public void CustomerNotFound_WithDifferentCustomerIds_ShouldReturnCorrectErrorWithId(long customerId)
	{
		// Act
		var error = CustomerServiceErrors.CustomerNotFound(customerId);

		// Assert
		Assert.NotNull(error);
		Assert.Equal("CUSTOMER_NOT_FOUND", error.Code);
		Assert.Equal($"Cliente com id {customerId} não encontrado", error.Message);
		Assert.Equal(HttpStatusCode.NotFound, error.HttpStatusCode);
		Assert.NotNull(error.Metadata);
		Assert.Equal(customerId, error.Metadata["customerId"]);
	}

	[Fact]
	public void CustomerNotFound_ShouldHaveCorrectToStringRepresentation()
	{
		// Arrange
		var customerId = 456L;

		// Act
		var error = CustomerServiceErrors.CustomerNotFound(customerId);

		// Assert
		Assert.Equal("CUSTOMER_NOT_FOUND: Cliente com id 456 não encontrado", error.ToString());
	}

	#endregion

	#region CustomerTypeCannotBeChanged Tests

	[Fact]
	public void CustomerTypeCannotBeChanged_WithValidCustomerId_ShouldReturnCorrectError()
	{
		// Arrange
		var customerId = 789L;

		// Act
		var error = CustomerServiceErrors.CustomerTypeCannotBeChanged(customerId);

		// Assert
		Assert.NotNull(error);
		Assert.Equal("CUSTOMER_TYPE_CANNOT_BE_CHANGED", error.Code);
		Assert.Equal("Não é possível alterar o tipo do cliente após a criação", error.Message);
		Assert.Equal(HttpStatusCode.BadRequest, error.HttpStatusCode);
		Assert.NotNull(error.Metadata);
		Assert.True(error.Metadata.ContainsKey("customerId"));
		Assert.Equal(customerId, error.Metadata["customerId"]);
	}

	[Theory]
	[InlineData(1L)]
	[InlineData(100L)]
	[InlineData(999L)]
	public void CustomerTypeCannotBeChanged_WithDifferentCustomerIds_ShouldReturnCorrectErrorWithId(long customerId)
	{
		// Act
		var error = CustomerServiceErrors.CustomerTypeCannotBeChanged(customerId);

		// Assert
		Assert.NotNull(error);
		Assert.Equal("CUSTOMER_TYPE_CANNOT_BE_CHANGED", error.Code);
		Assert.Equal("Não é possível alterar o tipo do cliente após a criação", error.Message);
		Assert.Equal(HttpStatusCode.BadRequest, error.HttpStatusCode);
		Assert.NotNull(error.Metadata);
		Assert.Equal(customerId, error.Metadata["customerId"]);
	}

	[Fact]
	public void CustomerTypeCannotBeChanged_ShouldHaveCorrectToStringRepresentation()
	{
		// Arrange
		var customerId = 321L;

		// Act
		var error = CustomerServiceErrors.CustomerTypeCannotBeChanged(customerId);

		// Assert
		Assert.Equal("CUSTOMER_TYPE_CANNOT_BE_CHANGED: Não é possível alterar o tipo do cliente após a criação", error.ToString());
	}

	#endregion

	#region IncompatibleRequestType Tests

	[Fact]
	public void IncompatibleRequestType_WithValidCustomerId_ShouldReturnCorrectError()
	{
		// Arrange
		var customerId = 654L;

		// Act
		var error = CustomerServiceErrors.IncompatibleRequestType(customerId);

		// Assert
		Assert.NotNull(error);
		Assert.Equal("INCOMPATIBLE_REQUEST_TYPE", error.Code);
		Assert.Equal("Tipo de request incompatível com o tipo do customer existente", error.Message);
		Assert.Equal(HttpStatusCode.BadRequest, error.HttpStatusCode);
		Assert.NotNull(error.Metadata);
		Assert.True(error.Metadata.ContainsKey("customerId"));
		Assert.Equal(customerId, error.Metadata["customerId"]);
	}

	[Theory]
	[InlineData(1L)]
	[InlineData(50L)]
	[InlineData(999L)]
	public void IncompatibleRequestType_WithDifferentCustomerIds_ShouldReturnCorrectErrorWithId(long customerId)
	{
		// Act
		var error = CustomerServiceErrors.IncompatibleRequestType(customerId);

		// Assert
		Assert.NotNull(error);
		Assert.Equal("INCOMPATIBLE_REQUEST_TYPE", error.Code);
		Assert.Equal("Tipo de request incompatível com o tipo do customer existente", error.Message);
		Assert.Equal(HttpStatusCode.BadRequest, error.HttpStatusCode);
		Assert.NotNull(error.Metadata);
		Assert.Equal(customerId, error.Metadata["customerId"]);
	}

	[Fact]
	public void IncompatibleRequestType_ShouldHaveCorrectToStringRepresentation()
	{
		// Arrange
		var customerId = 987L;

		// Act
		var error = CustomerServiceErrors.IncompatibleRequestType(customerId);

		// Assert
		Assert.Equal("INCOMPATIBLE_REQUEST_TYPE: Tipo de request incompatível com o tipo do customer existente", error.ToString());
	}

	#endregion

	#region Comparison Tests

	[Fact]
	public void AllErrorMethods_ShouldReturnDifferentErrorCodes()
	{
		// Arrange
		var customerId = 1L;

		// Act
		var customerNotFoundError = CustomerServiceErrors.CustomerNotFound(customerId);
		var customerTypeCannotBeChangedError = CustomerServiceErrors.CustomerTypeCannotBeChanged(customerId);
		var incompatibleRequestTypeError = CustomerServiceErrors.IncompatibleRequestType(customerId);

		// Assert
		Assert.NotEqual(customerNotFoundError.Code, customerTypeCannotBeChangedError.Code);
		Assert.NotEqual(customerNotFoundError.Code, incompatibleRequestTypeError.Code);
		Assert.NotEqual(customerTypeCannotBeChangedError.Code, incompatibleRequestTypeError.Code);
	}

	[Fact]
	public void AllErrorMethods_ShouldHaveCorrectHttpStatusCodes()
	{
		// Arrange
		var customerId = 1L;

		// Act
		var customerNotFoundError = CustomerServiceErrors.CustomerNotFound(customerId);
		var customerTypeCannotBeChangedError = CustomerServiceErrors.CustomerTypeCannotBeChanged(customerId);
		var incompatibleRequestTypeError = CustomerServiceErrors.IncompatibleRequestType(customerId);

		// Assert
		Assert.Equal(HttpStatusCode.NotFound, customerNotFoundError.HttpStatusCode);
		Assert.Equal(HttpStatusCode.BadRequest, customerTypeCannotBeChangedError.HttpStatusCode);
		Assert.Equal(HttpStatusCode.BadRequest, incompatibleRequestTypeError.HttpStatusCode);
	}

	[Fact]
	public void AllErrorMethods_ShouldIncludeCustomerIdInMetadata()
	{
		// Arrange
		var customerId = 42L;

		// Act
		var customerNotFoundError = CustomerServiceErrors.CustomerNotFound(customerId);
		var customerTypeCannotBeChangedError = CustomerServiceErrors.CustomerTypeCannotBeChanged(customerId);
		var incompatibleRequestTypeError = CustomerServiceErrors.IncompatibleRequestType(customerId);

		// Assert
		Assert.Equal(customerId, customerNotFoundError.Metadata!["customerId"]);
		Assert.Equal(customerId, customerTypeCannotBeChangedError.Metadata!["customerId"]);
		Assert.Equal(customerId, incompatibleRequestTypeError.Metadata!["customerId"]);
	}

	#endregion

	#region Edge Cases Tests

	[Fact]
	public void AllErrorMethods_WithZeroCustomerId_ShouldWork()
	{
		// Arrange
		var customerId = 0L;

		// Act
		var customerNotFoundError = CustomerServiceErrors.CustomerNotFound(customerId);
		var customerTypeCannotBeChangedError = CustomerServiceErrors.CustomerTypeCannotBeChanged(customerId);
		var incompatibleRequestTypeError = CustomerServiceErrors.IncompatibleRequestType(customerId);

		// Assert
		Assert.Equal(customerId, customerNotFoundError.Metadata!["customerId"]);
		Assert.Equal(customerId, customerTypeCannotBeChangedError.Metadata!["customerId"]);
		Assert.Equal(customerId, incompatibleRequestTypeError.Metadata!["customerId"]);
	}

	[Fact]
	public void AllErrorMethods_WithNegativeCustomerId_ShouldWork()
	{
		// Arrange
		var customerId = -1L;

		// Act
		var customerNotFoundError = CustomerServiceErrors.CustomerNotFound(customerId);
		var customerTypeCannotBeChangedError = CustomerServiceErrors.CustomerTypeCannotBeChanged(customerId);
		var incompatibleRequestTypeError = CustomerServiceErrors.IncompatibleRequestType(customerId);

		// Assert
		Assert.Equal(customerId, customerNotFoundError.Metadata!["customerId"]);
		Assert.Equal(customerId, customerTypeCannotBeChangedError.Metadata!["customerId"]);
		Assert.Equal(customerId, incompatibleRequestTypeError.Metadata!["customerId"]);
	}

	#endregion
}

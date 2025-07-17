using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Application.Services;
using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.Domain.Interfaces.Models.Customer;
using Kickoffa.API.Domain.Models.FreelancerCustomer;
using Kickoffa.API.Domain.Repositories;
using Kickoffa.API.TestUtils.InternalMember;
using NSubstitute;

namespace Kickoffa.API.Application.UnitTests.Services;

/// <summary>
/// Testes unitários para CustomerService
/// </summary>
public class CustomerServiceTests
{
	private const long OWNER_ID = 1L;

	private readonly ICustomerService _customerService;
	private readonly CancellationToken _cancellationToken;
	private readonly ICustomerRepository _customerRepository;
	private readonly ICreateCustomerFactory _createCustomerFactory;

	public CustomerServiceTests()
	{
		_cancellationToken = new CancellationToken();
		_customerRepository = Substitute.For<ICustomerRepository>();
		_createCustomerFactory = Substitute.For<ICreateCustomerFactory>();
		_customerService = new CustomerService(_customerRepository, _createCustomerFactory);
	}

	#region UpdateAsync Tests

	[Fact]
	public async Task UpdateAsync_WithValidNaturalPersonRequest_ShouldUpdateAndReturnCustomer()
	{
		// Arrange
		var customerId = 1L;
		var existingCustomer = new NaturalPerson(
			OWNER_ID,
			"João", "Silva", "12345678901",
			"11999999999", "Rua A, 123", "joao@email.com");

		existingCustomer.SetPrivatePropertyBackingField("Id", customerId);

		var updateRequest = new CreateNaturalPersonRequest
		{
			FirstName = "João Carlos",
			LastName = "Silva Santos",
			Email = "joao.carlos@email.com",
			PhoneNumber = "11888888888",
			Address = "Rua B, 456",
			Cpf = "12345678901"
		};

		_customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
			.Returns(existingCustomer);

		_customerRepository.SaveChangesAsync(Arg.Any<CancellationToken>())
			.Returns(1);

		// Act
		var result = await _customerService.UpdateAsync(customerId, updateRequest, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(customerId, result.Id);
		Assert.Equal("João Carlos", result.FirstName);
		Assert.Equal("Silva Santos", result.LastName);
		Assert.Equal("joao.carlos@email.com", result.Email);
		Assert.Equal("11888888888", result.PhoneNumber);
		Assert.Equal("Rua B, 456", result.Address);

		// Verificar que o customer foi atualizado
		Assert.Equal("João Carlos", existingCustomer.FirstName);
		Assert.Equal("Silva Santos", existingCustomer.LastName);
		Assert.Equal("joao.carlos@email.com", existingCustomer.Email);
		Assert.Equal("11888888888", existingCustomer.PhoneNumber);
		Assert.Equal("Rua B, 456", existingCustomer.Address);

		await _customerRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task UpdateAsync_WithValidLegalPersonRequest_ShouldUpdateAndReturnCustomer()
	{
		// Arrange
		var customerId = 2L;
		var existingCustomer = new LegalPerson(
			OWNER_ID, "Empresa ABC", "12345678000195",
			"11999999999", "Av. Principal, 100", "contato@empresa.com");

		existingCustomer.SetPrivatePropertyBackingField("Id", customerId);

		var updateRequest = new CreateLegalPersonRequest
		{
			Company = "Empresa ABC Ltda",
			Email = "novo@empresa.com",
			PhoneNumber = "11888888888",
			Address = "Av. Secundária, 200",
			Cnpj = "12345678000195"
		};

		_customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
			.Returns(existingCustomer);

		_customerRepository.SaveChangesAsync(Arg.Any<CancellationToken>())
			.Returns(1);

		// Act
		var result = await _customerService.UpdateAsync(customerId, updateRequest, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(customerId, result.Id);
		Assert.Equal("Empresa ABC Ltda", result.Company);
		Assert.Equal("novo@empresa.com", result.Email);
		Assert.Equal("11888888888", result.PhoneNumber);
		Assert.Equal("Av. Secundária, 200", result.Address);

		// Verificar que o customer foi atualizado
		Assert.Equal("novo@empresa.com", existingCustomer.Email);
		Assert.Equal("11888888888", existingCustomer.PhoneNumber);
		Assert.Equal("Av. Secundária, 200", existingCustomer.Address);

		await _customerRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task UpdateAsync_WithNonExistentCustomer_ShouldReturnNull()
	{
		// Arrange
		var customerId = 999L;
		var updateRequest = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva"
		};

		_customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
			.Returns((ICustomer?)null);

		// Act
		var result = await _customerService.UpdateAsync(customerId, updateRequest, _cancellationToken);

		// Assert
		Assert.Null(result);
		await _customerRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task UpdateAsync_WithNullRequest_ShouldThrowArgumentNullException()
	{
		// Arrange
		var customerId = 1L;

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(() =>
			_customerService.UpdateAsync(customerId, null!, _cancellationToken));
	}

	[Fact]
	public async Task UpdateAsync_WithDifferentCustomerType_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var customerId = 1L;

		var existingCustomer = new NaturalPerson(
			OWNER_ID,
			"João", "Silva", "12345678901",
			"11999999999", "Rua A, 123", "joao@email.com");

		existingCustomer.SetPrivatePropertyBackingField("Id", customerId);

		var updateRequest = new CreateLegalPersonRequest
		{
			Company = "Empresa ABC"
		};

		_customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
			.Returns(existingCustomer);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
			_customerService.UpdateAsync(customerId, updateRequest, _cancellationToken));

		Assert.Equal("Não é possível alterar o tipo do cliente após a criação", exception.Message);
		await _customerRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task UpdateAsync_WithIncompatibleRequestType_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var customerId = 1L;

		// Criar um request que não é nem NaturalPerson nem LegalPerson
		var existingCustomer = Substitute.For<INaturalPerson>();
		existingCustomer.Type.Returns(Domain.Models.Enums.CustomerType.NaturalPerson);

		var updateRequest = new CreateLegalPersonRequest
		{
			Company = "Empresa ABC"
		};

		_customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
			.Returns(existingCustomer);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
			_customerService.UpdateAsync(customerId, updateRequest, _cancellationToken));

		Assert.Equal("Não é possível alterar o tipo do cliente após a criação", exception.Message);
		await _customerRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task UpdateAsync_WithNaturalPersonAndNullOptionalFields_ShouldUpdateSuccessfully()
	{
		// Arrange
		var customerId = 1L;

		var existingCustomer = new NaturalPerson(
			OWNER_ID,
			"João", "Silva", "12345678901",
			"11999999999", "Rua A, 123", "joao@email.com");

		existingCustomer.SetPrivatePropertyBackingField("Id", customerId);

		var updateRequest = new CreateNaturalPersonRequest
		{
			FirstName = "João Carlos",
			LastName = "Silva Santos",
			Email = null, // Removendo email
			PhoneNumber = null, // Removendo telefone
			Address = null, // Removendo endereço
			Cpf = "12345678901"
		};

		_customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
			.Returns(existingCustomer);

		_customerRepository.SaveChangesAsync(Arg.Any<CancellationToken>())
			.Returns(1);

		// Act
		var result = await _customerService.UpdateAsync(customerId, updateRequest, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("João Carlos", result.FirstName);
		Assert.Equal("Silva Santos", result.LastName);
		Assert.Null(result.Email);
		Assert.Null(result.PhoneNumber);
		Assert.Null(result.Address);

		// Verificar que o customer foi atualizado com valores null
		Assert.Null(existingCustomer.Email);
		Assert.Null(existingCustomer.PhoneNumber);
		Assert.Null(existingCustomer.Address);
	}

	[Fact]
	public async Task UpdateAsync_WithLegalPersonAndNullOptionalFields_ShouldUpdateSuccessfully()
	{
		// Arrange
		var customerId = 2L;

		var existingCustomer = new LegalPerson(
			OWNER_ID, "Empresa ABC", "12345678000195",
			"11999999999", "Av. Principal, 100", "contato@empresa.com");

		existingCustomer.SetPrivatePropertyBackingField("Id", customerId);

		var updateRequest = new CreateLegalPersonRequest
		{
			Company = "Empresa ABC Ltda",
			Email = null, // Removendo email
			PhoneNumber = null, // Removendo telefone
			Address = null, // Removendo endereço
			Cnpj = "12345678000195"
		};

		_customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
			.Returns(existingCustomer);

		_customerRepository.SaveChangesAsync(Arg.Any<CancellationToken>())
			.Returns(1);

		// Act
		var result = await _customerService.UpdateAsync(customerId, updateRequest, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Empresa ABC Ltda", result.Company);
		Assert.Null(result.Email);
		Assert.Null(result.PhoneNumber);
		Assert.Null(result.Address);

		// Verificar que o customer foi atualizado com valores null
		Assert.Null(existingCustomer.Email);
		Assert.Null(existingCustomer.PhoneNumber);
		Assert.Null(existingCustomer.Address);
	}

	[Fact]
	public async Task UpdateAsync_WithEmptyStringFields_ShouldUpdateSuccessfully()
	{
		// Arrange
		var customerId = 1L;
		var existingCustomer = new NaturalPerson(
			OWNER_ID,
			"João", "Silva", "12345678901",
			"11999999999", "Rua A, 123", "joao@email.com");

		existingCustomer.SetPrivatePropertyBackingField("Id", customerId);

		var updateRequest = new CreateNaturalPersonRequest
		{
			FirstName = "João Carlos",
			LastName = "Silva Santos",
			Email = "", // String vazia
			PhoneNumber = "", // String vazia
			Address = "", // String vazia
			Cpf = "12345678901"
		};

		_customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
			.Returns(existingCustomer);

		_customerRepository.SaveChangesAsync(Arg.Any<CancellationToken>())
			.Returns(1);

		// Act
		var result = await _customerService.UpdateAsync(customerId, updateRequest, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("João Carlos", result.FirstName);
		Assert.Equal("Silva Santos", result.LastName);
		Assert.Equal("", result.Email);
		Assert.Equal("", result.PhoneNumber);
		Assert.Equal("", result.Address);

		// Verificar que o customer foi atualizado com strings vazias
		Assert.Equal("", existingCustomer.Email);
		Assert.Equal("", existingCustomer.PhoneNumber);
		Assert.Equal("", existingCustomer.Address);
	}

	[Theory]
	[InlineData(CustomerType.NaturalPerson, Domain.Models.Enums.CustomerType.LegalCompany)]
	[InlineData(CustomerType.LegalCompany, Domain.Models.Enums.CustomerType.NaturalPerson)]
	public async Task UpdateAsync_WithMismatchedCustomerTypes_ShouldThrowInvalidOperationException(
		CustomerType requestType,
		Domain.Models.Enums.CustomerType existingType)
	{
		// Arrange
		var customerId = 1L;
		ICustomer existingCustomer = existingType == Domain.Models.Enums.CustomerType.NaturalPerson
			? new NaturalPerson(
				  OWNER_ID,
				  "João", "Silva", "12345678901",
				  "11999999999", "Rua A, 123", "joao@email.com")
			: new LegalPerson(
				  OWNER_ID, "Empresa ABC", "12345678000195",
				  "11999999999", "Av. Principal, 100", "contato@empresa.com");

		CreateCustomerRequest updateRequest = requestType == CustomerType.NaturalPerson
			? new CreateNaturalPersonRequest { FirstName = "João", LastName = "Silva" }
			: new CreateLegalPersonRequest { Company = "Empresa ABC" };

		_customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
			.Returns(existingCustomer);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
			_customerService.UpdateAsync(customerId, updateRequest, _cancellationToken));

		Assert.Equal("Não é possível alterar o tipo do cliente após a criação", exception.Message);
		await _customerRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
	}

	#endregion
}
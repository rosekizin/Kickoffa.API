using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.UnitTests.Customer;

public class CreateCustomerRequestTests
{
	#region CreateNaturalPersonRequest Tests

	[Fact]
	public void CreateNaturalPersonRequest_WithValidData_ShouldPassValidation()
	{
		// Arrange
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva",
			Email = "joao@example.com",
			PhoneNumber = "11999999999",
			Address = "Rua das Flores, 123",
			Cpf = "16836736031"
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Empty(validationResults);
		Assert.Equal(CustomerType.NaturalPerson, request.Type);
	}

	[Fact]
	public void CreateNaturalPersonRequest_WithEmptyFirstName_ShouldFailValidation()
	{
		// Arrange
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "",
			LastName = "Silva",
			Email = "joao@example.com"
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O nome é obrigatório" && v.MemberNames.Contains("FirstName"));
	}

	[Fact]
	public void CreateNaturalPersonRequest_WithTooShortFirstName_ShouldFailValidation()
	{
		// Arrange
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "A", // 1 character
			LastName = "Silva",
			Email = "joao@example.com"
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O nome deve ter entre 2 e 100 caracteres" && v.MemberNames.Contains("FirstName"));
	}

	[Fact]
	public void CreateNaturalPersonRequest_WithTooLongFirstName_ShouldFailValidation()
	{
		// Arrange
		var longName = new string('A', 101); // 101 characters
		var request = new CreateNaturalPersonRequest
		{
			FirstName = longName,
			LastName = "Silva",
			Email = "joao@example.com"
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O nome deve ter entre 2 e 100 caracteres" && v.MemberNames.Contains("FirstName"));
	}

	[Fact]
	public void CreateNaturalPersonRequest_WithEmptyLastName_ShouldFailValidation()
	{
		// Arrange
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "",
			Email = "joao@example.com"
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O sobrenome é obrigatório" && v.MemberNames.Contains("LastName"));
	}

	[Fact]
	public void CreateNaturalPersonRequest_WithInvalidCpfLength_ShouldFailValidation()
	{
		// Arrange
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva",
			Cpf = "123456789" // 9 digits instead of 11
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O CPF deve ter exatamente 11 dígitos" && v.MemberNames.Contains("Cpf"));
	}

	[Fact]
	public void CreateNaturalPersonRequest_WithGreaterCpfLength_ShouldFailValidation()
	{
		// Arrange
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva",
			Cpf = "123.456.789-01" // Contains non-numeric characters and greater than 11 digits
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O CPF deve ter exatamente 11 dígitos" && v.MemberNames.Contains("Cpf"));
	}

	[Fact]
	public void CreateNaturalPersonRequest_WithInvalidCpf_ShouldFailValidation()
	{
		// Arrange
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva",
			Cpf = "11111111111" // Invalid CPF
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "CPF inválido" && v.MemberNames.Contains("Cpf"));
	}

	[Fact]
	public void CreateNaturalPersonRequest_WithNullCpf_ShouldPassValidation()
	{
		// Arrange
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva",
			Cpf = null
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Empty(validationResults);
	}

	[Fact]
	public void CreateNaturalPersonRequest_TypeShouldBeSetAutomatically()
	{
		// Arrange & Act
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva"
		};

		// Assert
		Assert.Equal(CustomerType.NaturalPerson, request.Type);
	}

	[Fact]
	public void CreateNaturalPersonRequest_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(CreateNaturalPersonRequest);

		// Act & Assert
		type.GetProperty(nameof(CreateNaturalPersonRequest.Id))!.AssertPropertyName("id").AssertRequired(Required.Default);
		type.GetProperty(nameof(CreateNaturalPersonRequest.Email))!.AssertPropertyName("email").AssertRequired(Required.Default);
		type.GetProperty(nameof(CreateNaturalPersonRequest.PhoneNumber))!.AssertPropertyName("phoneNumber").AssertRequired(Required.Default);
		type.GetProperty(nameof(CreateNaturalPersonRequest.Address))!.AssertPropertyName("address").AssertRequired(Required.Default);
		type.GetProperty(nameof(CreateNaturalPersonRequest.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
		type.GetProperty(nameof(CreateNaturalPersonRequest.FirstName))!.AssertPropertyName("firstName").AssertRequired(Required.Always);
		type.GetProperty(nameof(CreateNaturalPersonRequest.LastName))!.AssertPropertyName("lastName").AssertRequired(Required.Always);
		type.GetProperty(nameof(CreateNaturalPersonRequest.Cpf))!.AssertPropertyName("cpf").AssertRequired(Required.Always);
	}

	#endregion

	#region CreateLegalPersonRequest Tests

	[Fact]
	public void CreateLegalPersonRequest_WithValidData_ShouldPassValidation()
	{
		// Arrange
		var request = new CreateLegalPersonRequest
		{
			Company = "Empresa Teste Ltda",
			Email = "contato@empresa.com",
			PhoneNumber = "11999999999",
			Address = "Av. Paulista, 1000",
			Cnpj = "12345678000195"
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Empty(validationResults);
		Assert.Equal(CustomerType.LegalCompany, request.Type);
	}

	[Fact]
	public void CreateLegalPersonRequest_WithEmptyCompany_ShouldFailValidation()
	{
		// Arrange
		var request = new CreateLegalPersonRequest
		{
			Company = "",
			Email = "contato@empresa.com"
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O nome da empresa é obrigatório" && v.MemberNames.Contains("Company"));
	}

	[Fact]
	public void CreateLegalPersonRequest_WithTooShortCompany_ShouldFailValidation()
	{
		// Arrange
		var request = new CreateLegalPersonRequest
		{
			Company = "A", // 1 character
			Email = "contato@empresa.com"
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O nome da empresa deve ter entre 2 e 200 caracteres" && v.MemberNames.Contains("Company"));
	}

	[Fact]
	public void CreateLegalPersonRequest_WithTooLongCompany_ShouldFailValidation()
	{
		// Arrange
		var longCompany = new string('A', 201); // 201 characters
		var request = new CreateLegalPersonRequest
		{
			Company = longCompany,
			Email = "contato@empresa.com"
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O nome da empresa deve ter entre 2 e 200 caracteres" && v.MemberNames.Contains("Company"));
	}

	[Fact]
	public void CreateLegalPersonRequest_WithInvalidCnpjLength_ShouldFailValidation()
	{
		// Arrange
		var request = new CreateLegalPersonRequest
		{
			Company = "Empresa Teste",
			Cnpj = "123456789012" // 12 digits instead of 14
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O CNPJ deve ter exatamente 14 dígitos" && v.MemberNames.Contains("Cnpj"));
	}

	[Fact]
	public void CreateLegalPersonRequest_WithGreaterCnpjLength_ShouldFailValidation()
	{
		// Arrange
		var request = new CreateLegalPersonRequest
		{
			Company = "Empresa Teste",
			Cnpj = "12.345.678/0001-95" // Contains non-numeric characters and greater than 14 digits
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O CNPJ deve ter exatamente 14 dígitos" && v.MemberNames.Contains("Cnpj"));
	}

	[Fact]
	public void CreateLegalPersonRequest_WithInvalidCnpj_ShouldFailValidation()
	{
		// Arrange
		var request = new CreateLegalPersonRequest
		{
			Company = "Empresa Teste",
			Cnpj = "11111111111111" // Invalid CNPJ
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "CNPJ inválido" && v.MemberNames.Contains("Cnpj"));
	}

	[Fact]
	public void CreateLegalPersonRequest_WithNullCnpj_ShouldPassValidation()
	{
		// Arrange
		var request = new CreateLegalPersonRequest
		{
			Company = "Empresa Teste",
			Cnpj = null
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Empty(validationResults);
	}

	[Fact]
	public void CreateLegalPersonRequest_TypeShouldBeSetAutomatically()
	{
		// Arrange & Act
		var request = new CreateLegalPersonRequest
		{
			Company = "Empresa Teste"
		};

		// Assert
		Assert.Equal(CustomerType.LegalCompany, request.Type);
	}

	[Fact]
	public void CreateLegalPersonRequest_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(CreateLegalPersonRequest);

		// Act & Assert
		type.GetProperty(nameof(CreateLegalPersonRequest.Id))!.AssertPropertyName("id").AssertRequired(Required.Default);
		type.GetProperty(nameof(CreateLegalPersonRequest.Email))!.AssertPropertyName("email").AssertRequired(Required.Default);
		type.GetProperty(nameof(CreateLegalPersonRequest.PhoneNumber))!.AssertPropertyName("phoneNumber").AssertRequired(Required.Default);
		type.GetProperty(nameof(CreateLegalPersonRequest.Address))!.AssertPropertyName("address").AssertRequired(Required.Default);
		type.GetProperty(nameof(CreateLegalPersonRequest.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
		type.GetProperty(nameof(CreateLegalPersonRequest.Company))!.AssertPropertyName("company").AssertRequired(Required.Default);
		type.GetProperty(nameof(CreateLegalPersonRequest.Cnpj))!.AssertPropertyName("cnpj").AssertRequired(Required.Default);
	}

	#endregion

	#region Base Class Tests

	[Theory]
	[InlineData("test@example.com")]
	[InlineData("user.name@domain.co.uk")]
	[InlineData("test+tag@example.org")]
	public void CreateCustomerRequest_WithValidEmail_ShouldPassValidation(string email)
	{
		// Arrange
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva",
			Email = email
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Empty(validationResults);
	}

	[Fact]
	public void CreateCustomerRequest_WithInvalidEmail_ShouldFailValidation()
	{
		// Arrange
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva",
			Email = "invalid-email"
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "Email deve ter um formato válido" && v.MemberNames.Contains("Email"));
	}

	[Fact]
	public void CreateCustomerRequest_WithTooLongEmail_ShouldFailValidation()
	{
		// Arrange
		var longEmail = new string('a', 250) + "@example.com"; // 261 characters
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva",
			Email = longEmail
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O email deve ter no máximo 255 caracteres" && v.MemberNames.Contains("Email"));
	}

	[Fact]
	public void CreateCustomerRequest_WithTooLongPhoneNumber_ShouldFailValidation()
	{
		// Arrange
		var longPhone = new string('1', 21); // 21 characters
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva",
			PhoneNumber = longPhone
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O telefone deve ter no máximo 20 caracteres" && v.MemberNames.Contains("PhoneNumber"));
	}

	[Fact]
	public void CreateCustomerRequest_WithTooLongAddress_ShouldFailValidation()
	{
		// Arrange
		var longAddress = new string('A', 501); // 501 characters
		var request = new CreateNaturalPersonRequest
		{
			FirstName = "João",
			LastName = "Silva",
			Address = longAddress
		};

		// Act
		var validationContext = new ValidationContext(request);
		IEnumerable<ValidationResult> validationResults = request.Validate(validationContext);

		// Assert
		Assert.Single(validationResults);
		Assert.Contains(validationResults, v => v.ErrorMessage == "O endereço deve ter no máximo 500 caracteres" && v.MemberNames.Contains("Address"));
	}

	[Fact]
	public void CreateCustomerRequest_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(CreateCustomerRequest);

		// Act & Assert
		type.GetProperty(nameof(CreateCustomerRequest.Id))!.AssertPropertyName("id").AssertRequired(Required.Default);
		type.GetProperty(nameof(CreateCustomerRequest.Email))!.AssertPropertyName("email").AssertRequired(Required.Default);
		type.GetProperty(nameof(CreateCustomerRequest.PhoneNumber))!.AssertPropertyName("phoneNumber").AssertRequired(Required.Default);
		type.GetProperty(nameof(CreateCustomerRequest.Address))!.AssertPropertyName("address").AssertRequired(Required.Default);
		type.GetProperty(nameof(CreateCustomerRequest.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
	}

	#endregion

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
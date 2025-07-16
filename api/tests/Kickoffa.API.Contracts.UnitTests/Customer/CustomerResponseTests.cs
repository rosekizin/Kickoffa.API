using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.UnitTests.Customer;

public class CustomerResponseTests
{
    [Fact]
    public void CustomerResponse_WithRequiredProperties_ShouldCreateSuccessfully()
    {
        // Arrange
        var id = 1L;
        var type = CustomerType.NaturalPerson;
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;

        // Act
        var response = new CustomerResponse
        {
            Id = id,
            Type = type,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal(type, response.Type);
        Assert.Equal(createdDate, response.CreatedDateUtc);
        Assert.Equal(lastUpdatedDate, response.LastUpdatedDateUtc);
        Assert.Null(response.PhoneNumber);
        Assert.Null(response.Address);
        Assert.Null(response.Email);
        Assert.Null(response.FirstName);
        Assert.Null(response.LastName);
        Assert.Null(response.Cpf);
        Assert.Null(response.Company);
        Assert.Null(response.Cnpj);
    }

    [Fact]
    public void CustomerResponse_ForNaturalPerson_ShouldSetCorrectProperties()
    {
        // Arrange
        var id = 1L;
        var firstName = "João";
        var lastName = "Silva";
        var cpf = "12345678901";
        var email = "joao@example.com";
        var phoneNumber = "11999999999";
        var address = "Rua das Flores, 123";
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;

        // Act
        var response = new CustomerResponse
        {
            Id = id,
            Type = CustomerType.NaturalPerson,
            FirstName = firstName,
            LastName = lastName,
            Cpf = cpf,
            Email = email,
            PhoneNumber = phoneNumber,
            Address = address,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal(CustomerType.NaturalPerson, response.Type);
        Assert.Equal(firstName, response.FirstName);
        Assert.Equal(lastName, response.LastName);
        Assert.Equal(cpf, response.Cpf);
        Assert.Equal(email, response.Email);
        Assert.Equal(phoneNumber, response.PhoneNumber);
        Assert.Equal(address, response.Address);
        Assert.Equal(createdDate, response.CreatedDateUtc);
        Assert.Equal(lastUpdatedDate, response.LastUpdatedDateUtc);
        Assert.Null(response.Company);
        Assert.Null(response.Cnpj);
    }

    [Fact]
    public void CustomerResponse_ForLegalPerson_ShouldSetCorrectProperties()
    {
        // Arrange
        var id = 2L;
        var company = "Empresa Teste Ltda";
        var cnpj = "12345678000195";
        var email = "contato@empresa.com";
        var phoneNumber = "11999999999";
        var address = "Av. Paulista, 1000";
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;

        // Act
        var response = new CustomerResponse
        {
            Id = id,
            Type = CustomerType.LegalCompany,
            Company = company,
            Cnpj = cnpj,
            Email = email,
            PhoneNumber = phoneNumber,
            Address = address,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal(CustomerType.LegalCompany, response.Type);
        Assert.Equal(company, response.Company);
        Assert.Equal(cnpj, response.Cnpj);
        Assert.Equal(email, response.Email);
        Assert.Equal(phoneNumber, response.PhoneNumber);
        Assert.Equal(address, response.Address);
        Assert.Equal(createdDate, response.CreatedDateUtc);
        Assert.Equal(lastUpdatedDate, response.LastUpdatedDateUtc);
        Assert.Null(response.FirstName);
        Assert.Null(response.LastName);
        Assert.Null(response.Cpf);
    }

    [Fact]
    public void CustomerResponse_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;

        var response1 = new CustomerResponse
        {
            Id = 1L,
            Type = CustomerType.NaturalPerson,
            FirstName = "João",
            LastName = "Silva",
            Email = "joao@example.com",
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        var response2 = new CustomerResponse
        {
            Id = 1L,
            Type = CustomerType.NaturalPerson,
            FirstName = "João",
            LastName = "Silva",
            Email = "joao@example.com",
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Act & Assert
        Assert.Equal(response1, response2);
        Assert.True(response1 == response2);
        Assert.False(response1 != response2);
    }

    [Fact]
    public void CustomerResponse_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;

        var response1 = new CustomerResponse
        {
            Id = 1L,
            Type = CustomerType.NaturalPerson,
            FirstName = "João",
            LastName = "Silva",
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        var response2 = new CustomerResponse
        {
            Id = 2L,
            Type = CustomerType.NaturalPerson,
            FirstName = "João",
            LastName = "Silva",
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Act & Assert
        Assert.NotEqual(response1, response2);
        Assert.False(response1 == response2);
        Assert.True(response1 != response2);
    }

    [Fact]
    public void CustomerResponse_JsonSerialization_ShouldUseCorrectPropertyNames()
    {
        // Arrange
        var response = new CustomerResponse
        {
            Id = 1L,
            Type = CustomerType.NaturalPerson,
            FirstName = "João",
            LastName = "Silva",
            Cpf = "12345678901",
            Email = "joao@example.com",
            PhoneNumber = "11999999999",
            Address = "Rua das Flores, 123",
            CreatedDateUtc = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            LastUpdatedDateUtc = new DateTime(2024, 1, 2, 12, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var json = JsonConvert.SerializeObject(response);

        // Assert
        Assert.Contains("\"id\":", json);
        Assert.Contains("\"type\":", json);
        Assert.Contains("\"firstName\":", json);
        Assert.Contains("\"lastName\":", json);
        Assert.Contains("\"cpf\":", json);
        Assert.Contains("\"email\":", json);
        Assert.Contains("\"phoneNumber\":", json);
        Assert.Contains("\"address\":", json);
        Assert.Contains("\"createdDateUtc\":", json);
        Assert.Contains("\"lastUpdatedDateUtc\":", json);
    }

    [Fact]
    public void CustomerResponse_JsonDeserialization_ShouldWorkCorrectly()
    {
        // Arrange
        var json = @"{
            ""id"": 1,
            ""type"": 1,
            ""firstName"": ""João"",
            ""lastName"": ""Silva"",
            ""cpf"": ""12345678901"",
            ""email"": ""joao@example.com"",
            ""phoneNumber"": ""11999999999"",
            ""address"": ""Rua das Flores, 123"",
            ""createdDateUtc"": ""2024-01-01T12:00:00Z"",
            ""lastUpdatedDateUtc"": ""2024-01-02T12:00:00Z""
        }";

        // Act
        var response = JsonConvert.DeserializeObject<CustomerResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(1L, response.Id);
        Assert.Equal(CustomerType.NaturalPerson, response.Type);
        Assert.Equal("João", response.FirstName);
        Assert.Equal("Silva", response.LastName);
        Assert.Equal("12345678901", response.Cpf);
        Assert.Equal("joao@example.com", response.Email);
        Assert.Equal("11999999999", response.PhoneNumber);
        Assert.Equal("Rua das Flores, 123", response.Address);
        Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc), response.CreatedDateUtc);
        Assert.Equal(new DateTime(2024, 1, 2, 12, 0, 0, DateTimeKind.Utc), response.LastUpdatedDateUtc);
    }

    [Fact]
    public void CustomerResponse_JsonDeserialization_ForLegalPerson_ShouldWorkCorrectly()
    {
        // Arrange
        var json = @"{
            ""id"": 2,
            ""type"": 2,
            ""company"": ""Empresa Teste Ltda"",
            ""cnpj"": ""12345678000195"",
            ""email"": ""contato@empresa.com"",
            ""phoneNumber"": ""11999999999"",
            ""address"": ""Av. Paulista, 1000"",
            ""createdDateUtc"": ""2024-01-01T12:00:00Z"",
            ""lastUpdatedDateUtc"": ""2024-01-02T12:00:00Z""
        }";

        // Act
        var response = JsonConvert.DeserializeObject<CustomerResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2L, response.Id);
        Assert.Equal(CustomerType.LegalCompany, response.Type);
        Assert.Equal("Empresa Teste Ltda", response.Company);
        Assert.Equal("12345678000195", response.Cnpj);
        Assert.Equal("contato@empresa.com", response.Email);
        Assert.Equal("11999999999", response.PhoneNumber);
        Assert.Equal("Av. Paulista, 1000", response.Address);
        Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc), response.CreatedDateUtc);
        Assert.Equal(new DateTime(2024, 1, 2, 12, 0, 0, DateTimeKind.Utc), response.LastUpdatedDateUtc);
        Assert.Null(response.FirstName);
        Assert.Null(response.LastName);
        Assert.Null(response.Cpf);
    }

    [Theory]
    [InlineData(CustomerType.NaturalPerson)]
    [InlineData(CustomerType.LegalCompany)]
    public void CustomerResponse_WithDifferentCustomerTypes_ShouldSetCorrectly(CustomerType customerType)
    {
        // Arrange
        var createdDate = DateTime.UtcNow;
        var lastUpdatedDate = DateTime.UtcNow;

        // Act
        var response = new CustomerResponse
        {
            Id = 1L,
            Type = customerType,
            CreatedDateUtc = createdDate,
            LastUpdatedDateUtc = lastUpdatedDate
        };

        // Assert
        Assert.Equal(customerType, response.Type);
    }

    [Fact]
    public void CustomerResponse_WithAllOptionalPropertiesNull_ShouldBeValid()
    {
        // Arrange & Act
        var response = new CustomerResponse
        {
            Id = 1L,
            Type = CustomerType.NaturalPerson,
            CreatedDateUtc = DateTime.UtcNow,
            LastUpdatedDateUtc = DateTime.UtcNow,
            PhoneNumber = null,
            Address = null,
            Email = null,
            FirstName = null,
            LastName = null,
            Cpf = null,
            Company = null,
            Cnpj = null
        };

        // Assert
        Assert.Equal(1L, response.Id);
        Assert.Equal(CustomerType.NaturalPerson, response.Type);
        Assert.Null(response.PhoneNumber);
        Assert.Null(response.Address);
        Assert.Null(response.Email);
        Assert.Null(response.FirstName);
        Assert.Null(response.LastName);
        Assert.Null(response.Cpf);
        Assert.Null(response.Company);
        Assert.Null(response.Cnpj);
	}

	[Fact]
	public void CustomerResponse_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(CustomerResponse);

		// Act & Assert
		type.GetProperty(nameof(CustomerResponse.Id))!.AssertPropertyName("id").AssertRequired(Required.Always);
		type.GetProperty(nameof(CustomerResponse.PhoneNumber))!.AssertPropertyName("phoneNumber").AssertRequired(Required.Default);
		type.GetProperty(nameof(CustomerResponse.Address))!.AssertPropertyName("address").AssertRequired(Required.Default);
		type.GetProperty(nameof(CustomerResponse.Email))!.AssertPropertyName("email").AssertRequired(Required.Default);
		type.GetProperty(nameof(CustomerResponse.Type))!.AssertPropertyName("type").AssertRequired(Required.Always);
		type.GetProperty(nameof(CustomerResponse.CreatedDateUtc))!.AssertPropertyName("createdDateUtc").AssertRequired(Required.Always);
		type.GetProperty(nameof(CustomerResponse.LastUpdatedDateUtc))!.AssertPropertyName("lastUpdatedDateUtc").AssertRequired(Required.Always);
		type.GetProperty(nameof(CustomerResponse.FirstName))!.AssertPropertyName("firstName").AssertRequired(Required.Default);
		type.GetProperty(nameof(CustomerResponse.LastName))!.AssertPropertyName("lastName").AssertRequired(Required.Default);
		type.GetProperty(nameof(CustomerResponse.Cpf))!.AssertPropertyName("cpf").AssertRequired(Required.Default);
		type.GetProperty(nameof(CustomerResponse.Company))!.AssertPropertyName("company").AssertRequired(Required.Default);
		type.GetProperty(nameof(CustomerResponse.Cnpj))!.AssertPropertyName("cnpj").AssertRequired(Required.Default);
	}
}
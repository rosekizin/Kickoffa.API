using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.Contracts.Newtonsoft;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.UnitTests.Newtonsoft;

public class CustomerRequestConverterTests
{
    private readonly CustomerRequestConverter _converter;
    private readonly JsonSerializerSettings _settings;

    public CustomerRequestConverterTests()
    {
        _converter = new CustomerRequestConverter();
        _settings = new JsonSerializerSettings();
        _settings.Converters.Add(_converter);
    }

    [Fact]
    public void ReadJson_WithNaturalPersonCustomer_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = @"{
            ""type"": 1,
            ""firstName"": ""João"",
            ""lastName"": ""Silva"",
            ""email"": ""joao@example.com"",
            ""phoneNumber"": ""11999999999"",
            ""address"": ""Rua das Flores, 123"",
            ""cpf"": ""12345678901""
        }";

        // Act
        var result = JsonConvert.DeserializeObject<CreateCustomerRequest>(json, _settings);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreateNaturalPersonRequest>(result);
        var naturalPerson = (CreateNaturalPersonRequest)result;
        Assert.Equal(CustomerType.NaturalPerson, naturalPerson.Type);
        Assert.Equal("João", naturalPerson.FirstName);
        Assert.Equal("Silva", naturalPerson.LastName);
        Assert.Equal("joao@example.com", naturalPerson.Email);
        Assert.Equal("11999999999", naturalPerson.PhoneNumber);
        Assert.Equal("Rua das Flores, 123", naturalPerson.Address);
        Assert.Equal("12345678901", naturalPerson.Cpf);
    }

    [Fact]
    public void ReadJson_WithLegalPersonCustomer_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = @"{
            ""type"": 2,
            ""company"": ""Empresa Teste Ltda"",
            ""email"": ""contato@empresa.com"",
            ""phoneNumber"": ""11999999999"",
            ""address"": ""Av. Paulista, 1000"",
            ""cnpj"": ""12345678000195""
        }";

        // Act
        var result = JsonConvert.DeserializeObject<CreateCustomerRequest>(json, _settings);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreateLegalPersonRequest>(result);
        var legalPerson = (CreateLegalPersonRequest)result;
        Assert.Equal(CustomerType.LegalCompany, legalPerson.Type);
        Assert.Equal("Empresa Teste Ltda", legalPerson.Company);
        Assert.Equal("contato@empresa.com", legalPerson.Email);
        Assert.Equal("11999999999", legalPerson.PhoneNumber);
        Assert.Equal("Av. Paulista, 1000", legalPerson.Address);
        Assert.Equal("12345678000195", legalPerson.Cnpj);
    }

    [Fact]
    public void ReadJson_WithMissingType_ShouldThrowJsonSerializationException()
    {
        // Arrange
        var json = @"{
            ""firstName"": ""João"",
            ""lastName"": ""Silva"",
            ""email"": ""joao@example.com""
        }";

        // Act & Assert
        var exception = Assert.Throws<JsonSerializationException>(() =>
            JsonConvert.DeserializeObject<CreateCustomerRequest>(json, _settings));
        
        Assert.Contains("Campo 'type' obrigatório", exception.Message);
    }

    [Fact]
    public void ReadJson_WithInvalidType_ShouldThrowJsonSerializationException()
    {
        // Arrange
        var json = @"{
            ""type"": 999,
            ""firstName"": ""João"",
            ""lastName"": ""Silva""
        }";

        // Act & Assert
        var exception = Assert.Throws<JsonSerializationException>(() =>
            JsonConvert.DeserializeObject<CreateCustomerRequest>(json, _settings));
        
        Assert.Contains("Tipo de cliente", exception.Message);
        Assert.Contains("não suportado", exception.Message);
    }

    [Fact]
    public void WriteJson_WithNaturalPersonRequest_ShouldSerializeCorrectly()
    {
        // Arrange
        var request = new CreateNaturalPersonRequest
        {
            FirstName = "João",
            LastName = "Silva",
            Email = "joao@example.com",
            PhoneNumber = "11999999999",
            Address = "Rua das Flores, 123",
            Cpf = "12345678901"
        };

        // Act
        var json = JsonConvert.SerializeObject(request, _settings);

        // Assert
        Assert.Contains("\"type\":1", json); // NaturalPerson = 1
        Assert.Contains("\"firstName\":\"João\"", json);
        Assert.Contains("\"lastName\":\"Silva\"", json);
        Assert.Contains("\"email\":\"joao@example.com\"", json);
        Assert.Contains("\"cpf\":\"12345678901\"", json);
    }

    [Fact]
    public void WriteJson_WithLegalPersonRequest_ShouldSerializeCorrectly()
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
        var json = JsonConvert.SerializeObject(request, _settings);

        // Assert
        Assert.Contains("\"type\":2", json); // LegalCompany = 2
        Assert.Contains("\"company\":\"Empresa Teste Ltda\"", json);
        Assert.Contains("\"email\":\"contato@empresa.com\"", json);
        Assert.Contains("\"cnpj\":\"12345678000195\"", json);
    }

    [Fact]
    public void RoundTrip_WithBothCustomerTypes_ShouldPreserveData()
    {
        // Arrange
        var requests = new List<CreateCustomerRequest>
        {
            new CreateNaturalPersonRequest
            {
                FirstName = "João",
                LastName = "Silva",
                Email = "joao@example.com",
                Cpf = "12345678901"
            },
            new CreateLegalPersonRequest
            {
                Company = "Empresa Teste Ltda",
                Email = "contato@empresa.com",
                Cnpj = "12345678000195"
            }
        };

        foreach (var originalRequest in requests)
        {
            // Act
            var json = JsonConvert.SerializeObject(originalRequest, _settings);
            var deserializedRequest = JsonConvert.DeserializeObject<CreateCustomerRequest>(json, _settings);

            // Assert
            Assert.NotNull(deserializedRequest);
            Assert.Equal(originalRequest.GetType(), deserializedRequest.GetType());
            Assert.Equal(originalRequest.Type, deserializedRequest.Type);
            Assert.Equal(originalRequest.Email, deserializedRequest.Email);
        }
    }

    [Fact]
    public void WriteJson_WithNullRequest_ShouldSerializeNull()
    {
        // Arrange
        CreateCustomerRequest? request = null;

        // Act
        var json = JsonConvert.SerializeObject(request, _settings);

        // Assert
        Assert.Equal("null", json);
    }

    [Theory]
    [InlineData(1, typeof(CreateNaturalPersonRequest))]
    [InlineData(2, typeof(CreateLegalPersonRequest))]
    public void ReadJson_WithValidTypes_ShouldDeserializeToCorrectType(int typeValue, Type expectedType)
    {
        // Arrange
        var json = typeValue == 1 
            ? $@"{{
                ""type"": {typeValue},
                ""firstName"": ""Test"",
                ""lastName"": ""User""
            }}"
            : $@"{{
                ""type"": {typeValue},
                ""company"": ""Test Company""
            }}";

        // Act
        var result = JsonConvert.DeserializeObject<CreateCustomerRequest>(json, _settings);

        // Assert
        Assert.NotNull(result);
        Assert.IsType(expectedType, result);
        Assert.Equal((CustomerType)typeValue, result.Type);
    }
}

using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.Contracts.Newtonsoft;
using Newtonsoft.Json;
using NSubstitute;

namespace Kickoffa.API.Contracts.UnitTests.Newtonsoft;

public class ComponentRequestConverterTests
{
    /*
    private readonly ComponentRequestConverter _converter;
    private readonly JsonSerializerSettings _settings;

    public ComponentRequestConverterTests()
    {
        _converter = new ComponentRequestConverter();
        _settings = new JsonSerializerSettings();
        _settings.Converters.Add(_converter);
    }

    [Fact]
    public void ReadJson_WithTextComponent_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = @"{
            ""id"": 1,
            ""title"": ""Text Component"",
            ""type"": ""text"",
            ""isRequired"": true,
            ""order"": 1,
            ""placeholder"": ""Enter text"",
            ""maxLength"": 100
        }";

        // Act
        var result = JsonConvert.DeserializeObject<ComponentRequest>(json, _settings);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<TextComponentRequest>(result);
        var textComponent = (TextComponentRequest)result;
        Assert.Equal(1L, textComponent.Id);
        Assert.Equal("Text Component", textComponent.Title);
        Assert.Equal(ComponentTypeRequest.Text, textComponent.Type);
        Assert.True(textComponent.IsRequired);
        Assert.Equal(1, textComponent.Order);
        Assert.Equal("Enter text", textComponent.Placeholder);
        Assert.Equal(100, textComponent.MaxLength);
    }

    [Fact]
    public void ReadJson_WithCheckboxComponent_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = @"{
            ""id"": 2,
            ""title"": ""Checkbox Component"",
            ""type"": ""checkbox"",
            ""isRequired"": false,
            ""order"": 2
        }";

        // Act
        var result = JsonConvert.DeserializeObject<ComponentRequest>(json, _settings);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CheckboxComponentRequest>(result);
        var checkboxComponent = (CheckboxComponentRequest)result;
        Assert.Equal(2L, checkboxComponent.Id);
        Assert.Equal("Checkbox Component", checkboxComponent.Title);
        Assert.Equal(ComponentTypeRequest.Checkbox, checkboxComponent.Type);
        Assert.False(checkboxComponent.IsRequired);
        Assert.Equal(2, checkboxComponent.Order);
    }

    [Fact]
    public void ReadJson_WithUploadComponent_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = @"{
            ""id"": 3,
            ""title"": ""Upload Component"",
            ""type"": ""upload"",
            ""isRequired"": true,
            ""order"": 3,
            ""placeholder"": ""Drop files here"",
            ""allowedFileTypeIds"": [1, 2, 3]
        }";

        // Act
        var result = JsonConvert.DeserializeObject<ComponentRequest>(json, _settings);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<UploadComponentRequest>(result);
        var uploadComponent = (UploadComponentRequest)result;
        Assert.Equal(3L, uploadComponent.Id);
        Assert.Equal("Upload Component", uploadComponent.Title);
        Assert.Equal(ComponentTypeRequest.Upload, uploadComponent.Type);
        Assert.True(uploadComponent.IsRequired);
        Assert.Equal(3, uploadComponent.Order);
        Assert.Equal("Drop files here", uploadComponent.Placeholder);
        Assert.Equal(new List<long> { 1, 2, 3 }, uploadComponent.AllowedFileTypeIds);
    }

    [Fact]
    public void ReadJson_WithSignatureComponent_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = @"{
            ""id"": 4,
            ""title"": ""Signature Component"",
            ""type"": ""signature"",
            ""isRequired"": true,
            ""order"": 4
        }";

        // Act
        var result = JsonConvert.DeserializeObject<ComponentRequest>(json, _settings);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<SignatureComponentRequest>(result);
        var signatureComponent = (SignatureComponentRequest)result;
        Assert.Equal(4L, signatureComponent.Id);
        Assert.Equal("Signature Component", signatureComponent.Title);
        Assert.Equal(ComponentTypeRequest.Signature, signatureComponent.Type);
        Assert.True(signatureComponent.IsRequired);
        Assert.Equal(4, signatureComponent.Order);
    }

    [Fact]
    public void ReadJson_WithConfirmationComponent_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = @"{
            ""id"": 5,
            ""title"": ""Confirmation Component"",
            ""type"": ""confirmation"",
            ""isRequired"": true,
            ""order"": 5,
            ""confirmationText"": ""I agree to the terms""
        }";

        // Act
        var result = JsonConvert.DeserializeObject<ComponentRequest>(json, _settings);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ConfirmationComponentRequest>(result);
        var confirmationComponent = (ConfirmationComponentRequest)result;
        Assert.Equal(5L, confirmationComponent.Id);
        Assert.Equal("Confirmation Component", confirmationComponent.Title);
        Assert.Equal(ComponentTypeRequest.Confirmation, confirmationComponent.Type);
        Assert.True(confirmationComponent.IsRequired);
        Assert.Equal(5, confirmationComponent.Order);
        Assert.Equal("I agree to the terms", confirmationComponent.ConfirmationText);
    }

    [Theory]
    [InlineData("TEXT")]
    [InlineData("Text")]
    [InlineData("text")]
    [InlineData("tExT")]
    public void ReadJson_WithCaseInsensitiveType_ShouldDeserializeCorrectly(string typeValue)
    {
        // Arrange
        var json = $@"{{
            ""id"": 1,
            ""title"": ""Text Component"",
            ""type"": ""{typeValue}"",
            ""isRequired"": true,
            ""order"": 1
        }}";

        // Act
        var result = JsonConvert.DeserializeObject<ComponentRequest>(json, _settings);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<TextComponentRequest>(result);
    }

    [Fact]
    public void ReadJson_WithMissingType_ShouldThrowJsonSerializationException()
    {
        // Arrange
        var json = @"{
            ""id"": 1,
            ""title"": ""Component without type"",
            ""isRequired"": true,
            ""order"": 1
        }";

        // Act & Assert
        var exception = Assert.Throws<JsonSerializationException>(() =>
            JsonConvert.DeserializeObject<ComponentRequest>(json, _settings));
        
        Assert.Equal("Campo 'type' obrigatório para desserialização de ComponentRequest.", exception.Message);
    }

    [Fact]
    public void ReadJson_WithEmptyType_ShouldThrowJsonSerializationException()
    {
        // Arrange
        var json = @"{
            ""id"": 1,
            ""title"": ""Component with empty type"",
            ""type"": """",
            ""isRequired"": true,
            ""order"": 1
        }";

        // Act & Assert
        var exception = Assert.Throws<JsonSerializationException>(() =>
            JsonConvert.DeserializeObject<ComponentRequest>(json, _settings));
        
        Assert.Equal("Campo 'type' obrigatório para desserialização de ComponentRequest.", exception.Message);
    }

    [Fact]
    public void ReadJson_WithWhitespaceType_ShouldThrowJsonSerializationException()
    {
        // Arrange
        var json = @"{
            ""id"": 1,
            ""title"": ""Component with whitespace type"",
            ""type"": ""   "",
            ""isRequired"": true,
            ""order"": 1
        }";

        // Act & Assert
        var exception = Assert.Throws<JsonSerializationException>(() =>
            JsonConvert.DeserializeObject<ComponentRequest>(json, _settings));
        
        Assert.Equal("Campo 'type' obrigatório para desserialização de ComponentRequest.", exception.Message);
    }

    [Fact]
    public void ReadJson_WithUnsupportedType_ShouldThrowJsonSerializationException()
    {
        // Arrange
        var json = @"{
            ""id"": 1,
            ""title"": ""Component with unsupported type"",
            ""type"": ""unsupported"",
            ""isRequired"": true,
            ""order"": 1
        }";

        // Act & Assert
        var exception = Assert.Throws<JsonSerializationException>(() =>
            JsonConvert.DeserializeObject<ComponentRequest>(json, _settings));
        
        Assert.Equal("Tipo de componente 'unsupported' não suportado.", exception.Message);
    }

    [Fact]
    public void WriteJson_WithTextComponent_ShouldSerializeCorrectly()
    {
        // Arrange
        var component = new TextComponentRequest
        {
            Id = 1,
            Title = "Text Component",
            IsRequired = true,
            Order = 1,
            Placeholder = "Enter text",
            MaxLength = 100
        };

        // Act
        var json = JsonConvert.SerializeObject(component, _settings);

        // Assert
        Assert.Contains("\"id\":1", json);
        Assert.Contains("\"title\":\"Text Component\"", json);
        Assert.Contains("\"type\":1", json); // Enum serialized as number
        Assert.Contains("\"isRequired\":true", json);
        Assert.Contains("\"order\":1", json);
        Assert.Contains("\"placeholder\":\"Enter text\"", json);
        Assert.Contains("\"maxLength\":100", json);
    }

    [Fact]
    public void WriteJson_WithCheckboxComponent_ShouldSerializeCorrectly()
    {
        // Arrange
        var component = new CheckboxComponentRequest
        {
            Id = 2,
            Title = "Checkbox Component",
            IsRequired = false,
            Order = 2
        };

        // Act
        var json = JsonConvert.SerializeObject(component, _settings);

        // Assert
        Assert.Contains("\"id\":2", json);
        Assert.Contains("\"title\":\"Checkbox Component\"", json);
        Assert.Contains("\"type\":0", json); // Checkbox = 0
        Assert.Contains("\"isRequired\":false", json);
        Assert.Contains("\"order\":2", json);
    }

    [Fact]
    public void WriteJson_WithNullComponent_ShouldSerializeNull()
    {
        // Arrange
        ComponentRequest? component = null;

        // Act
        var json = JsonConvert.SerializeObject(component, _settings);

        // Assert
        Assert.Equal("null", json);
    }

    [Fact]
    public void RoundTrip_WithAllComponentTypes_ShouldPreserveData()
    {
        // Arrange
        var components = new List<ComponentRequest>
        {
            new TextComponentRequest
            {
                Id = 1,
                Title = "Text Component",
                IsRequired = true,
                Order = 1,
                Placeholder = "Enter text",
                MaxLength = 100
            },
            new CheckboxComponentRequest
            {
                Id = 2,
                Title = "Checkbox Component",
                IsRequired = false,
                Order = 2
            },
            new UploadComponentRequest
            {
                Id = 3,
                Title = "Upload Component",
                IsRequired = true,
                Order = 3,
                AllowedFileTypeIds = new List<long> { 1, 2 }
            },
            new SignatureComponentRequest
            {
                Id = 4,
                Title = "Signature Component",
                IsRequired = true,
                Order = 4
            },
            new ConfirmationComponentRequest
            {
                Id = 5,
                Title = "Confirmation Component",
                IsRequired = true,
                Order = 5,
                ConfirmationText = "I agree"
            }
        };

        foreach (var originalComponent in components)
        {
            // Act
            var json = JsonConvert.SerializeObject(originalComponent, _settings);
            var deserializedComponent = JsonConvert.DeserializeObject<ComponentRequest>(json, _settings);

            // Assert
            Assert.NotNull(deserializedComponent);
            Assert.Equal(originalComponent.GetType(), deserializedComponent.GetType());
            Assert.Equal(originalComponent.Id, deserializedComponent.Id);
            Assert.Equal(originalComponent.Title, deserializedComponent.Title);
            Assert.Equal(originalComponent.IsRequired, deserializedComponent.IsRequired);
            Assert.Equal(originalComponent.Order, deserializedComponent.Order);
        }
    }
    */
}

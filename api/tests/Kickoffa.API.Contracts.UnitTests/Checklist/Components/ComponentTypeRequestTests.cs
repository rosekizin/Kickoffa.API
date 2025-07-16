using Kickoffa.API.Contracts.Checklist.Components;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components;

public class ComponentTypeRequestTests
{
    [Fact]
    public void ComponentTypeRequest_ShouldHaveCorrectValues()
    {
        // Assert
        Assert.Equal(0, (int)ComponentTypeRequest.Checkbox);
        Assert.Equal(1, (int)ComponentTypeRequest.Text);
        Assert.Equal(2, (int)ComponentTypeRequest.Upload);
        Assert.Equal(3, (int)ComponentTypeRequest.Signature);
        Assert.Equal(4, (int)ComponentTypeRequest.Confirmation);
    }

    [Fact]
    public void ComponentTypeRequest_ShouldHaveCorrectNames()
    {
        // Assert
        Assert.Equal("Checkbox", ComponentTypeRequest.Checkbox.ToString());
        Assert.Equal("Text", ComponentTypeRequest.Text.ToString());
        Assert.Equal("Upload", ComponentTypeRequest.Upload.ToString());
        Assert.Equal("Signature", ComponentTypeRequest.Signature.ToString());
        Assert.Equal("Confirmation", ComponentTypeRequest.Confirmation.ToString());
    }

    [Theory]
    [InlineData(ComponentTypeRequest.Checkbox, 0)]
    [InlineData(ComponentTypeRequest.Text, 1)]
    [InlineData(ComponentTypeRequest.Upload, 2)]
    [InlineData(ComponentTypeRequest.Signature, 3)]
    [InlineData(ComponentTypeRequest.Confirmation, 4)]
    public void ComponentTypeRequest_ShouldCastToIntCorrectly(ComponentTypeRequest componentType, int expectedValue)
    {
        // Act
        var intValue = (int)componentType;

        // Assert
        Assert.Equal(expectedValue, intValue);
    }

    [Theory]
    [InlineData(0, ComponentTypeRequest.Checkbox)]
    [InlineData(1, ComponentTypeRequest.Text)]
    [InlineData(2, ComponentTypeRequest.Upload)]
    [InlineData(3, ComponentTypeRequest.Signature)]
    [InlineData(4, ComponentTypeRequest.Confirmation)]
    public void ComponentTypeRequest_ShouldCastFromIntCorrectly(int intValue, ComponentTypeRequest expectedComponentType)
    {
        // Act
        var componentType = (ComponentTypeRequest)intValue;

        // Assert
        Assert.Equal(expectedComponentType, componentType);
    }

    [Fact]
    public void ComponentTypeRequest_ShouldSupportEnumComparison()
    {
        // Arrange
        var type1 = ComponentTypeRequest.Checkbox;
        var type2 = ComponentTypeRequest.Checkbox;
        var type3 = ComponentTypeRequest.Text;

        // Assert
        Assert.Equal(type1, type2);
        Assert.NotEqual(type1, type3);
        Assert.True(type1 == type2);
        Assert.False(type1 == type3);
        Assert.False(type1 != type2);
        Assert.True(type1 != type3);
    }

    [Fact]
    public void ComponentTypeRequest_ShouldSupportEnumParsing()
    {
        // Act & Assert
        Assert.True(Enum.TryParse<ComponentTypeRequest>("Checkbox", out var checkbox));
        Assert.Equal(ComponentTypeRequest.Checkbox, checkbox);

        Assert.True(Enum.TryParse<ComponentTypeRequest>("Text", out var text));
        Assert.Equal(ComponentTypeRequest.Text, text);

        Assert.True(Enum.TryParse<ComponentTypeRequest>("Upload", out var upload));
        Assert.Equal(ComponentTypeRequest.Upload, upload);

        Assert.True(Enum.TryParse<ComponentTypeRequest>("Signature", out var signature));
        Assert.Equal(ComponentTypeRequest.Signature, signature);

        Assert.True(Enum.TryParse<ComponentTypeRequest>("Confirmation", out var confirmation));
        Assert.Equal(ComponentTypeRequest.Confirmation, confirmation);

        Assert.False(Enum.TryParse<ComponentTypeRequest>("InvalidType", out _));
    }

    [Fact]
    public void ComponentTypeRequest_ShouldSupportEnumParsingIgnoreCase()
    {
        // Act & Assert
        Assert.True(Enum.TryParse<ComponentTypeRequest>("checkbox", true, out var checkbox));
        Assert.Equal(ComponentTypeRequest.Checkbox, checkbox);

        Assert.True(Enum.TryParse<ComponentTypeRequest>("TEXT", true, out var text));
        Assert.Equal(ComponentTypeRequest.Text, text);

        Assert.True(Enum.TryParse<ComponentTypeRequest>("upload", true, out var upload));
        Assert.Equal(ComponentTypeRequest.Upload, upload);

        Assert.True(Enum.TryParse<ComponentTypeRequest>("SIGNATURE", true, out var signature));
        Assert.Equal(ComponentTypeRequest.Signature, signature);

        Assert.True(Enum.TryParse<ComponentTypeRequest>("confirmation", true, out var confirmation));
        Assert.Equal(ComponentTypeRequest.Confirmation, confirmation);
    }

    [Fact]
    public void ComponentTypeRequest_GetValues_ShouldReturnAllValues()
    {
        // Act
        var values = Enum.GetValues<ComponentTypeRequest>();

        // Assert
        Assert.Equal(5, values.Length);
        Assert.Contains(ComponentTypeRequest.Checkbox, values);
        Assert.Contains(ComponentTypeRequest.Text, values);
        Assert.Contains(ComponentTypeRequest.Upload, values);
        Assert.Contains(ComponentTypeRequest.Signature, values);
        Assert.Contains(ComponentTypeRequest.Confirmation, values);
    }

    [Fact]
    public void ComponentTypeRequest_GetNames_ShouldReturnAllNames()
    {
        // Act
        var names = Enum.GetNames<ComponentTypeRequest>();

        // Assert
        Assert.Equal(5, names.Length);
        Assert.Contains("Checkbox", names);
        Assert.Contains("Text", names);
        Assert.Contains("Upload", names);
        Assert.Contains("Signature", names);
        Assert.Contains("Confirmation", names);
    }

    [Fact]
    public void ComponentTypeRequest_IsDefined_ShouldWorkCorrectly()
    {
        // Assert
        Assert.True(Enum.IsDefined(typeof(ComponentTypeRequest), ComponentTypeRequest.Checkbox));
        Assert.True(Enum.IsDefined(typeof(ComponentTypeRequest), ComponentTypeRequest.Text));
        Assert.True(Enum.IsDefined(typeof(ComponentTypeRequest), ComponentTypeRequest.Upload));
        Assert.True(Enum.IsDefined(typeof(ComponentTypeRequest), ComponentTypeRequest.Signature));
        Assert.True(Enum.IsDefined(typeof(ComponentTypeRequest), ComponentTypeRequest.Confirmation));
        Assert.True(Enum.IsDefined(typeof(ComponentTypeRequest), 0));
        Assert.True(Enum.IsDefined(typeof(ComponentTypeRequest), 1));
        Assert.True(Enum.IsDefined(typeof(ComponentTypeRequest), 2));
        Assert.True(Enum.IsDefined(typeof(ComponentTypeRequest), 3));
        Assert.True(Enum.IsDefined(typeof(ComponentTypeRequest), 4));
        Assert.False(Enum.IsDefined(typeof(ComponentTypeRequest), 5));
        Assert.False(Enum.IsDefined(typeof(ComponentTypeRequest), -1));
        Assert.False(Enum.IsDefined(typeof(ComponentTypeRequest), 999));
    }

    [Theory]
    [InlineData("Checkbox")]
    [InlineData("Text")]
    [InlineData("Upload")]
    [InlineData("Signature")]
    [InlineData("Confirmation")]
    public void ComponentTypeRequest_IsDefined_WithStringNames_ShouldWorkCorrectly(string name)
    {
        // Assert
        Assert.True(Enum.IsDefined(typeof(ComponentTypeRequest), name));
    }

    [Theory]
    [InlineData("InvalidType")]
    [InlineData("")]
    [InlineData("checkbox")]
    [InlineData("TEXT")]
    [InlineData("Button")]
    [InlineData("Input")]
    public void ComponentTypeRequest_IsDefined_WithInvalidStringNames_ShouldReturnFalse(string name)
    {
        // Assert
        Assert.False(Enum.IsDefined(typeof(ComponentTypeRequest), name));
    }

    [Fact]
    public void ComponentTypeRequest_ShouldSupportSwitchStatement()
    {
        // Arrange
        var checkboxResult = GetComponentTypeDescription(ComponentTypeRequest.Checkbox);
        var textResult = GetComponentTypeDescription(ComponentTypeRequest.Text);
        var uploadResult = GetComponentTypeDescription(ComponentTypeRequest.Upload);
        var signatureResult = GetComponentTypeDescription(ComponentTypeRequest.Signature);
        var confirmationResult = GetComponentTypeDescription(ComponentTypeRequest.Confirmation);

        // Assert
        Assert.Equal("Checkbox Component", checkboxResult);
        Assert.Equal("Text Component", textResult);
        Assert.Equal("Upload Component", uploadResult);
        Assert.Equal("Signature Component", signatureResult);
        Assert.Equal("Confirmation Component", confirmationResult);
    }

    [Fact]
    public void ComponentTypeRequest_ShouldSupportHashCode()
    {
        // Arrange
        var type1 = ComponentTypeRequest.Checkbox;
        var type2 = ComponentTypeRequest.Checkbox;
        var type3 = ComponentTypeRequest.Text;

        // Assert
        Assert.Equal(type1.GetHashCode(), type2.GetHashCode());
        Assert.NotEqual(type1.GetHashCode(), type3.GetHashCode());
    }

    [Fact]
    public void ComponentTypeRequest_ShouldSupportDictionaryKeys()
    {
        // Arrange
        var dictionary = new Dictionary<ComponentTypeRequest, string>
        {
            { ComponentTypeRequest.Checkbox, "Checkbox Component" },
            { ComponentTypeRequest.Text, "Text Component" },
            { ComponentTypeRequest.Upload, "Upload Component" },
            { ComponentTypeRequest.Signature, "Signature Component" },
            { ComponentTypeRequest.Confirmation, "Confirmation Component" }
        };

        // Act & Assert
        Assert.Equal("Checkbox Component", dictionary[ComponentTypeRequest.Checkbox]);
        Assert.Equal("Text Component", dictionary[ComponentTypeRequest.Text]);
        Assert.Equal("Upload Component", dictionary[ComponentTypeRequest.Upload]);
        Assert.Equal("Signature Component", dictionary[ComponentTypeRequest.Signature]);
        Assert.Equal("Confirmation Component", dictionary[ComponentTypeRequest.Confirmation]);
        Assert.True(dictionary.ContainsKey(ComponentTypeRequest.Checkbox));
        Assert.True(dictionary.ContainsKey(ComponentTypeRequest.Text));
        Assert.True(dictionary.ContainsKey(ComponentTypeRequest.Upload));
        Assert.True(dictionary.ContainsKey(ComponentTypeRequest.Signature));
        Assert.True(dictionary.ContainsKey(ComponentTypeRequest.Confirmation));
    }

    [Fact]
    public void ComponentTypeRequest_ShouldHaveSequentialValues()
    {
        // Arrange
        var values = Enum.GetValues<ComponentTypeRequest>().Cast<int>().OrderBy(x => x).ToArray();

        // Assert
        for (int i = 0; i < values.Length; i++)
        {
            Assert.Equal(i, values[i]);
        }
    }

    [Theory]
    [InlineData(ComponentTypeRequest.Checkbox, ComponentTypeRequest.Text, true)]
    [InlineData(ComponentTypeRequest.Text, ComponentTypeRequest.Upload, true)]
    [InlineData(ComponentTypeRequest.Upload, ComponentTypeRequest.Signature, true)]
    [InlineData(ComponentTypeRequest.Signature, ComponentTypeRequest.Confirmation, true)]
    [InlineData(ComponentTypeRequest.Checkbox, ComponentTypeRequest.Confirmation, false)]
    public void ComponentTypeRequest_ShouldSupportComparison(ComponentTypeRequest first, ComponentTypeRequest second, bool firstIsLess)
    {
        // Act & Assert
        if (firstIsLess)
        {
            Assert.True(first < second);
            Assert.False(first > second);
        }
        else
        {
            Assert.False(first < second);
            Assert.True(first > second);
        }
    }

    #region Helper Methods

    private static string GetComponentTypeDescription(ComponentTypeRequest componentType)
    {
        return componentType switch
        {
            ComponentTypeRequest.Checkbox => "Checkbox Component",
            ComponentTypeRequest.Text => "Text Component",
            ComponentTypeRequest.Upload => "Upload Component",
            ComponentTypeRequest.Signature => "Signature Component",
            ComponentTypeRequest.Confirmation => "Confirmation Component",
            _ => "Unknown Component"
        };
    }

    #endregion
}

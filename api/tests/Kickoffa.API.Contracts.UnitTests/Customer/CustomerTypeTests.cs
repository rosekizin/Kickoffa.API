using Kickoffa.API.Contracts.Customer;

namespace Kickoffa.API.Contracts.UnitTests.Customer;

public class CustomerTypeTests
{
    [Fact]
    public void CustomerType_ShouldHaveCorrectValues()
    {
        // Assert
        Assert.Equal(1, (int)CustomerType.NaturalPerson);
        Assert.Equal(2, (int)CustomerType.LegalCompany);
    }

    [Fact]
    public void CustomerType_ShouldHaveCorrectNames()
    {
        // Assert
        Assert.Equal("NaturalPerson", CustomerType.NaturalPerson.ToString());
        Assert.Equal("LegalCompany", CustomerType.LegalCompany.ToString());
    }

    [Theory]
    [InlineData(CustomerType.NaturalPerson, 1)]
    [InlineData(CustomerType.LegalCompany, 2)]
    public void CustomerType_ShouldCastToIntCorrectly(CustomerType customerType, int expectedValue)
    {
        // Act
        var intValue = (int)customerType;

        // Assert
        Assert.Equal(expectedValue, intValue);
    }

    [Theory]
    [InlineData(1, CustomerType.NaturalPerson)]
    [InlineData(2, CustomerType.LegalCompany)]
    public void CustomerType_ShouldCastFromIntCorrectly(int intValue, CustomerType expectedCustomerType)
    {
        // Act
        var customerType = (CustomerType)intValue;

        // Assert
        Assert.Equal(expectedCustomerType, customerType);
    }

    [Fact]
    public void CustomerType_ShouldSupportEnumComparison()
    {
        // Arrange
        var type1 = CustomerType.NaturalPerson;
        var type2 = CustomerType.NaturalPerson;
        var type3 = CustomerType.LegalCompany;

        // Assert
        Assert.Equal(type1, type2);
        Assert.NotEqual(type1, type3);
        Assert.True(type1 == type2);
        Assert.False(type1 == type3);
        Assert.False(type1 != type2);
        Assert.True(type1 != type3);
    }

    [Fact]
    public void CustomerType_ShouldSupportEnumParsing()
    {
        // Act & Assert
        Assert.True(Enum.TryParse<CustomerType>("NaturalPerson", out var naturalPerson));
        Assert.Equal(CustomerType.NaturalPerson, naturalPerson);

        Assert.True(Enum.TryParse<CustomerType>("LegalCompany", out var legalCompany));
        Assert.Equal(CustomerType.LegalCompany, legalCompany);

        Assert.False(Enum.TryParse<CustomerType>("InvalidType", out _));
    }

    [Fact]
    public void CustomerType_ShouldSupportEnumParsingIgnoreCase()
    {
        // Act & Assert
        Assert.True(Enum.TryParse<CustomerType>("naturalperson", true, out var naturalPerson));
        Assert.Equal(CustomerType.NaturalPerson, naturalPerson);

        Assert.True(Enum.TryParse<CustomerType>("legalcompany", true, out var legalCompany));
        Assert.Equal(CustomerType.LegalCompany, legalCompany);

        Assert.True(Enum.TryParse<CustomerType>("NATURALPERSON", true, out var naturalPersonUpper));
        Assert.Equal(CustomerType.NaturalPerson, naturalPersonUpper);
    }

    [Fact]
    public void CustomerType_GetValues_ShouldReturnAllValues()
    {
        // Act
        var values = Enum.GetValues<CustomerType>();

        // Assert
        Assert.Equal(2, values.Length);
        Assert.Contains(CustomerType.NaturalPerson, values);
        Assert.Contains(CustomerType.LegalCompany, values);
    }

    [Fact]
    public void CustomerType_GetNames_ShouldReturnAllNames()
    {
        // Act
        var names = Enum.GetNames<CustomerType>();

        // Assert
        Assert.Equal(2, names.Length);
        Assert.Contains("NaturalPerson", names);
        Assert.Contains("LegalCompany", names);
    }

    [Fact]
    public void CustomerType_IsDefined_ShouldWorkCorrectly()
    {
        // Assert
        Assert.True(Enum.IsDefined(typeof(CustomerType), CustomerType.NaturalPerson));
        Assert.True(Enum.IsDefined(typeof(CustomerType), CustomerType.LegalCompany));
        Assert.True(Enum.IsDefined(typeof(CustomerType), 1));
        Assert.True(Enum.IsDefined(typeof(CustomerType), 2));
        Assert.False(Enum.IsDefined(typeof(CustomerType), 0));
        Assert.False(Enum.IsDefined(typeof(CustomerType), 3));
        Assert.False(Enum.IsDefined(typeof(CustomerType), 999));
    }

    [Theory]
    [InlineData("NaturalPerson")]
    [InlineData("LegalCompany")]
    public void CustomerType_IsDefined_WithStringNames_ShouldWorkCorrectly(string name)
    {
        // Assert
        Assert.True(Enum.IsDefined(typeof(CustomerType), name));
    }

    [Theory]
    [InlineData("InvalidType")]
    [InlineData("")]
    [InlineData("naturalperson")]
    [InlineData("NATURALPERSON")]
    public void CustomerType_IsDefined_WithInvalidStringNames_ShouldReturnFalse(string name)
    {
        // Assert
        Assert.False(Enum.IsDefined(typeof(CustomerType), name));
    }

    [Fact]
    public void CustomerType_ShouldSupportSwitchStatement()
    {
        // Arrange
        var naturalPersonResult = GetCustomerTypeDescription(CustomerType.NaturalPerson);
        var legalCompanyResult = GetCustomerTypeDescription(CustomerType.LegalCompany);

        // Assert
        Assert.Equal("Pessoa Física", naturalPersonResult);
        Assert.Equal("Pessoa Jurídica", legalCompanyResult);
    }

    [Fact]
    public void CustomerType_ShouldSupportHashCode()
    {
        // Arrange
        var type1 = CustomerType.NaturalPerson;
        var type2 = CustomerType.NaturalPerson;
        var type3 = CustomerType.LegalCompany;

        // Assert
        Assert.Equal(type1.GetHashCode(), type2.GetHashCode());
        Assert.NotEqual(type1.GetHashCode(), type3.GetHashCode());
    }

    [Fact]
    public void CustomerType_ShouldSupportDictionaryKeys()
    {
        // Arrange
        var dictionary = new Dictionary<CustomerType, string>
        {
            { CustomerType.NaturalPerson, "Pessoa Física" },
            { CustomerType.LegalCompany, "Pessoa Jurídica" }
        };

        // Act & Assert
        Assert.Equal("Pessoa Física", dictionary[CustomerType.NaturalPerson]);
        Assert.Equal("Pessoa Jurídica", dictionary[CustomerType.LegalCompany]);
        Assert.True(dictionary.ContainsKey(CustomerType.NaturalPerson));
        Assert.True(dictionary.ContainsKey(CustomerType.LegalCompany));
    }

    #region Helper Methods

    private static string GetCustomerTypeDescription(CustomerType customerType)
    {
        return customerType switch
        {
            CustomerType.NaturalPerson => "Pessoa Física",
            CustomerType.LegalCompany => "Pessoa Jurídica",
            _ => "Tipo Desconhecido"
        };
    }

    #endregion
}

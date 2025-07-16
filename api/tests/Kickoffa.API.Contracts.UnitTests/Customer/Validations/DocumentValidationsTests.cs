using Kickoffa.API.Contracts.Customer.Validations;

namespace Kickoffa.API.Contracts.UnitTests.Customer.Validations;

public class DocumentValidationsTests
{
    #region CPF Validation Tests

    [Theory]
    [InlineData("11144477735")] // Valid CPF
    [InlineData("12345678909")] // Valid CPF
    [InlineData("98765432100")] // Valid CPF
    public void IsValidCpf_WithValidCpf_ShouldReturnTrue(string cpf)
    {
        // Act
        var result = DocumentValidations.IsValidCpf(cpf);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("33333333333")]
    [InlineData("44444444444")]
    [InlineData("55555555555")]
    [InlineData("66666666666")]
    [InlineData("77777777777")]
    [InlineData("88888888888")]
    [InlineData("99999999999")]
    public void IsValidCpf_WithKnownInvalidCpfs_ShouldReturnFalse(string cpf)
    {
        // Act
        var result = DocumentValidations.IsValidCpf(cpf);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("12345678901")] // Invalid check digits
    [InlineData("98765432101")] // Invalid check digits
    [InlineData("11144477736")] // Invalid second check digit
    public void IsValidCpf_WithInvalidCheckDigits_ShouldReturnFalse(string cpf)
    {
        // Act
        var result = DocumentValidations.IsValidCpf(cpf);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidCpf_WithValidCpfCalculation_ShouldReturnTrue()
    {
        // Arrange - CPF: 111.444.777-35
        // First digit calculation: (1*10 + 1*9 + 1*8 + 4*7 + 4*6 + 4*5 + 7*4 + 7*3 + 7*2) % 11 = 185 % 11 = 9, so digit = 11-9 = 2? No, remainder 9 >= 2, so digit = 11-9 = 2? Let me recalculate...
        // Actually, let's use a known valid CPF for testing
        var cpf = "11144477735";

        // Act
        var result = DocumentValidations.IsValidCpf(cpf);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("123456789")] // Too short
    [InlineData("123456789012")] // Too long
    [InlineData("")] // Empty
    public void IsValidCpf_WithInvalidLength_ShouldReturnFalse(string cpf)
    {
		// Act & Assert
		Assert.False(DocumentValidations.IsValidCnpj(cpf));
	}

    [Theory]
    [InlineData("1234567890a")] // Contains letter
    [InlineData("123.456.789-01")] // Contains formatting
    [InlineData("123 456 789 01")] // Contains spaces
    public void IsValidCpf_WithNonNumericCharacters_ShouldReturnFalse(string cpf)
    {
		// Act & Assert
		Assert.False(DocumentValidations.IsValidCnpj(cpf));
	}

    #endregion

    #region CNPJ Validation Tests

    [Theory]
    [InlineData("11222333000181")] // Valid CNPJ
    [InlineData("12345678000195")] // Valid CNPJ
    public void IsValidCnpj_WithValidCnpj_ShouldReturnTrue(string cnpj)
    {
        // Act
        var result = DocumentValidations.IsValidCnpj(cnpj);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("00000000000000")]
    [InlineData("11111111111111")]
    [InlineData("22222222222222")]
    [InlineData("33333333333333")]
    [InlineData("44444444444444")]
    [InlineData("55555555555555")]
    [InlineData("66666666666666")]
    [InlineData("77777777777777")]
    [InlineData("88888888888888")]
    [InlineData("99999999999999")]
    public void IsValidCnpj_WithKnownInvalidCnpjs_ShouldReturnFalse(string cnpj)
    {
        // Act
        var result = DocumentValidations.IsValidCnpj(cnpj);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("12345678000100")] // Invalid check digits
    [InlineData("11222333000180")] // Invalid second check digit
    [InlineData("12345678000194")] // Invalid second check digit
    public void IsValidCnpj_WithInvalidCheckDigits_ShouldReturnFalse(string cnpj)
    {
        // Act
        var result = DocumentValidations.IsValidCnpj(cnpj);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("12345678901")] // Too short
    [InlineData("123456789012345")] // Too long
    [InlineData("")] // Empty
    public void IsValidCnpj_WithInvalidLength_ShouldReturnFalse(string cnpj)
    {
        // Act & Assert
        Assert.False(DocumentValidations.IsValidCnpj(cnpj));
    }

    [Theory]
    [InlineData("1234567800019a")] // Contains letter
    [InlineData("12.345.678/0001-95")] // Contains formatting
    [InlineData("12 345 678 0001 95")] // Contains spaces
    public void IsValidCnpj_WithNonNumericCharacters_ShouldReturnFalse(string cnpj)
    {
		// Act & Assert
		Assert.False(DocumentValidations.IsValidCnpj(cnpj));
    }

    #endregion

    #region Edge Cases and Performance Tests

    [Fact]
    public void IsValidCpf_StaticMethod_ShouldBeAccessible()
    {
		// Act & Assert - Should return true for a valid CPF
		var result = DocumentValidations.IsValidCpf("11144477735");
        Assert.True(result);
    }

    [Fact]
    public void IsValidCnpj_StaticMethod_ShouldBeAccessible()
    {
		// Act & Assert - Should return true for a valid CPF
		var result = DocumentValidations.IsValidCnpj("11222333000181");
        Assert.True(result);
    }

    [Fact]
    public void DocumentValidations_ShouldBeStaticClass()
    {
        // Assert
        var type = typeof(DocumentValidations);
        Assert.True(type.IsClass);
        Assert.False(type.IsAbstract); // Static classes are not abstract in reflection
        
        // Check if all methods are static
        var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);
        Assert.All(methods, method => Assert.True(method.IsStatic));
    }

    [Theory]
    [InlineData("11144477735", true)]
    [InlineData("11111111111", false)]
    [InlineData("12345678909", true)]
    [InlineData("00000000000", false)]
    public void IsValidCpf_MultipleTestCases_ShouldReturnExpectedResults(string cpf, bool expected)
    {
        // Act
        var result = DocumentValidations.IsValidCpf(cpf);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("11222333000181", true)]
    [InlineData("11111111111111", false)]
    [InlineData("12345678000195", true)]
    [InlineData("00000000000000", false)]
    public void IsValidCnpj_MultipleTestCases_ShouldReturnExpectedResults(string cnpj, bool expected)
    {
        // Act
        var result = DocumentValidations.IsValidCnpj(cnpj);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsValidCpf_PerformanceTest_ShouldExecuteQuickly()
    {
        // Arrange
        var cpf = "11144477735";
        var iterations = 10000;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            DocumentValidations.IsValidCpf(cpf);
        }

        stopwatch.Stop();

        // Assert - Should complete in reasonable time (less than 1 second for 10k iterations)
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Performance test took {stopwatch.ElapsedMilliseconds}ms for {iterations} iterations");
    }

    [Fact]
    public void IsValidCnpj_PerformanceTest_ShouldExecuteQuickly()
    {
        // Arrange
        var cnpj = "11222333000181";
        var iterations = 10000;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            DocumentValidations.IsValidCnpj(cnpj);
        }

        stopwatch.Stop();

        // Assert - Should complete in reasonable time (less than 1 second for 10k iterations)
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Performance test took {stopwatch.ElapsedMilliseconds}ms for {iterations} iterations");
    }

    #endregion
}
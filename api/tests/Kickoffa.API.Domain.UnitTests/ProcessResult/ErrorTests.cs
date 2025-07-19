using Kickoffa.API.Domain.Interfaces.ProcessResult;
using Kickoffa.API.Domain.ProcessResult;
using System.Net;

namespace Kickoffa.API.Domain.UnitTests.ProcessResult;

/// <summary>
/// Testes unitários para Error e IError
/// </summary>
public class ErrorTests
{
	#region Constructor Tests

	[Fact]
	public void Error_Constructor_WithValidParameters_ShouldCreateError()
	{
		// Arrange
		var code = "TEST_ERROR";
		var message = "Erro de teste";
		var httpStatusCode = HttpStatusCode.BadRequest;
		var metadata = new Dictionary<string, object> { { "key", "value" } };

		// Act
		var error = new Error(code, message, httpStatusCode, metadata);

		// Assert
		Assert.Equal(code, error.Code);
		Assert.Equal(message, error.Message);
		Assert.Equal(httpStatusCode, error.HttpStatusCode);
		Assert.Equal(metadata, error.Metadata);
	}

	[Fact]
	public void Error_Constructor_WithoutMetadata_ShouldCreateErrorWithNullMetadata()
	{
		// Arrange
		var code = "TEST_ERROR";
		var message = "Erro de teste";
		var httpStatusCode = HttpStatusCode.BadRequest;

		// Act
		var error = new Error(code, message, httpStatusCode);

		// Assert
		Assert.Equal(code, error.Code);
		Assert.Equal(message, error.Message);
		Assert.Equal(httpStatusCode, error.HttpStatusCode);
		Assert.Null(error.Metadata);
	}

	[Fact]
	public void Error_Constructor_WithNullMetadata_ShouldCreateErrorWithNullMetadata()
	{
		// Arrange
		var code = "TEST_ERROR";
		var message = "Erro de teste";
		var httpStatusCode = HttpStatusCode.BadRequest;

		// Act
		var error = new Error(code, message, httpStatusCode, null);

		// Assert
		Assert.Equal(code, error.Code);
		Assert.Equal(message, error.Message);
		Assert.Equal(httpStatusCode, error.HttpStatusCode);
		Assert.Null(error.Metadata);
	}

	[Fact]
	public void Error_Constructor_WithEmptyMetadata_ShouldCreateErrorWithEmptyMetadata()
	{
		// Arrange
		var code = "TEST_ERROR";
		var message = "Erro de teste";
		var httpStatusCode = HttpStatusCode.BadRequest;
		var metadata = new Dictionary<string, object>();

		// Act
		var error = new Error(code, message, httpStatusCode, metadata);

		// Assert
		Assert.Equal(code, error.Code);
		Assert.Equal(message, error.Message);
		Assert.Equal(httpStatusCode, error.HttpStatusCode);
		Assert.Equal(metadata, error.Metadata);
		Assert.Empty(error.Metadata!);
	}

	#endregion

	#region Property Tests

	[Theory]
	[InlineData("ERROR_CODE_1", "Mensagem de erro 1", HttpStatusCode.BadRequest)]
	[InlineData("ERROR_CODE_2", "Mensagem de erro 2", HttpStatusCode.NotFound)]
	[InlineData("ERROR_CODE_3", "Mensagem de erro 3", HttpStatusCode.InternalServerError)]
	[InlineData("ERROR_CODE_4", "Mensagem de erro 4", HttpStatusCode.Unauthorized)]
	[InlineData("ERROR_CODE_5", "Mensagem de erro 5", HttpStatusCode.Forbidden)]
	public void Error_Properties_ShouldReturnCorrectValues(string code, string message, HttpStatusCode httpStatusCode)
	{
		// Act
		var error = new Error(code, message, httpStatusCode);

		// Assert
		Assert.Equal(code, error.Code);
		Assert.Equal(message, error.Message);
		Assert.Equal(httpStatusCode, error.HttpStatusCode);
	}

	[Fact]
	public void Error_Code_ShouldBeSettableOnInit()
	{
		// Arrange & Act
		var error = new Error("INITIAL_CODE", "Message", HttpStatusCode.BadRequest)
		{
			Code = "NEW_CODE"
		};

		// Assert
		Assert.Equal("NEW_CODE", error.Code);
	}

	[Fact]
	public void Error_Message_ShouldBeSettableOnInit()
	{
		// Arrange & Act
		var error = new Error("CODE", "Initial Message", HttpStatusCode.BadRequest)
		{
			Message = "New Message"
		};

		// Assert
		Assert.Equal("New Message", error.Message);
	}

	[Fact]
	public void Error_HttpStatusCode_ShouldBeSettableOnInit()
	{
		// Arrange & Act
		var error = new Error("CODE", "Message", HttpStatusCode.BadRequest)
		{
			HttpStatusCode = HttpStatusCode.NotFound
		};

		// Assert
		Assert.Equal(HttpStatusCode.NotFound, error.HttpStatusCode);
	}

	[Fact]
	public void Error_Metadata_ShouldBeSettableOnInit()
	{
		// Arrange
		var initialMetadata = new Dictionary<string, object> { { "key1", "value1" } };
		var newMetadata = new Dictionary<string, object> { { "key2", "value2" } };

		// Act
		var error = new Error("CODE", "Message", HttpStatusCode.BadRequest, initialMetadata)
		{
			Metadata = newMetadata
		};

		// Assert
		Assert.Equal(newMetadata, error.Metadata);
		Assert.NotEqual(initialMetadata, error.Metadata);
	}

	#endregion

	#region ToString Tests

	[Fact]
	public void Error_ToString_ShouldReturnCodeAndMessage()
	{
		// Arrange
		var code = "TEST_ERROR";
		var message = "Erro de teste";
		var error = new Error(code, message, HttpStatusCode.BadRequest);

		// Act
		var result = error.ToString();

		// Assert
		Assert.Equal("TEST_ERROR: Erro de teste", result);
	}

	[Theory]
	[InlineData("", "Mensagem")]
	[InlineData("CODIGO", "")]
	[InlineData("", "")]
	public void Error_ToString_WithEmptyCodeOrMessage_ShouldHandleGracefully(string code, string message)
	{
		// Arrange
		var error = new Error(code, message, HttpStatusCode.BadRequest);

		// Act
		var result = error.ToString();

		// Assert
		Assert.Equal($"{code}: {message}", result);
	}

	[Fact]
	public void Error_ToString_WithSpecialCharacters_ShouldHandleCorrectly()
	{
		// Arrange
		var code = "ERROR_WITH_SPECIAL_CHARS";
		var message = "Erro com caracteres especiais: áéíóú çñü @#$%";
		var error = new Error(code, message, HttpStatusCode.BadRequest);

		// Act
		var result = error.ToString();

		// Assert
		Assert.Equal($"{code}: {message}", result);
	}

	#endregion

	#region Metadata Tests

	[Fact]
	public void Error_WithComplexMetadata_ShouldStoreCorrectly()
	{
		// Arrange
		var metadata = new Dictionary<string, object>
		{
			{ "string", "value" },
			{ "int", 42 },
			{ "bool", true },
			{ "datetime", DateTime.Now },
			{ "list", new List<string> { "item1", "item2" } },
			{ "object", new { Property = "value" } }
		};

		// Act
		var error = new Error("COMPLEX_ERROR", "Erro complexo", HttpStatusCode.BadRequest, metadata);

		// Assert
		Assert.Equal(metadata, error.Metadata);
		Assert.Equal("value", error.Metadata!["string"]);
		Assert.Equal(42, error.Metadata["int"]);
		Assert.Equal(true, error.Metadata["bool"]);
		Assert.IsType<DateTime>(error.Metadata["datetime"]);
		Assert.IsType<List<string>>(error.Metadata["list"]);
	}

	[Fact]
	public void Error_WithNullValuesInMetadata_ShouldHandleCorrectly()
	{
		// Arrange
		var metadata = new Dictionary<string, object>
		{
			{ "nullValue", null! },
			{ "stringValue", "test" }
		};

		// Act
		var error = new Error("NULL_METADATA_ERROR", "Erro com metadata null", HttpStatusCode.BadRequest, metadata);

		// Assert
		Assert.Equal(metadata, error.Metadata);
		Assert.Null(error.Metadata!["nullValue"]);
		Assert.Equal("test", error.Metadata["stringValue"]);
	}

	#endregion

	#region Interface Implementation Tests

	[Fact]
	public void Error_ShouldImplementIError()
	{
		// Arrange
		var error = new Error("TEST_ERROR", "Erro de teste", HttpStatusCode.BadRequest);

		// Act & Assert
		Assert.IsAssignableFrom<IError>(error);
	}

	[Fact]
	public void Error_AsIError_ShouldExposeAllProperties()
	{
		// Arrange
		var code = "TEST_ERROR";
		var message = "Erro de teste";
		var httpStatusCode = HttpStatusCode.BadRequest;
		var metadata = new Dictionary<string, object> { { "key", "value" } };
		var error = new Error(code, message, httpStatusCode, metadata);

		// Act
		IError iError = error;

		// Assert
		Assert.Equal(code, iError.Code);
		Assert.Equal(message, iError.Message);
		Assert.Equal(httpStatusCode, iError.HttpStatusCode);
		Assert.Equal(metadata, iError.Metadata);
		Assert.Equal("TEST_ERROR: Erro de teste", iError.ToString());
	}

	#endregion

	#region Edge Cases Tests

	[Theory]
	[InlineData(HttpStatusCode.Continue)]
	[InlineData(HttpStatusCode.SwitchingProtocols)]
	[InlineData(HttpStatusCode.OK)]
	[InlineData(HttpStatusCode.Created)]
	[InlineData(HttpStatusCode.Accepted)]
	[InlineData(HttpStatusCode.MovedPermanently)]
	[InlineData(HttpStatusCode.Found)]
	[InlineData(HttpStatusCode.BadRequest)]
	[InlineData(HttpStatusCode.Unauthorized)]
	[InlineData(HttpStatusCode.Forbidden)]
	[InlineData(HttpStatusCode.NotFound)]
	[InlineData(HttpStatusCode.MethodNotAllowed)]
	[InlineData(HttpStatusCode.InternalServerError)]
	[InlineData(HttpStatusCode.NotImplemented)]
	[InlineData(HttpStatusCode.BadGateway)]
	[InlineData(HttpStatusCode.ServiceUnavailable)]
	public void Error_WithDifferentHttpStatusCodes_ShouldWorkCorrectly(HttpStatusCode statusCode)
	{
		// Act
		var error = new Error("TEST_ERROR", "Erro de teste", statusCode);

		// Assert
		Assert.Equal(statusCode, error.HttpStatusCode);
	}

	[Fact]
	public void Error_WithVeryLongCodeAndMessage_ShouldHandleCorrectly()
	{
		// Arrange
		var longCode = new string('A', 1000);
		var longMessage = new string('B', 10000);

		// Act
		var error = new Error(longCode, longMessage, HttpStatusCode.BadRequest);

		// Assert
		Assert.Equal(longCode, error.Code);
		Assert.Equal(longMessage, error.Message);
		Assert.Equal($"{longCode}: {longMessage}", error.ToString());
	}

	#endregion
}
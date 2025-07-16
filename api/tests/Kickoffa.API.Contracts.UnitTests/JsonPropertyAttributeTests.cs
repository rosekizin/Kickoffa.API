using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.TestUtils.JsonProperty;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.UnitTests;

/// <summary>
/// Testes para verificar os atributos JsonProperty das classes Request e Response
/// </summary>
public class JsonPropertyAttributeTests
{

	[Fact]
	public void FileTypeSizeConfigResponse_ShouldHaveCorrectJsonPropertyAttributes()
	{
		// Arrange
		var type = typeof(FileTypeSizeConfigResponse);

		// Act & Assert
		type.GetProperty(nameof(FileTypeSizeConfigResponse.Id))!.AssertPropertyName("id").AssertRequired(Required.Always);
		type.GetProperty(nameof(FileTypeSizeConfigResponse.UploadComponentId))!.AssertPropertyName("uploadComponentId").AssertRequired(Required.Always);
		type.GetProperty(nameof(FileTypeSizeConfigResponse.FileTypeId))!.AssertPropertyName("fileTypeId").AssertRequired(Required.Always);
		type.GetProperty(nameof(FileTypeSizeConfigResponse.MaxSizeMB))!.AssertPropertyName("maxSizeMB").AssertRequired(Required.Always);
		type.GetProperty(nameof(FileTypeSizeConfigResponse.CreatedDateUtc))!.AssertPropertyName("createdDateUtc").AssertRequired(Required.Always);
		type.GetProperty(nameof(FileTypeSizeConfigResponse.LastUpdatedDateUtc))!.AssertPropertyName("lastUpdatedDateUtc").AssertRequired(Required.Always);
	}
}
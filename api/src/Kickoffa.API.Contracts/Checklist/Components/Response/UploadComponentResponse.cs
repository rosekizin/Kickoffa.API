using Kickoffa.API.Contracts.Checklist.Components.Request;
using Kickoffa.API.Contracts.FileType;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.Checklist.Components.Response
{
	/// <summary>
	/// Response para componente de upload
	/// </summary>
	public sealed record UploadComponentResponse : ComponentResponse
	{
		[JsonProperty(PropertyName = "placeholder", Required = Required.Default)]
		public string? Placeholder { get; init; }

		[JsonProperty(PropertyName = "allowedFileTypes", Required = Required.Always)]
		public ICollection<FileTypeResponse>? AllowedFileTypes { get; init; }

		[JsonProperty(PropertyName = "fileTypeSizeConfigs", Required = Required.Always)]
		public ICollection<FileTypeSizeConfigResponse>? FileTypeSizeConfigs { get; init; }

		[JsonProperty(PropertyName = "componentFiles", Required = Required.Default)]
		public ICollection<UploadComponentFileResponse>? ComponentFiles { get; init; }

		public UploadComponentResponse()
		{
			Type = "upload";
		}
	}
}
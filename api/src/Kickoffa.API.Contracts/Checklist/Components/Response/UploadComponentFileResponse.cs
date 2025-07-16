using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kickoffa.API.Contracts.Checklist.Components.Response
{
	/// <summary>
	/// Response de um arquivo enviado pelo cliente (relacionado ao componente)
	/// </summary>
	public sealed record UploadComponentFileResponse
	{
		[JsonProperty(PropertyName = "id", Required = Required.Always)]
		public required long Id { get; init; }

		[JsonProperty(PropertyName = "componentId", Required = Required.Always)]
		public required long ComponentId { get; init; }

		[JsonProperty(PropertyName = "fileName", Required = Required.Always)]
		public required string FileName { get; init; }

		[JsonProperty(PropertyName = "originalName", Required = Required.Always)]
		public required string OriginalName { get; init; }

		[JsonProperty(PropertyName = "mimeType", Required = Required.Always)]
		public required string MimeType { get; init; }

		[JsonProperty(PropertyName = "size", Required = Required.Always)]
		public required long Size { get; init; }

		[JsonProperty(PropertyName = "url", Required = Required.Always)]
		public required string Url { get; init; }

		[JsonProperty(PropertyName = "createdDateUtc", Required = Required.Always)]
		public required DateTime CreatedDateUtc { get; init; }
	}
}
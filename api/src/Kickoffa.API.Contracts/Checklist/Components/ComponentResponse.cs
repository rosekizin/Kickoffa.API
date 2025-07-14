using Kickoffa.API.Contracts.FileType;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.Checklist.Components;

/// <summary>
/// Classe base para responses de componentes
/// </summary>
public abstract record ComponentResponse
{
	[JsonProperty(PropertyName = "id", Required = Required.Always)]
	public required long Id { get; init; }

	[JsonProperty(PropertyName = "sectionId", Required = Required.Always)]
	public required long SectionId { get; init; }

	[JsonProperty(PropertyName = "title", Required = Required.Always)]
	public required string Title { get; init; }

	[JsonProperty(PropertyName = "description", Required = Required.Default)]
	public string? Description { get; init; }

	[JsonProperty(PropertyName = "type", Required = Required.Always)]
	public required string Type { get; init; }

	[JsonProperty(PropertyName = "isRequired", Required = Required.Always)]
	public required bool IsRequired { get; init; }

	[JsonProperty(PropertyName = "order", Required = Required.Always)]
	public required int Order { get; init; }

	[JsonProperty(PropertyName = "status", Required = Required.Default)]
	public ComponentStatusResponse? Status { get; init; }

	[JsonProperty(PropertyName = "createdDateUtc", Required = Required.Always)]
	public required DateTime CreatedDateUtc { get; init; }

	[JsonProperty(PropertyName = "lastUpdatedDateUtc", Required = Required.Always)]
	public required DateTime LastUpdatedDateUtc { get; init; }
}

/// <summary>
/// Response para componente de checkbox
/// </summary>
public sealed record CheckboxComponentResponse : ComponentResponse
{
    public CheckboxComponentResponse()
    {
        Type = "checkbox";
    }
}

/// <summary>
/// Response para componente de texto
/// </summary>
public sealed record TextComponentResponse : ComponentResponse
{
	[JsonProperty(PropertyName = "placeholder", Required = Required.Default)]
	public string? Placeholder { get; init; }

	[JsonProperty(PropertyName = "maxLength", Required = Required.Default)]
	public int? MaxLength { get; init; }
    
    public TextComponentResponse()
    {
        Type = "text";
    }
}

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

/// <summary>
/// Response para componente de assinatura
/// </summary>
public sealed record SignatureComponentResponse : ComponentResponse
{
    public SignatureComponentResponse()
    {
        Type = "signature";
    }
}

/// <summary>
/// Response para componente de confirmação
/// </summary>
public sealed record ConfirmationComponentResponse : ComponentResponse
{
	[JsonProperty(PropertyName = "confirmationText", Required = Required.Default)]
	public string? ConfirmationText { get; init; }
    
    public ConfirmationComponentResponse()
    {
        Type = "confirmation";
    }
}

/// <summary>
/// Response do status de um componente
/// </summary>
public sealed record ComponentStatusResponse
{
	[JsonProperty(PropertyName = "id", Required = Required.Always)]
	public required long Id { get; init; }

	[JsonProperty(PropertyName = "componentId", Required = Required.Always)]
	public required long ComponentId { get; init; }

	[JsonProperty(PropertyName = "isCompleted", Required = Required.Always)]
	public required bool IsCompleted { get; init; }

	[JsonProperty(PropertyName = "completedAt", Required = Required.Default)]
	public DateTime? CompletedAt { get; init; }

	// Dados da resposta

	[JsonProperty(PropertyName = "textResponse", Required = Required.Default)]
	public string? TextResponse { get; init; }

	[JsonProperty(PropertyName = "signatureData", Required = Required.Default)]
	public string? SignatureData { get; init; }

	[JsonProperty(PropertyName = "uploadedFiles", Required = Required.Default)]
	public ICollection<UploadedFileResponse>? UploadedFiles { get; init; }

	[JsonProperty(PropertyName = "createdDateUtc", Required = Required.Default)]
	public required DateTime CreatedDateUtc { get; init; }

	[JsonProperty(PropertyName = "lastUpdatedDateUtc", Required = Required.Default)]
	public required DateTime LastUpdatedDateUtc { get; init; }
}

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

/// <summary>
/// Response de um arquivo enviado (relacionado ao status do componente)
/// </summary>
public sealed record UploadedFileResponse
{
    public required long Id { get; init; }
    public required long ComponentStatusId { get; init; }
    public required string FileName { get; init; }
    public required string OriginalName { get; init; }
    public required string MimeType { get; init; }
    public required long Size { get; init; }
    public required string Url { get; init; }
    public required DateTime CreatedDateUtc { get; init; }
}

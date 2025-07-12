using Kickoffa.API.Contracts.FileType;

namespace Kickoffa.API.Contracts.Checklist;

/// <summary>
/// Response da criação/consulta de um checklist
/// </summary>
public sealed record ChecklistResponse
{
    public required long Id { get; init; }
    public required long OwnerId { get; init; }
    public required string Title { get; init; }
    public required string Slug { get; init; }
    public string? Description { get; init; }
    public DateTime? Deadline { get; init; }
    public string? AccessToken { get; init; }
    public required bool IsPublished { get; init; }
    public required ICollection<SectionResponse> Sections { get; init; } = [];
    public required DateTime CreatedDateUtc { get; init; }
    public required DateTime LastUpdatedDateUtc { get; init; }
}

/// <summary>
/// Response de uma seção
/// </summary>
public sealed record SectionResponse
{
    public required long Id { get; init; }
    public required long ChecklistId { get; init; }
    public required string Title { get; init; }
    public required string Type { get; init; } // "briefing" ou "checklist"
    public required int Order { get; init; }
    
    // Para seções de briefing
    public string? ContentJson { get; init; }
    public string? ContentHtml { get; init; }
    public DateTime? ContentLastUpdated { get; init; }
    
    // Para seções de checklist
    public ICollection<ComponentResponse>? Components { get; init; }
    
    public required DateTime CreatedDateUtc { get; init; }
    public required DateTime LastUpdatedDateUtc { get; init; }
}

/// <summary>
/// Response de um componente
/// </summary>
public sealed record ComponentResponse
{
    public required long Id { get; init; }
    public required long SectionId { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required string Type { get; init; } // "checkbox", "upload", "text", "signature", "confirmation"
    public required bool IsRequired { get; init; }
    public required int Order { get; init; }

    // Propriedades específicas por tipo - seguindo padrão do domínio

    // Para TextComponent
    public string? Placeholder { get; init; }
    public int? MaxLength { get; init; }

    // Para UploadComponent
    public int? MaxSizeMB { get; init; }
    public ICollection<FileTypeResponse>? AllowedFileTypes { get; init; }
    public ICollection<UploadComponentFileResponse>? ComponentFiles { get; init; }

    // Para ConfirmationComponent
    public string? ConfirmationText { get; init; }

    // Status do componente (se disponível)
    public ComponentStatusResponse? Status { get; init; }

    public required DateTime CreatedDateUtc { get; init; }
    public required DateTime LastUpdatedDateUtc { get; init; }
}

/// <summary>
/// Response do status de um componente
/// </summary>
public sealed record ComponentStatusResponse
{
    public required long Id { get; init; }
    public required long ComponentId { get; init; }
    public required bool IsCompleted { get; init; }
    public DateTime? CompletedAt { get; init; }
    
    // Dados da resposta
    public string? TextResponse { get; init; }
    public string? SignatureData { get; init; }
    public ICollection<UploadedFileResponse>? UploadedFiles { get; init; }
    
    public required DateTime CreatedDateUtc { get; init; }
    public required DateTime LastUpdatedDateUtc { get; init; }
}

/// <summary>
/// Response de um arquivo enviado pelo cliente (relacionado ao componente)
/// </summary>
public sealed record UploadComponentFileResponse
{
    public required long Id { get; init; }
    public required long ComponentId { get; init; }
    public required string FileName { get; init; }
    public required string OriginalName { get; init; }
    public required string MimeType { get; init; }
    public required long Size { get; init; }
    public required string Url { get; init; }
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
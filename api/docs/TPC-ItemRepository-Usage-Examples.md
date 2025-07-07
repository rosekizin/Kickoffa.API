# TPC ItemRepository - Exemplos de Uso

## 📋 **Visão Geral**

Com a migração para **Table-Per-Concrete-Type (TPC)**, cada tipo de Item tem sua própria tabela e seu próprio mapeamento EF Core específico, mas usamos um único `ItemRepository` que fornece métodos genéricos e específicos para cada tipo.

## 🗂️ **Estrutura de Mapeamentos**

### **Mapeamentos Específicos:**
- `ItemEntityFrameworkMapping` - Configurações base (TPC, relacionamentos, índices comuns)
- `TextItemEntityFrameworkMapping` - Configurações específicas do TextItem
- `UploadItemEntityFrameworkMapping` - Configurações específicas do UploadItem
- `ConfirmationItemEntityFrameworkMapping` - Configurações específicas do ConfirmationItem
- `CheckboxItemEntityFrameworkMapping` - Configurações específicas do CheckboxItem
- `SignatureItemEntityFrameworkMapping` - Configurações específicas do SignatureItem

### **Vantagens:**
- **Separação de responsabilidades** - Cada tipo gerencia suas próprias configurações
- **Manutenibilidade** - Fácil encontrar e modificar configurações específicas
- **Escalabilidade** - Adicionar novos tipos sem impactar outros
- **Índices otimizados** - Cada tipo pode ter índices específicos para suas consultas
- **FileType autônomo** - FileType é uma entidade independente sem relacionamentos bidirecionais

## 🔧 **Estrutura das Tabelas**

```sql
-- Cada tipo tem sua própria tabela
CREATE TABLE TextItems (
    Id BIGINT PRIMARY KEY,
    SectionId BIGINT,
    Title NVARCHAR(200),
    Placeholder NVARCHAR(200),
    MaxLength INT
);

CREATE TABLE UploadItems (
    Id BIGINT PRIMARY KEY,
    SectionId BIGINT,
    Title NVARCHAR(200),
    MaxSizeMB INT,
    AllowedMimeTypes NVARCHAR(500)
);

-- etc...
```

## **🔗 Relacionamento UploadItem ↔ FileType:**

### **✅ Estrutura Unidirecional:**
```csharp
// UploadItem conhece FileType
public class UploadItem : Item
{
    public ICollection<FileType> AllowedFileTypes { get; private set; }
}

// FileType é autônomo - não conhece UploadItem
public class FileType : BaseEntity<FileType>
{
    // Apenas propriedades do domínio, sem relacionamentos
}
```

### **✅ Mapeamento EF Core:**
```csharp
// Relacionamento N:N unidirecional
entity.HasMany(u => u.AllowedFileTypes)
    .WithMany() // Sem propriedade de volta no FileType
    .UsingEntity("UploadItemAllowedFileTypes");
```

### **✅ Consultas:**
```csharp
// ✅ Funciona - UploadItem → FileType
var uploadItem = await context.UploadItems
    .Include(u => u.AllowedFileTypes)
    .FirstAsync(u => u.Id == 1);

// ✅ Consulta reversa usando Where
var uploadItemsUsingJpg = await context.UploadItems
    .Include(u => u.AllowedFileTypes)
    .Where(u => u.AllowedFileTypes.Any(ft => ft.Extension == ".jpg"))
    .ToListAsync();
```

## 🎯 **Exemplos de Uso no Controller**

### **1. Buscar todos os itens de uma seção (mixed types):**

```csharp
[HttpGet("sections/{sectionId}/items")]
public async Task<ActionResult<IEnumerable<ItemDto>>> GetSectionItems(long sectionId)
{
    // EF faz UNION ALL automaticamente
    var items = await _itemRepository.GetBySectionIdOrderedAsync(sectionId, CancellationToken.None);
    
    // Cast para DTOs específicos
    var itemDtos = items.Select(item => item switch
    {
        TextItem textItem => new TextItemDto
        {
            Id = textItem.Id,
            Type = "text",
            Title = textItem.Title,
            Placeholder = textItem.Placeholder,
            MaxLength = textItem.MaxLength
        },
        UploadItem uploadItem => new UploadItemDto
        {
            Id = uploadItem.Id,
            Type = "upload",
            Title = uploadItem.Title,
            MaxSizeMB = uploadItem.MaxSizeMB,
            AllowedMimeTypes = uploadItem.AllowedMimeTypes
        },
        ConfirmationItem confirmationItem => new ConfirmationItemDto
        {
            Id = confirmationItem.Id,
            Type = "confirmation",
            Title = confirmationItem.Title,
            ConfirmationText = confirmationItem.ConfirmationText
        },
        CheckboxItem checkboxItem => new CheckboxItemDto
        {
            Id = checkboxItem.Id,
            Type = "checkbox",
            Title = checkboxItem.Title
        },
        SignatureItem signatureItem => new SignatureItemDto
        {
            Id = signatureItem.Id,
            Type = "signature",
            Title = signatureItem.Title
        },
        _ => throw new InvalidOperationException($"Unknown item type: {item.GetType()}")
    });
    
    return Ok(itemDtos);
}
```

### **2. Buscar apenas um tipo específico:**

```csharp
[HttpGet("sections/{sectionId}/text-items")]
public async Task<ActionResult<IEnumerable<TextItemDto>>> GetTextItems(long sectionId)
{
    // Consulta apenas a tabela TextItems
    var textItems = await _itemRepository.GetBySectionIdAsync<TextItem>(sectionId, CancellationToken.None);
    
    var dtos = textItems.Select(item => new TextItemDto
    {
        Id = item.Id,
        Type = "text",
        Title = item.Title,
        Placeholder = item.Placeholder,
        MaxLength = item.MaxLength
    });
    
    return Ok(dtos);
}
```

### **3. Buscar por critérios específicos:**

```csharp
[HttpGet("text-items/by-placeholder")]
public async Task<ActionResult<IEnumerable<TextItemDto>>> GetTextItemsByPlaceholder(string placeholder)
{
    var textItems = await _itemRepository.GetTextItemsByPlaceholderAsync(placeholder, CancellationToken.None);
    
    var dtos = textItems.Select(item => new TextItemDto
    {
        Id = item.Id,
        Type = "text",
        Title = item.Title,
        Placeholder = item.Placeholder,
        MaxLength = item.MaxLength
    });
    
    return Ok(dtos);
}

[HttpGet("confirmation-items/search")]
public async Task<ActionResult<IEnumerable<ConfirmationItemDto>>> SearchConfirmationItems(string searchText)
{
    var items = await _itemRepository.SearchConfirmationItemsByTextAsync(searchText, CancellationToken.None);
    
    var dtos = items.Select(item => new ConfirmationItemDto
    {
        Id = item.Id,
        Type = "confirmation",
        Title = item.Title,
        ConfirmationText = item.ConfirmationText
    });
    
    return Ok(dtos);
}
```

### **4. Estatísticas por tipo:**

```csharp
[HttpGet("sections/{sectionId}/stats")]
public async Task<ActionResult<SectionStatsDto>> GetSectionStats(long sectionId)
{
    var totalItems = await _itemRepository.CountBySectionIdAsync(sectionId, CancellationToken.None);
    var textItemsCount = await _itemRepository.CountBySectionIdAsync<TextItem>(sectionId, CancellationToken.None);
    var uploadItemsCount = await _itemRepository.CountBySectionIdAsync<UploadItem>(sectionId, CancellationToken.None);
    var confirmationItemsCount = await _itemRepository.CountBySectionIdAsync<ConfirmationItem>(sectionId, CancellationToken.None);
    var checkboxItemsCount = await _itemRepository.CountBySectionIdAsync<CheckboxItem>(sectionId, CancellationToken.None);
    var signatureItemsCount = await _itemRepository.CountBySectionIdAsync<SignatureItem>(sectionId, CancellationToken.None);
    
    return Ok(new SectionStatsDto
    {
        TotalItems = totalItems,
        TextItems = textItemsCount,
        UploadItems = uploadItemsCount,
        ConfirmationItems = confirmationItemsCount,
        CheckboxItems = checkboxItemsCount,
        SignatureItems = signatureItemsCount
    });
}
```

## 🚀 **Vantagens da Abordagem TPC**

### **✅ Performance:**
- **Consultas específicas** são mais rápidas (apenas uma tabela)
- **Sem colunas NULL** desnecessárias
- **Índices otimizados** por tipo

### **✅ Flexibilidade:**
- **Métodos genéricos** para consultas mistas
- **Métodos específicos** para consultas por tipo
- **Type-safe** com generics

### **✅ Manutenibilidade:**
- **Um repositório** para gerenciar
- **Fácil adicionar** novos tipos
- **Testes centralizados**

## 📊 **SQL Gerado**

### **Consulta mista (todos os tipos):**
```sql
-- EF gera UNION ALL automaticamente
SELECT * FROM (
    SELECT Id, SectionId, Title, 'TextItem' as Discriminator, Placeholder, MaxLength, NULL as MaxSizeMB, NULL as ConfirmationText
    FROM TextItems
    UNION ALL
    SELECT Id, SectionId, Title, 'UploadItem' as Discriminator, NULL as Placeholder, NULL as MaxLength, MaxSizeMB, NULL as ConfirmationText  
    FROM UploadItems
    UNION ALL
    SELECT Id, SectionId, Title, 'ConfirmationItem' as Discriminator, NULL as Placeholder, NULL as MaxLength, NULL as MaxSizeMB, ConfirmationText
    FROM ConfirmationItems
) WHERE SectionId = @sectionId
ORDER BY [Order]
```

### **Consulta específica:**
```sql
-- Apenas uma tabela
SELECT Id, SectionId, Title, Placeholder, MaxLength
FROM TextItems
WHERE SectionId = @sectionId
ORDER BY [Order]
```

## 🎯 **Resumo**

Com TPC + ItemRepository expandido, você tem:
- **Uma consulta** para buscar todos os tipos (UNION ALL otimizado)
- **Consultas específicas** para performance máxima
- **Type-safe casting** em memória
- **Flexibilidade total** para diferentes cenários de uso

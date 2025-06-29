# Sistema de Gerenciamento de Tipos de Arquivo

## 🎯 Objetivo

Implementar um sistema robusto e flexível para gerenciar tipos de arquivo permitidos em uploads, substituindo a abordagem manual de digitação de MIME types por uma interface amigável com busca dinâmica.

## 🏗️ Arquitetura Implementada

### **Backend (API)**

#### **1. Modelo de Domínio**

```csharp
// Enum para categorização
public enum FileTypeCategory
{
    Image, Document, Audio, Video, Archive, Code, Design, Spreadsheet, Presentation, Font, Other
}

// Entidade principal
public class FileType
{
    public long Id { get; set; }
    public string MimeType { get; set; }        // "image/jpeg"
    public string Extension { get; set; }       // ".jpg"
    public string DisplayName { get; set; }     // "JPEG Image"
    public string? Description { get; set; }    // "Formato de imagem comprimida"
    public FileTypeCategory Category { get; set; }
    public bool IsActive { get; set; }
    public int? RecommendedMaxSizeMB { get; set; }
    public int DisplayOrder { get; set; }
}

// Relacionamento many-to-many
public class UploadItemFileType
{
    public long UploadItemId { get; set; }
    public long FileTypeId { get; set; }
    public UploadItem UploadItem { get; set; }
    public FileType FileType { get; set; }
}
```

#### **2. Repositório e Serviços**

```csharp
// Interface do repositório
public interface IFileTypeRepository
{
    Task<IEnumerable<FileType>> GetActiveFileTypesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<FileType>> SearchFileTypesAsync(string searchTerm, CancellationToken cancellationToken);
    Task<IEnumerable<FileType>> GetFileTypesByCategoryAsync(FileTypeCategory category, CancellationToken cancellationToken);
}

// Serviço de aplicação
public interface IFileTypeService
{
    Task<FileTypesSearchResponse> GetActiveFileTypesAsync(CancellationToken cancellationToken);
    Task<FileTypesSearchResponse> SearchFileTypesAsync(string? searchTerm, CancellationToken cancellationToken);
    Task<FileTypesSearchResponse> GetFileTypesByCategoryAsync(FileTypeCategory category, CancellationToken cancellationToken);
}
```

#### **3. API Endpoints**

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FileTypeController : ControllerBase
{
    [HttpGet]                           // GET /api/filetypes
    [HttpGet("search")]                 // GET /api/filetypes/search?search=pdf
    [HttpGet("category/{category}")]    // GET /api/filetypes/category/Image
}
```

#### **4. Contratos de Resposta**

```csharp
public record FileTypeResponse(
    long Id,
    string MimeType,
    string Extension,
    string DisplayName,
    string? Description,
    FileTypeCategory Category,
    int? RecommendedMaxSizeMB
);

public record FileTypesSearchResponse(
    IEnumerable<FileTypeResponse> FileTypes,
    int TotalCount
);
```

### **Frontend (React/TypeScript)**

#### **1. Tipos TypeScript**

```typescript
export interface FileType {
  id: number
  mimeType: string
  extension: string
  displayName: string
  description?: string
  category: FileTypeCategory
  recommendedMaxSizeMB?: number
}

export enum FileTypeCategory {
  Image = 'Image',
  Document = 'Document',
  Audio = 'Audio',
  Video = 'Video',
  Archive = 'Archive',
  Code = 'Code',
  Design = 'Design',
  Spreadsheet = 'Spreadsheet',
  Presentation = 'Presentation',
  Font = 'Font',
  Other = 'Other'
}
```

#### **2. Hooks de API**

```typescript
export const useFileTypes = () => useQuery({
  queryKey: ['fileTypes'],
  queryFn: async () => {
    const response = await api.get<FileTypesSearchResponse>('/filetypes')
    return response.data
  }
})

export const useSearchFileTypes = (searchTerm?: string) => useQuery({
  queryKey: ['fileTypes', 'search', searchTerm],
  queryFn: async () => {
    const params = searchTerm ? { search: searchTerm } : {}
    const response = await api.get<FileTypesSearchResponse>('/filetypes/search', { params })
    return response.data
  }
})
```

#### **3. Componente FileTypeSelector**

```typescript
interface FileTypeSelectorProps {
  selectedFileTypes: FileType[]
  onSelectionChange: (fileTypes: FileType[]) => void
  placeholder?: string
  maxSelections?: number
}

export const FileTypeSelector = ({ ... }) => {
  // Implementação com:
  // - Busca com debounce
  // - Agrupamento por categoria
  // - Seleção múltipla
  // - Limite máximo de seleções
  // - Interface amigável
}
```

## 🎨 Funcionalidades Implementadas

### **✅ 1. Busca Dinâmica**
- Busca em tempo real com debounce (300ms)
- Pesquisa em: nome, extensão, descrição e MIME type
- Resultados agrupados por categoria

### **✅ 2. Interface Amigável**
- Dropdown com busca integrada
- Chips para tipos selecionados
- Informações detalhadas (extensão, MIME type, tamanho recomendado)
- Categorização visual

### **✅ 3. Validações**
- Limite máximo de seleções
- Tipos ativos/inativos
- Tamanhos recomendados por tipo

### **✅ 4. Tipos de Arquivo Suportados**

**Imagens:** JPEG, PNG, GIF, SVG, WebP  
**Documentos:** PDF, DOC, DOCX, TXT, RTF  
**Planilhas:** XLS, XLSX, CSV  
**Apresentações:** PPT, PPTX  
**Áudio:** MP3, WAV, OGG  
**Vídeo:** MP4, AVI, MOV  
**Design:** PSD, AI, INDD  
**Arquivos:** ZIP, RAR  
**Código:** HTML, CSS, JS  
**Fontes:** TTF, OTF  

## 🔄 Migração da Abordagem Anterior

### **❌ Antes:**
```typescript
// Campo manual para MIME types
<input
  type="text"
  placeholder="Ex: image/jpeg,image/png,application/pdf"
  value={allowedMimeTypes}
  onChange={(e) => setAllowedMimeTypes(e.target.value)}
/>
```

### **✅ Agora:**
```typescript
// Seletor visual e intuitivo
<FileTypeSelector
  selectedFileTypes={selectedFileTypes}
  onSelectionChange={setSelectedFileTypes}
  placeholder="Selecione os tipos de arquivo permitidos..."
  maxSelections={10}
/>
```

## 🚀 Benefícios

1. **UX Melhorada**: Interface visual ao invés de digitação manual
2. **Menos Erros**: Validação automática de MIME types
3. **Flexibilidade**: Fácil adicionar novos tipos via seed data
4. **Performance**: Busca otimizada com debounce
5. **Manutenibilidade**: Tipos centralizados no backend
6. **Escalabilidade**: Suporte a categorias e metadados

## 📝 Próximos Passos

1. **Migration**: Criar migration para as novas tabelas
2. **Seed Data**: Popular banco com tipos de arquivo comuns
3. **Testes**: Implementar testes unitários e de integração
4. **Cache**: Implementar cache para tipos de arquivo
5. **Admin**: Interface administrativa para gerenciar tipos

## 🎯 Resultado

O sistema agora oferece uma **experiência moderna e intuitiva** para seleção de tipos de arquivo, eliminando erros de digitação e proporcionando uma interface profissional para freelancers configurarem uploads de forma eficiente.

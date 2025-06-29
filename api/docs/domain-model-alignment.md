# Alinhamento do Modelo de Domínio Frontend/Backend

## 📋 Resumo das Alterações

Este documento descreve as alterações realizadas para alinhar os modelos de domínio entre o frontend (React/TypeScript) e o backend (C#/.NET) do projeto Kickoffa.API.

## 🎯 Objetivo

Sincronizar as classes de domínio com o estado atual da interface de criação de checklists (`/checklists/new`), removendo propriedades desnecessárias e ajustando tipos para compatibilidade.

## 🔧 Alterações Realizadas

### **Frontend (TypeScript)**

#### **1. Interface Checklist**
```typescript
// ANTES
export interface Checklist {
  id: string
  token: string
  createdBy: string
}

// DEPOIS
export interface Checklist {
  id: number
  slug: string
  accessToken: string
  ownerId: number // ID do usuário que criou o checklist
}
```

#### **2. Interface Section**
```typescript
// ADICIONADO
export interface Section {
  contentLastUpdated?: string // Para auditoria
}
```

#### **3. Interface Item**
```typescript
// ANTES
export interface Item {
  type: 'checkbox' | 'upload' | 'text' | 'signature'
  config?: {
    allowedFileTypes?: string[]
    maxFileSize?: number
    placeholder?: string
  }
}

// DEPOIS
export interface Item {
  type: 'checkbox' | 'upload' | 'text' | 'signature' | 'confirmation'
  // Propriedades específicas (movidas de config)
  allowedMimeTypes?: string
  maxSizeMB?: number
  placeholder?: string
  maxLength?: number
  confirmationText?: string
}
```

### **Backend (C#)**

#### **1. Classe Section**
```csharp
// ANTES
public class Section
{
    public Guid Id { get; set; }
    public Guid ChecklistId { get; set; }
    public bool IsCollapsed { get; set; } = false;        // ❌ Estado de UI
    public string? IconName { get; set; }                 // ❌ Lógica de apresentação
    public string? CustomCssClass { get; set; }           // ❌ Violação de responsabilidades
}

// DEPOIS
public class Section
{
    public long Id { get; set; }
    public long ChecklistId { get; set; }
    public DateTime? ContentLastUpdated { get; set; }     // ✅ Auditoria
}
```

#### **2. Enum ItemType**
```csharp
// ANTES
public enum ItemType
{
    SimpleCheckbox,
    TextInput,
    FileUpload,
    Signature,
    Confirmation
}

// DEPOIS - Nomes compatíveis com frontend
public enum ItemType
{
    Checkbox,
    Text,
    Upload,
    Signature,
    Confirmation
}
```

#### **3. Classe Item**
```csharp
// Propriedades organizadas por tipo
public string? AllowedMimeTypes { get; set; } // Para Upload
public int? MaxSizeMB { get; set; } // Para Upload
public string? Placeholder { get; set; } // Para Text e Upload
public int? MaxLength { get; set; } // Para Text
public string? ConfirmationText { get; set; } // Para Confirmation
```

## 🎨 Componentes Atualizados

### **1. ChecklistItemEditor**
- ✅ Suporte ao tipo `confirmation`
- ✅ Propriedades específicas ao invés de `config` genérico
- ✅ Configurações avançadas para cada tipo de item

### **2. SortableChecklistItems**
- ✅ Ícone para tipo `confirmation`
- ✅ Tipos atualizados

### **3. SortableSectionList**
- ✅ Propriedade `contentLastUpdated` adicionada

## 🚨 Princípios Aplicados

### **❌ Removido do Backend:**
1. **`IsCollapsed`** - Estado de UI deve ser gerenciado no frontend
2. **`IconName`** - Lógica de apresentação é responsabilidade do frontend
3. **`CustomCssClass`** - Violação de responsabilidades e risco de segurança

### **✅ Mantido no Backend:**
1. **`ContentLastUpdated`** - Informação de auditoria importante
2. **Propriedades específicas** - Estrutura clara ao invés de config genérico

### **🔧 Padronização de IDs:**
- **Todos os IDs**: Alterados de `Guid` para `long` (auto-increment)
- **Consistência**: Alinhado com `User : IdentityUser<long>` e `BaseEntity<T>`
- **Performance**: IDs numéricos são mais eficientes para indexação

### **🎯 Benefícios:**
- **Separação de responsabilidades** - Backend foca em dados, Frontend em apresentação
- **Segurança** - Sem CSS arbitrário no banco
- **Performance** - Menos dados trafegados + IDs numéricos eficientes
- **Manutenibilidade** - UI gerenciada onde deve ser
- **Flexibilidade** - Frontend pode evoluir independentemente
- **Consistência** - Padrão único de IDs em todo o projeto

## 📝 Próximos Passos

1. **Atualizar DTOs** - Ajustar DTOs de request/response para refletir as mudanças
2. **Migrations** - Criar migrations para remover colunas desnecessárias
3. **Testes** - Atualizar testes unitários e de integração
4. **Documentação da API** - Atualizar Swagger/OpenAPI specs

## 🔍 Validação

- ✅ Frontend compila sem erros
- ✅ Backend compila sem erros  
- ✅ Tipos alinhados entre frontend e backend
- ✅ Novo tipo `confirmation` suportado
- ✅ Propriedades de UI removidas do backend

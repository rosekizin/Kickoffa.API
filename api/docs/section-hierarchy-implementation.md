# Implementação da Hierarquia de Seções

## 📋 Visão Geral

Esta implementação refatora a classe `Section` para usar herança, separando responsabilidades entre diferentes tipos de seções:

- **`Section`** - Classe base abstrata
- **`BriefingSection`** - Para conteúdo rico e mídias
- **`ChecklistSection`** - Para itens interativos

## 🏗️ Arquitetura

### Hierarquia de Classes

```
Section (abstract)
├── BriefingSection
└── ChecklistSection
```

### Table-Per-Hierarchy (TPH)

Utilizamos TPH no Entity Framework com discriminador na coluna `Type`:
- `"Briefing"` → `BriefingSection`
- `"Checklist"` → `ChecklistSection`

## 📁 Arquivos Criados/Modificados

### Novos Arquivos

1. **`BriefingSection.cs`** - Seção para conteúdo rico
2. **`ChecklistSection.cs`** - Seção para itens de checklist
3. **`SectionFactory.cs`** - Factory para criação de seções
4. **`SectionExtensions.cs`** - Extensões utilitárias
5. **`AddSectionHierarchy.sql`** - Migration para o banco

### Arquivos Modificados

1. **`Section.cs`** - Transformado em classe base abstrata
2. **`SectionEntityFrameworkMapping.cs`** - Atualizado para TPH
3. **`BriefingMedia.cs`** - Relacionamento com `BriefingSection`
4. **`Item.cs`** - Relacionamento com `ChecklistSection`

## 🎯 Benefícios

### Separação de Responsabilidades
- **BriefingSection**: Foca em conteúdo e mídias
- **ChecklistSection**: Foca em itens e progresso

### Type Safety
- Métodos específicos para cada tipo
- Compilador previne uso incorreto

### Extensibilidade
- Fácil adição de novos tipos de seção
- Factory pattern para criação consistente

## 🔧 Como Usar

### Criando Seções

```csharp
// Usando Factory
var briefing = SectionFactory.CreateBriefingSection(
    checklistId: 1, 
    title: "Informações do Projeto", 
    order: 1,
    contentJson: "...",
    contentHtml: "..."
);

var checklist = SectionFactory.CreateChecklistSection(
    checklistId: 1, 
    title: "Tarefas", 
    order: 2
);

// Criação genérica
var section = SectionFactory.CreateSection(
    SectionType.Briefing, 
    checklistId: 1, 
    title: "Nova Seção", 
    order: 3
);
```

### Trabalhando com Seções

```csharp
// Verificação de tipo
if (section.IsBriefingSection())
{
    var briefing = section.AsBriefingSection();
    briefing?.UpdateContent(newJson, newHtml);
}

// Métodos específicos
var checklistSection = new ChecklistSection();
checklistSection.AddItem(newItem);
var progress = checklistSection.CalculateProgress();

var briefingSection = new BriefingSection();
briefingSection.AddMedia(newMedia);
var hasContent = briefingSection.HasContent();
```

### Extensões Úteis

```csharp
// Verificações
bool isEmpty = section.IsEmpty();
bool hasContent = section.HasContent();
string description = section.GetContentDescription();

// Estatísticas
var stats = section.GetStatistics();
// Para ChecklistSection: ItemCount, Progress, RequiredItemsCompleted
// Para BriefingSection: MediaCount, HasTextContent, LastContentUpdate
```

## 🗄️ Banco de Dados

### Estrutura da Tabela

```sql
CREATE TABLE "Sections" (
    "Id" BIGINT PRIMARY KEY,
    "ChecklistId" BIGINT NOT NULL,
    "Title" VARCHAR(200) NOT NULL,
    "Order" INTEGER NOT NULL,
    "Type" VARCHAR(50) NOT NULL, -- Discriminador TPH
    
    -- Campos específicos de BriefingSection
    "ContentJson" TEXT NULL,
    "ContentHtml" TEXT NULL,
    "ContentLastUpdated" TIMESTAMP NULL,
    
    -- Campos de auditoria
    "CreatedDateUtc" TIMESTAMP NOT NULL,
    "LastUpdatedDateUtc" TIMESTAMP NOT NULL,
    
    CONSTRAINT "CK_Sections_Type" CHECK ("Type" IN ('Briefing', 'Checklist'))
);
```

### Relacionamentos

- **BriefingSection** ↔ **BriefingMedia** (1:N)
- **ChecklistSection** ↔ **Item** (1:N)
- **Section** ↔ **Checklist** (N:1)

## 🚀 Migration

Execute o script `AddSectionHierarchy.sql` para:

1. Atualizar coluna `Type` para string
2. Converter valores existentes
3. Adicionar constraints
4. Criar índices necessários

## ⚠️ Considerações

### Compatibilidade
- Código existente que usa `Section` diretamente precisará ser atualizado
- Queries LINQ podem precisar de ajustes para tipos específicos

### Performance
- TPH é eficiente para consultas polimórficas
- Índice no discriminador melhora performance de filtros por tipo

### Evolução
- Novos tipos de seção podem ser adicionados facilmente
- Factory pattern centraliza lógica de criação
- Extensões facilitam operações comuns

## 📝 Próximos Passos

1. **Executar migration** no banco de dados
2. **Atualizar services** para usar tipos específicos
3. **Atualizar controllers** para trabalhar com hierarquia
4. **Criar testes** para novos comportamentos
5. **Atualizar documentação** da API

## 🧪 Testes Recomendados

- Criação de seções via Factory
- Conversão entre tipos (se necessário)
- Operações específicas de cada tipo
- Queries polimórficas no EF
- Serialização/deserialização JSON

Esta implementação fornece uma base sólida para evolução futura do sistema de seções, mantendo separação clara de responsabilidades e type safety.

# Bug EF Core: HasDatabaseName() com TPC (Table-Per-Concrete-Type)

## 🐛 **Problema Identificado**

### **Sintoma**
Ao usar **TPC (Table-Per-Concrete-Type)** no Entity Framework Core e definir nomes customizados de índices com `HasDatabaseName()`, o EF Core gera migrations com **índices duplicados** e **nomes incorretos**.

### **Exemplo do Bug**
```csharp
// ❌ CONFIGURAÇÃO QUE CAUSA O BUG
public void Map(ModelBuilder modelBuilder)
{
    var entity = modelBuilder.Entity<CheckboxComponent>();
    entity.ToTable("CheckboxComponents");
    
    entity.HasIndex(c => c.SectionId)
        .HasDatabaseName("IX_CheckboxComponents_SectionId"); // ← CAUSA BUG TPC
}

public void Map(ModelBuilder modelBuilder)
{
    var entity = modelBuilder.Entity<SignatureComponent>();
    entity.ToTable("SignatureComponents");
    
    entity.HasIndex(s => s.SectionId)
        .HasDatabaseName("IX_SignatureComponents_SectionId"); // ← CAUSA BUG TPC
}
```

### **Migration Gerada (Incorreta)**
```sql
-- ❌ RESULTADO: Índices duplicados com nomes errados
CREATE INDEX "IX_SignatureComponents_SectionId" ON "CheckboxComponents" ("SectionId");
CREATE INDEX "IX_SignatureComponents_SectionId" ON "ConfirmationComponents" ("SectionId");
CREATE INDEX "IX_SignatureComponents_SectionId" ON "TextComponents" ("SectionId");
CREATE INDEX "IX_SignatureComponents_SectionId" ON "UploadComponents" ("SectionId");
CREATE INDEX "IX_SignatureComponents_SectionId" ON "SignatureComponents" ("SectionId");
```

**Problema**: Todas as tabelas recebem o nome do índice do **último tipo processado**.

## ✅ **Solução**

### **Remover HasDatabaseName() em Entidades TPC**
```csharp
// ✅ CONFIGURAÇÃO CORRETA (Sem HasDatabaseName)
public void Map(ModelBuilder modelBuilder)
{
    var entity = modelBuilder.Entity<CheckboxComponent>();
    entity.ToTable("CheckboxComponents");
    
    entity.HasIndex(c => c.SectionId); // ← SEM HasDatabaseName()
    entity.HasIndex(c => new { c.SectionId, c.Order }); // ← SEM HasDatabaseName()
    entity.HasIndex(c => c.IsRequired); // ← SEM HasDatabaseName()
}

public void Map(ModelBuilder modelBuilder)
{
    var entity = modelBuilder.Entity<SignatureComponent>();
    entity.ToTable("SignatureComponents");
    
    entity.HasIndex(s => s.SectionId); // ← SEM HasDatabaseName()
    entity.HasIndex(s => new { s.SectionId, s.IsRequired }); // ← SEM HasDatabaseName()
}
```

### **Migration Gerada (Correta)**
```sql
-- ✅ RESULTADO: Índices corretos com nomes automáticos
CREATE INDEX "IX_CheckboxComponents_SectionId" ON "CheckboxComponents" ("SectionId");
CREATE INDEX "IX_ConfirmationComponents_SectionId" ON "ConfirmationComponents" ("SectionId");
CREATE INDEX "IX_TextComponents_SectionId" ON "TextComponents" ("SectionId");
CREATE INDEX "IX_UploadComponents_SectionId" ON "UploadComponents" ("SectionId");
CREATE INDEX "IX_SignatureComponents_SectionId" ON "SignatureComponents" ("SectionId");
```

## 📋 **Detalhes Técnicos**

### **Versões Afetadas**
- ✅ **Confirmado**: EF Core 8.0.x
- ✅ **Confirmado**: EF Core 9.0.6 (versão mais recente)
- ❓ **Status**: Bug ainda não corrigido

### **Cenários Afetados**
- ✅ **TPC (Table-Per-Concrete-Type)** com `HasDatabaseName()`
- ❌ **TPH (Table-Per-Hierarchy)**: Não afetado
- ❌ **TPT (Table-Per-Type)**: Não afetado

### **Causa Raiz**
O EF Core tem um bug interno onde, ao processar entidades TPC com nomes customizados de índices, ele "vaza" o nome do último tipo processado para todos os outros tipos da hierarquia.

## 🎯 **Boas Práticas**

### **✅ Para TPC - Use Convenção Automática**
```csharp
// Deixe o EF Core gerar nomes automaticamente
entity.HasIndex(c => c.SectionId);
entity.HasIndex(c => new { c.SectionId, c.Order });

// Nomes gerados automaticamente:
// IX_CheckboxComponents_SectionId
// IX_CheckboxComponents_SectionId_Order
```

### **✅ Para TPH/TPT - HasDatabaseName() Funciona**
```csharp
// Em TPH/TPT, HasDatabaseName() funciona normalmente
entity.HasIndex(c => c.SectionId)
    .HasDatabaseName("IX_Components_SectionId"); // ✅ OK em TPH/TPT
```

### **✅ Índices Filtrados - Cuidado Especial**
```csharp
// Para índices filtrados, ainda pode usar HasDatabaseName() se necessário
entity.HasIndex(c => c.IsRequired)
    .HasFilter("\"IsRequired\" = true"); // Sem HasDatabaseName() é mais seguro
```

## 🔗 **Referências**

- **GitHub Issue**: https://github.com/efcore/EFCore.NamingConventions/issues/185#issuecomment-1876016568
- **Discussão**: Problema reportado pela comunidade em dezembro de 2023
- **Status**: Bug confirmado, sem previsão de correção

## 🚀 **Migração de Código Existente**

### **Passo 1: Identificar Entidades TPC**
```bash
# Buscar por HasDatabaseName em mapeamentos TPC
grep -r "HasDatabaseName" src/*/EntityFramework/Mapping/*Component*
```

### **Passo 2: Remover HasDatabaseName()**
```csharp
// ANTES
entity.HasIndex(c => c.SectionId)
    .HasDatabaseName("IX_CheckboxComponents_SectionId");

// DEPOIS
entity.HasIndex(c => c.SectionId);
```

### **Passo 3: Regenerar Migration**
```bash
dotnet ef migrations remove
dotnet ef migrations add FixTpcIndexNames
dotnet ef database update
```

## 💡 **Dica para Desenvolvedores**

**Sempre que usar TPC no EF Core, evite `HasDatabaseName()` em índices.** A convenção automática do EF Core gera nomes consistentes e evita este bug conhecido.

Este bug pode causar horas de debugging desnecessário. Compartilhe esta informação com sua equipe! 🏆

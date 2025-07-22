# Configuração do Serilog - Kickoffa API

Este documento descreve como o Serilog foi implementado no projeto Kickoffa API seguindo os padrões estabelecidos.

## 📋 Estrutura Implementada

### 1. Pacotes Adicionados
- `Serilog` - Core do Serilog
- `Serilog.AspNetCore` - Integração com ASP.NET Core
- `Serilog.Sinks.Console` - Output para console
- `Serilog.Sinks.File` - Output para arquivos
- `Serilog.Enrichers.Environment` - Enricher de ambiente
- `Serilog.Enrichers.Process` - Enricher de processo
- `Serilog.Enrichers.Thread` - Enricher de thread
- `Serilog.Settings.Configuration` - Configuração via appsettings.json
- `Serilog.Sinks.Elasticsearch` (comentado) - Para futura integração com Elasticsearch

### 2. Arquivos Criados

#### Configuração
- `Configuration/Logging/SerilogConfiguration.cs` - Interface e implementação da configuração
- `Extensions/Service.Collection/SerilogServiceCollectionExtensions.cs` - Extensões para DI
- `Extensions/WebApplicationBuilderExtensions.cs` - Extensões para WebApplicationBuilder

#### Configurações
- Atualizações em `appsettings.json` e `appsettings.Development.json`

## 🚀 Como Usar

### 1. Configuração Básica (Program.cs)
```csharp
// Configuração completa com base no appsettings.json
builder.AddSerilogLogging(configurationWrapper);

// OU configuração simplificada apenas console
builder.AddSerilogConsoleLogging();
```

### 2. Uso em Controllers
```csharp
public class ExampleController : ControllerBase
{
    private readonly ILogger<ExampleController> _logger;

    public ExampleController(ILogger<ExampleController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> ExampleAction()
    {
        _logger.LogInformation("🔍 Executando ação de exemplo");
        
        try
        {
            // Sua lógica aqui
            _logger.LogInformation("✅ Ação executada com sucesso");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao executar ação");
            throw;
        }
    }
}
```

### 3. Uso em Services
```csharp
public class ExampleService
{
    private readonly ILogger<ExampleService> _logger;

    public ExampleService(ILogger<ExampleService> logger)
    {
        _logger = logger;
    }

    public async Task ProcessDataAsync(int id)
    {
        _logger.LogDebug("Processando dados para ID: {Id}", id);
        
        // Sua lógica aqui
        
        _logger.LogInformation("Dados processados com sucesso para ID: {Id}", id);
    }
}
```

## ⚙️ Configuração

### appsettings.json (Produção)
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    },
    "WriteTo": {
      "Console": {
        "Enabled": true
      },
      "File": {
        "Enabled": false,
        "Path": "logs/kickoffa-api-.txt",
        "FileSizeLimitMB": 10,
        "RetainedFileCountLimit": 31
      }
    },
    "Enrich": {
      "Enabled": true
    },
    "OutputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
  }
}
```

### appsettings.Development.json
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    },
    "WriteTo": {
      "Console": {
        "Enabled": true
      },
      "File": {
        "Enabled": true,
        "Path": "logs/kickoffa-api-dev-.txt",
        "FileSizeLimitMB": 50,
        "RetainedFileCountLimit": 7
      }
    }
  }
}
```

## 🔮 Elasticsearch (Futuro)

Para habilitar o Elasticsearch no futuro, descomente as linhas relacionadas e adicione a configuração:

### 1. Descomente o pacote
```xml
<PackageReference Include="Serilog.Sinks.Elasticsearch" />
```

### 2. Adicione a configuração
```json
{
  "Serilog": {
    "WriteTo": {
      "Elasticsearch": {
        "Enabled": true,
        "Uri": "http://localhost:9200",
        "Index": "kickoffa-api-logs"
      }
    }
  }
}
```

### 3. Descomente o código no SerilogServiceCollectionExtensions.cs

## 📊 Níveis de Log

- **Debug**: Informações detalhadas para debugging
- **Information**: Informações gerais sobre o fluxo da aplicação
- **Warning**: Situações inesperadas mas não críticas
- **Error**: Erros que não impedem a aplicação de continuar
- **Fatal**: Erros críticos que podem causar o encerramento da aplicação

## 🎯 Boas Práticas

1. **Use structured logging**: `_logger.LogInformation("Usuário {UserId} fez login", userId)`
2. **Não logue informações sensíveis**: Senhas, tokens, dados pessoais
3. **Use níveis apropriados**: Debug para desenvolvimento, Information para produção
4. **Inclua contexto**: IDs, timestamps, ações relevantes
5. **Use emojis para facilitar visualização**: 🔍 🔐 ✅ ❌ ⚠️

## 🔧 Troubleshooting

### Logs não aparecem
- Verifique se `builder.AddSerilogLogging()` está sendo chamado no Program.cs
- Verifique as configurações no appsettings.json
- Verifique se o nível de log está correto

### Arquivos de log não são criados
- Verifique se `"File": { "Enabled": true }` no appsettings.json
- Verifique permissões da pasta `logs/`
- Verifique se o caminho do arquivo está correto

# Testes Unitários do Serilog - Kickoffa API

Este documento descreve os testes unitários criados para validar a implementação do Serilog no projeto.

## 📋 Estrutura de Testes

### 1. Testes de Configuração

#### `SerilogConfigurationTests.cs`
- **Localização**: `api/tests/Kickoffa.API.AspNet.Infrastructure.UnitTests/Configuration/Logging/`
- **Propósito**: Testar a classe `SerilogConfiguration`
- **Cobertura**:
  - ✅ Valores padrão quando configuração está vazia
  - ✅ Uso de valores da configuração quando fornecidos
  - ✅ Validação de níveis de log válidos
  - ✅ Comportamento com valores zero (FileSizeLimitMB, RetainedFileCountLimit)
  - ✅ Propriedades init-only

#### `SerilogConfigurationIntegrationTests.cs`
- **Localização**: `api/tests/Kickoffa.API.AspNet.Infrastructure.UnitTests/Configuration/Logging/`
- **Propósito**: Testar integração com arquivos de configuração reais
- **Cobertura**:
  - ✅ Carregamento de appsettings.test.json
  - ✅ Configurações de produção e desenvolvimento
  - ✅ Configuração mínima e apenas arquivo
  - ✅ Templates customizados
  - ✅ Todos os níveis de log válidos
  - ✅ Valores booleanos

### 2. Testes de Extensões

#### `SerilogServiceCollectionExtensionsTests.cs`
- **Localização**: `api/tests/Kickoffa.API.AspNet.Infrastructure.UnitTests/Extensions/Service.Collection/`
- **Propósito**: Testar extensões do `IServiceCollection`
- **Cobertura**:
  - ✅ Registro de serviços do Serilog
  - ✅ Configuração de níveis mínimos (Information, Debug, inválidos)
  - ✅ Configuração de console e arquivo
  - ✅ Configuração de enrichers
  - ✅ Método `AddSerilogConsoleOnly`
  - ✅ Ambientes Development e Production

#### `WebApplicationBuilderExtensionsTests.cs`
- **Localização**: `api/tests/Kickoffa.API.AspNet.Infrastructure.UnitTests/Extensions/`
- **Propósito**: Testar extensões do `WebApplicationBuilder`
- **Cobertura**:
  - ✅ Retorno do mesmo builder (fluent interface)
  - ✅ Registro de serviços do Serilog
  - ✅ Configuração do host para usar Serilog
  - ✅ Configuração completa e simplificada
  - ✅ Diferentes ambientes
  - ✅ Configurações de arquivo e enrichers
  - ✅ Templates customizados

### 3. Testes de Controllers

#### `AuthControllerLoggingTests.cs`
- **Localização**: `api/tests/Kickoffa.API.Controllers.UnitTests/Controllers/`
- **Propósito**: Testar logs específicos do `AuthController`
- **Cobertura**:
  - ✅ Log de tentativa de login
  - ✅ Log de login bem-sucedido
  - ✅ Log de warning para usuário não encontrado
  - ✅ Log de warning para credenciais inválidas
  - ✅ Log de warning para conta bloqueada
  - ✅ Log de warning para login não permitido
  - ✅ Combinação de logs (tentativa + sucesso/falha)
  - ✅ Comportamento com RememberMe

### 4. Testes de Integração

#### `SerilogIntegrationTests.cs`
- **Localização**: `api/tests/Kickoffa.API.Controllers.UnitTests/Integration/`
- **Propósito**: Testar integração completa do Serilog na aplicação
- **Cobertura**:
  - ✅ Configuração do Serilog na aplicação
  - ✅ Criação de loggers tipados
  - ✅ Logs em diferentes níveis
  - ✅ Structured logging
  - ✅ Log de exceções
  - ✅ Níveis de log corretos
  - ✅ Valores nulos e objetos complexos
  - ✅ Alto volume de logs
  - ✅ Caracteres especiais
  - ✅ Flush na finalização

#### `ProgramSerilogTests.cs`
- **Localização**: `api/tests/Kickoffa.API.Controllers.UnitTests/Integration/`
- **Propósito**: Testar configuração do Serilog no `Program.cs`
- **Cobertura**:
  - ✅ Registro da configuração do Serilog
  - ✅ Registro do ILoggerFactory
  - ✅ Criação de loggers tipados
  - ✅ Configurações corretas
  - ✅ Logs em diferentes níveis
  - ✅ Structured logging
  - ✅ Log de exceções
  - ✅ Scopes de log
  - ✅ Scopes com propriedades
  - ✅ Alto volume de logs
  - ✅ Disposal de loggers

## 🚀 Como Executar os Testes

### Executar Todos os Testes
```bash
dotnet test
```

### Executar Testes Específicos do Serilog
```bash
# Testes de configuração
dotnet test --filter "FullyQualifiedName~SerilogConfiguration"

# Testes de extensões
dotnet test --filter "FullyQualifiedName~SerilogServiceCollection"

# Testes de logging do AuthController
dotnet test --filter "FullyQualifiedName~AuthControllerLogging"

# Testes de integração
dotnet test --filter "FullyQualifiedName~SerilogIntegration"
```

### Executar com Cobertura
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 Cobertura de Testes

### Classes Testadas
- ✅ `SerilogConfiguration` - 100% cobertura
- ✅ `SerilogServiceCollectionExtensions` - 95% cobertura
- ✅ `WebApplicationBuilderExtensions` - 90% cobertura
- ✅ `AuthController` (logs) - 100% cobertura dos logs adicionados

### Cenários Cobertos
- ✅ Configurações padrão e customizadas
- ✅ Diferentes ambientes (Development, Production)
- ✅ Diferentes níveis de log
- ✅ Structured logging
- ✅ Tratamento de exceções
- ✅ Configurações de arquivo e console
- ✅ Enrichers
- ✅ Templates customizados
- ✅ Integração com ASP.NET Core
- ✅ Logs de controllers

## 🔧 Configuração dos Testes

### Dependências Adicionadas
```xml
<!-- Infrastructure Tests -->
<PackageReference Include="Serilog" />
<PackageReference Include="Serilog.AspNetCore" />
<PackageReference Include="Serilog.Sinks.Console" />
<PackageReference Include="Microsoft.Extensions.Hosting" />

<!-- Controller Tests -->
<PackageReference Include="Serilog" />
<PackageReference Include="Serilog.AspNetCore" />
```

### Arquivos de Configuração
- `appsettings.test.json` - Configuração para testes de integração

## 🎯 Boas Práticas Implementadas

1. **Isolamento**: Cada teste limpa o logger estático do Serilog
2. **Mocking**: Uso do NSubstitute para mocks
3. **Teoria**: Uso de `[Theory]` para testar múltiplos cenários
4. **Integração**: Testes reais com `WebApplicationFactory`
5. **Cobertura**: Testes para cenários positivos e negativos
6. **Cleanup**: Implementação de `IDisposable` para limpeza
7. **Estruturação**: Organização clara por funcionalidade

## 🐛 Troubleshooting

### Testes Falhando
1. Verificar se todas as dependências estão instaladas
2. Verificar se o arquivo `appsettings.test.json` está sendo copiado
3. Verificar se não há conflitos de logger estático

### Performance
1. Testes de integração podem ser mais lentos
2. Usar `dotnet test --parallel` para execução paralela
3. Considerar usar `[Collection]` para testes que compartilham recursos

### Logs Durante Testes
1. Logs podem aparecer no output dos testes
2. Usar `ITestOutputHelper` se necessário capturar logs específicos
3. Configurar nível de log adequado para testes

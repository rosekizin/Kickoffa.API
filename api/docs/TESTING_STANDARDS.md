# 🧪 Padrões de Testes Unitários - Kickoffa.API

## Visão Geral

Este documento define os padrões e convenções para testes unitários no projeto Kickoffa.API, baseado na estrutura atual implementada e validada.

## 📁 Estrutura de Projetos de Teste

### **Organização por Responsabilidade**
```
api/tests/
├── Kickoffa.API.Controllers.UnitTests/        # 🎯 Testes de Controllers
├── Kickoffa.API.Application.UnitTests/        # 🔧 Testes de Application Layer
├── Kickoffa.API.AspNet.Infrastructure.UnitTests/ # 🏗️ Testes de Infrastructure
├── Kickoffa.API.Data.UnitTests/               # 🗄️ Testes de Data Layer
└── Kickoffa.API.Domain.UnitTests/             # 📋 Testes de Domain Models
```

### **Espelhamento de Estrutura**
Cada projeto de teste **espelha exatamente** a estrutura do projeto principal:
```
src/Kickoffa.API.Application/Services/AppUser/UserService.cs
tests/Kickoffa.API.Application.UnitTests/Services/AppUser/UserServiceTests.cs
```

## 🎯 Padrões de Implementação

### **1. Nomenclatura de Classes**
```csharp
// Padrão: {ClasseName}Tests
public class UserServiceTests
public class AuthControllerTests
public class PostgreDbConfigurationTests
```

### **2. Nomenclatura de Métodos**
```csharp
// Padrão: MethodName_Scenario_ExpectedResult
[Fact]
public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUser()

[Fact]
public void Constructor_WithValidConfiguration_ShouldSetConnectionString()
```

### **3. Estrutura AAA (Arrange, Act, Assert)**
```csharp
[Fact]
public async Task LoginAsync_WithValidCredentials_ShouldReturnOk()
{
    // Arrange
    var request = new LoginRequest { Email = "test@example.com", Password = "password123" };
    var user = new User("test@example.com");
    _userService.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);

    // Act
    var result = await _authController.LoginAsync(request, CancellationToken.None);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    Assert.Equal(user.Id.ToString(), response.UserId);
}
```

## 🔧 Estratégias de Mocking

### **1. NSubstitute para Interfaces**
```csharp
public class AuthControllerTests
{
    private readonly IUserService _userService;
    private readonly ISignInManagerWrapper _signInManagerWrapper;
    
    public AuthControllerTests()
    {
        _userService = Substitute.For<IUserService>();
        _signInManagerWrapper = Substitute.For<ISignInManagerWrapper>();
    }
}
```

### **2. Configuração In-Memory para Configuration**
```csharp
[Fact]
public void GetValue_WithValidKey_ShouldReturnValue()
{
    // Arrange
    var inMemorySettings = new Dictionary<string, string?>
    {
        {"TestKey", "TestValue"},
        {"SectionName:SomeKey", "SectionValue"}
    };

    IConfiguration configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(inMemorySettings)
        .Build();

    _configurationWrapper = new ConfigurationWrapper(configuration);
    
    // Act & Assert...
}
```

### **3. Constantes para Paths de Configuração**
```csharp
public class PostgreDbConfigurationTests
{
    private const string POSTGRE_CONFIGURATION_PORT_PATH = "Postgre:Port";
    private const string POSTGRE_CONFIGURATION_SERVER_PATH = "Postgre:Server";
    private const string POSTGRE_CONFIGURATION_SSL_MODE_PATH = "Postgre:SslMode";
    // ... outras constantes
}
```

## 📋 Padrões de Assertions

### **1. Verificação de Tipos**
```csharp
var okResult = Assert.IsType<OkObjectResult>(result.Result);
var response = Assert.IsType<LoginResponse>(okResult.Value);
```

### **2. Verificação de Chamadas de Mock**
```csharp
await _signInManagerWrapper.Received(1).SignOutAsync();
_userService.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);
```

### **3. Verificação de Propriedades**
```csharp
Assert.Equal(user.Id.ToString(), response.UserId);
Assert.Equal(user.Email, response.Email);
Assert.True(response.Success);
Assert.NotNull(response.Session);
```

## 🎨 Padrões de Dados de Teste

### **1. Theory com InlineData**
```csharp
[Theory]
[InlineData("Key1", "Value1")]
[InlineData("Database:Host", "localhost")]
[InlineData("Logging:LogLevel:Default", "Information")]
public void GetValue_WithDifferentKeys_ShouldReturnCorrectValues(string key, string expectedValue)
```

### **2. Objetos de Teste Realistas**
```csharp
var request = new LoginRequest
{
    Email = "test@example.com",
    Password = "password123",
    RememberMe = false
};
```

### **3. Connection Strings Formatadas**
```csharp
var expectedConnectionString = $"Host={server};Port={port};Database={database};Username={username};Password={password};SSL Mode={sslMode};Trust Server Certificate=true;Timeout={commandTimeout};";
```

## 🔍 Cenários de Teste Obrigatórios

### **1. Para Controllers**
- ✅ **Cenário de Sucesso**: Dados válidos retornam OK
- ✅ **Cenário de Erro**: Dados inválidos retornam erro apropriado
- ✅ **Cenário de Autorização**: Usuário não autenticado retorna Unauthorized
- ✅ **Cenário de Validação**: Campos obrigatórios ausentes

### **2. Para Services**
- ✅ **Cenário de Sucesso**: Operação bem-sucedida
- ✅ **Cenário de Falha**: Dependência falha
- ✅ **Cenário de Dados Inválidos**: Validação de entrada
- ✅ **Cenário de Estado**: Verificação de estado interno

### **3. Para Configuration**
- ✅ **Cenário de Configuração Válida**: Valores corretos
- ✅ **Cenário de Configuração Ausente**: Valores padrão ou erro
- ✅ **Cenário de Tipos Diferentes**: String, int, bool

## 🚀 Melhores Práticas Implementadas

### **✅ Isolamento de Testes**
- Cada teste é independente
- Mocks são configurados por teste
- Sem estado compartilhado

### **✅ Legibilidade**
- Nomes descritivos de métodos
- Comentários AAA claros
- Constantes para valores mágicos

### **✅ Manutenibilidade**
- Estrutura espelhada dos projetos
- Padrões consistentes
- Fácil localização de testes

### **✅ Cobertura Abrangente**
- Cenários positivos e negativos
- Edge cases importantes
- Verificação de comportamento

## 📊 Métricas de Qualidade

### **Status Atual dos Testes**
```
✅ Kickoffa.API.Controllers.UnitTests: 7 testes passando
✅ Kickoffa.API.Application.UnitTests: Estrutura implementada
✅ Kickoffa.API.AspNet.Infrastructure.UnitTests: 3 testes implementados
✅ Kickoffa.API.Data.UnitTests: Estrutura criada
✅ Kickoffa.API.Domain.UnitTests: Estrutura criada
```

### **Padrões de Qualidade**
- **Nomenclatura**: 100% consistente
- **Estrutura AAA**: 100% implementada
- **Mocking**: NSubstitute padrão
- **Assertions**: Específicas e claras

## 🔄 Evolução dos Padrões

### **Versão Atual (v2.0)**
- ✅ Interfaces centralizadas em `Application.Interfaces`
- ✅ Mocking correto de dependências
- ✅ Configuration in-memory para testes
- ✅ Constantes para paths de configuração
- ✅ Verificação de tipos específicos

### **Melhorias Implementadas**
- **Antes**: Mocking incorreto de classes concretas
- **Agora**: Mocking correto de interfaces
- **Antes**: Testes misturados em projetos
- **Agora**: Separação clara por responsabilidade

---

📝 **Nota**: Esta é a versão oficial e atualizada dos padrões de teste. Todos os novos testes devem seguir estas convenções.

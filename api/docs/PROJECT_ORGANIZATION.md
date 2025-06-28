# 🏗️ Organização do Projeto Kickoffa.API

## Visão Geral

Este documento descreve a estrutura organizacional do projeto Kickoffa.API, incluindo a separação de responsabilidades, organização de interfaces e estratégia de testes.

## 📁 Estrutura de Projetos

### Projetos Principais

```
api/src/
├── Kickoffa.API/                           # 🎯 API Controllers e Startup
├── Kickoffa.API.Application/               # 🔧 Lógica de Negócio
├── Kickoffa.API.AspNet.Infrastructure/     # 🏗️ Infraestrutura ASP.NET
├── Kickoffa.API.Data/                      # 🗄️ Acesso a Dados
├── Kickoffa.API.Domain/                    # 📋 Modelos de Domínio
└── Kickoffa.API.Contracts/                 # 📡 Contratos de API
```

### Projetos de Teste

```
api/tests/
├── Kickoffa.API.Controllers.UnitTests/           # 🧪 Testes de Controllers
├── Kickoffa.API.Application.UnitTests/           # 🧪 Testes de Application
├── Kickoffa.API.AspNet.Infrastructure.UnitTests/ # 🧪 Testes de Infrastructure
├── Kickoffa.API.Data.UnitTests/                  # 🧪 Testes de Data
└── Kickoffa.API.Domain.UnitTests/                # 🧪 Testes de Domain
```

## 🔧 Organização do Projeto Application

### Estrutura de Pastas

```
Kickoffa.API.Application/
├── Interfaces/              # 📋 Todas as interfaces centralizadas
│   ├── IUserService.cs
│   ├── ICustomerService.cs
│   ├── IEmailService.cs
│   ├── ICreateCustomerFactory.cs
│   ├── ISignInManagerWrapper.cs
│   └── IUserManagerWrapper.cs
├── Services/                # 🔧 Implementações de serviços
│   ├── AppUser/
│   │   └── UserService.cs
│   ├── Email/
│   │   └── EmailService.cs
│   └── CustomerService.cs
├── Wrappers/                # 🎭 Wrappers para testabilidade
│   ├── SignInManagerWrapper.cs
│   └── UserManagerWrapper.cs
└── Factories/               # 🏭 Factories para criação de objetos
    └── CreateCustomerFactory.cs
```

### 🎯 Benefícios da Centralização de Interfaces

#### ✅ **Organização**
- **Localização única**: Todas as interfaces em um só lugar
- **Fácil navegação**: Desenvolvedores sabem onde encontrar contratos
- **Visão geral**: Entendimento rápido de todas as abstrações

#### ✅ **Manutenibilidade**
- **Mudanças centralizadas**: Alterações de interface em local único
- **Versionamento claro**: Histórico de mudanças de contratos
- **Refatoração simplificada**: IDEs encontram implementações facilmente

#### ✅ **Desenvolvimento**
- **IntelliSense melhorado**: Autocomplete mais eficiente
- **Menos conflitos**: Reduz merge conflicts em diferentes pastas
- **Padrão consistente**: Todos seguem mesma estrutura

## 🧪 Estratégia de Testes

### Separação por Responsabilidade

#### **Controllers Tests**
```
Kickoffa.API.Controllers.UnitTests/
└── AuthControllerTests.cs          # Testa apenas lógica de controller
```

**Responsabilidades:**
- Validação de entrada
- Mapeamento de responses
- Códigos de status HTTP
- Autorização e autenticação

#### **Application Tests**
```
Kickoffa.API.Application.UnitTests/
└── Services/
    ├── AppUser/
    │   └── UserServiceTests.cs     # Lógica de negócio de usuários
    ├── Email/
    │   └── EmailServiceTests.cs    # Lógica de envio de emails
    └── CustomerServiceTests.cs     # Lógica de negócio de clientes
```

**Responsabilidades:**
- Regras de negócio
- Validações de domínio
- Orquestração de operações
- Transformações de dados

#### **Infrastructure Tests**
```
Kickoffa.API.AspNet.Infrastructure.UnitTests/
├── Configuration/
│   └── Data/
│       └── PostgreDbConfigurationTests.cs
├── Wrappers/
│   └── ConfigurationWrapperTests.cs
└── Extensions/
    └── ServiceCollection/
        └── ApplicationServiceCollectionExtensionsTests.cs
```

**Responsabilidades:**
- Configurações
- Wrappers de framework
- Extensions methods
- Injeção de dependência

### 📋 Padrões de Teste

#### **Nomenclatura**
```csharp
// Padrão: MethodName_Scenario_ExpectedResult
[Fact]
public void LoginAsync_WithValidCredentials_ShouldReturnOk()

[Theory]
[InlineData("valid@email.com", true)]
[InlineData("invalid-email", false)]
public void IsEmailValid_WithDifferentInputs_ShouldReturnExpectedResult(string email, bool expected)
```

#### **Estrutura AAA (Arrange, Act, Assert)**
```csharp
[Fact]
public void CreateCustomer_WithValidData_ShouldReturnCustomer()
{
    // Arrange
    var customerData = new CreateCustomerRequest { ... };
    _repository.CreateAsync(Arg.Any<Customer>()).Returns(expectedCustomer);

    // Act
    var result = await _customerService.CreateAsync(...);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedCustomer.Id, result.Id);
}
```

#### **Mocking com NSubstitute**
```csharp
// Configuração de mocks
_userService = Substitute.For<IUserService>();
_signInManagerWrapper = Substitute.For<ISignInManagerWrapper>();

// Verificação de chamadas
await _signInManagerWrapper.Received(1).SignOutAsync();
_userService.DidNotReceive().CreateAsync(Arg.Any<string>(), Arg.Any<string>());
```

## 🔄 Namespaces e Imports

### Padrão de Namespaces

#### **Interfaces**
```csharp
namespace Kickoffa.API.Application.Interfaces;
```

#### **Implementações**
```csharp
using Kickoffa.API.Application.Interfaces;  // Import das interfaces

namespace Kickoffa.API.Application.Services.AppUser;
```

#### **Controllers**
```csharp
using Kickoffa.API.Application.Interfaces;  // Import das interfaces

namespace Kickoffa.API.Controllers;
```

### 🎯 Benefícios da Padronização

- **Consistência**: Todos os projetos seguem mesmo padrão
- **Facilidade**: Imports previsíveis e organizados
- **Manutenção**: Mudanças de namespace centralizadas
- **Tooling**: Melhor suporte de IDEs e ferramentas

## 📊 Métricas de Qualidade

### Cobertura de Testes
- **Meta**: 80%+ de cobertura de código
- **Foco**: Lógica de negócio e casos críticos
- **Exclusões**: DTOs, modelos simples, configurações

### Organização de Código
- **✅ Interfaces centralizadas**: Implementado
- **✅ Testes separados por responsabilidade**: Implementado
- **✅ Namespaces consistentes**: Implementado
- **✅ Documentação atualizada**: Implementado
- **✅ Projetos adicionados na solution**: Implementado
- **✅ Build funcionando**: Todos os projetos principais compilando
- **✅ Testes executando**: 7 testes passando no AuthController

## 🚀 Próximos Passos

### Melhorias Planejadas
1. **Testes de Integração**: Criar projeto para testes end-to-end
2. **Performance Tests**: Adicionar testes de carga
3. **Architecture Tests**: Validar dependências entre camadas
4. **Code Analysis**: Configurar analyzers para qualidade

### Padrões a Implementar
1. **Result Pattern**: Para tratamento de erros
2. **CQRS**: Separação de comandos e queries
3. **Domain Events**: Para comunicação entre agregados
4. **Specification Pattern**: Para queries complexas

---

📝 **Nota**: Esta organização segue princípios de Clean Architecture e Domain-Driven Design, garantindo separação clara de responsabilidades e alta testabilidade.

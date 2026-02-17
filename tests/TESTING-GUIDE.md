# 🧪 Guia Completo de Testes - PHCWEBAPI

## 📋 Índice
1. [Visão Geral](#visão-geral)
2. [Stack de Testes](#stack-de-testes)
3. [Como Executar](#como-executar)
4. [Padrões e Boas Práticas](#padrões-e-boas-práticas)
5. [Exemplos de Uso](#exemplos-de-uso)
6. [AAA Pattern](#aaa-pattern)
7. [Coverage](#coverage)
8. [CI/CD Integration](#cicd-integration)

---

## 🎯 Visão Geral

Este projeto utiliza uma **stack profissional** para testes em .NET:

```
xUnit          → Framework de testes
AutoFixture    → Geração automática de dados
AutoMoq        → Criação automática de mocks
Moq            → Mocking library
FluentAssertions → Assertions legíveis
Bogus          → Dados realistas (opcional)
```

### Por que essa combinação?

| Ferramenta | Benefício |
|------------|-----------|
| **xUnit** | Padrão Microsoft, performance, paralelismo |
| **AutoFixture** | Elimina 80% do código de setup de testes |
| **AutoMoq** | Cria mocks automaticamente via DI |
| **Moq** | Mocking poderoso e simples |
| **FluentAssertions** | Assertions legíveis e ricas em detalhes |
| **Theory** | Testa múltiplos cenários sem duplicar código |

---

## 📦 Stack de Testes

### Pacotes Instalados

```xml
<!-- Framework -->
<PackageReference Include="xunit" Version="2.9.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />

<!-- AutoFixture -->
<PackageReference Include="AutoFixture" Version="4.18.1" />
<PackageReference Include="AutoFixture.Xunit2" Version="4.18.1" />
<PackageReference Include="AutoFixture.AutoMoq" Version="4.18.1" />

<!-- Mocking -->
<PackageReference Include="Moq" Version="4.20.72" />

<!-- Assertions -->
<PackageReference Include="FluentAssertions" Version="6.12.2" />

<!-- Dados Realistas (opcional) -->
<PackageReference Include="Bogus" Version="35.6.1" />

<!-- Coverage -->
<PackageReference Include="coverlet.collector" Version="6.0.2" />
```

---

## 🚀 Como Executar

### 1. **Via CLI (PowerShell/Terminal)**

```powershell
# Executar TODOS os testes
cd tests/Parameters.Application.Tests
dotnet test

# Executar testes com detalhes
dotnet test --verbosity detailed

# Executar testes de uma classe específica
dotnet test --filter "FullyQualifiedName~CreateParameterCommandHandlerTests"

# Executar apenas um teste
dotnet test --filter "DisplayName=Handle_ValidCommand_ShouldCreateParameterAndReturnDto"

# Gerar relatório de coverage
dotnet test --collect:"XPlat Code Coverage"
```

### 2. **Via Visual Studio**

1. **Test Explorer**: `View` → `Test Explorer` (ou `Ctrl+E, T`)
2. **Run All Tests**: Botão "Run All" no Test Explorer
3. **Debug Test**: Right-click no teste → "Debug"
4. **Live Unit Testing**: `Test` → `Live Unit Testing` → `Start`

### 3. **Via VS Code**

1. Instalar extensão: `.NET Core Test Explorer`
2. Sidebar: Ícone de testes Flask 🧪
3. Clicar em "Run All Tests"

### 4. **Executar Testes em Watch Mode**

```powershell
# Testes rodam automaticamente ao salvar arquivos
dotnet watch test
```

---

## 📖 Padrões e Boas Práticas

### 1️⃣ **AAA Pattern (Arrange-Act-Assert)**

```csharp
[Fact]
public async Task Handle_ValidCommand_ShouldCreateParameter()
{
    // ✅ ARRANGE: Preparar dados e configurar mocks
    var inputDto = new CreateParameterInputDTO { /** ... **/ };
    var mockRepository = new Mock<IPara1Repository>();
    mockRepository.Setup(r => r.AddAsync(...)).ReturnsAsync(...);
    
    // ✅ ACT: Executar ação sendo testada
    var result = await handler.Handle(command, CancellationToken.None);
    
    // ✅ ASSERT: Verificar resultado
    result.Should().NotBeNull();
    result.Descricao.Should().Be(inputDto.Descricao);
}
```

### 2️⃣ **Nomenclatura de Testes**

**Padrão:** `MethodName_Scenario_ExpectedBehavior`

```csharp
✅ GOOD:
Handle_ValidCommand_ShouldCreateParameterAndReturnDto
Handle_EmptyDescription_ShouldThrowValidationException
Validate_DescricaoTooLong_ShouldHaveValidationError

❌ BAD:
Test1
CreateTest
ValidateDescricao
```

### 3️⃣ **Usar AutoFixture para Setup**

```csharp
// ❌ BAD: Setup manual (verbose)
var inputDto = new CreateParameterInputDTO 
{ 
    Descricao = "Test", 
    Valor = "Value", 
    Tipo = "T", 
    Dec = 2, 
    Tam = 10 
};

// ✅ GOOD: AutoFixture gera automaticamente
[Theory]
[AutoMoqData]
public async Task Handle_ValidCommand_ShouldWork(
    CreateParameterInputDTO inputDto, // ✅ Gerado automaticamente
    [Frozen] Mock<IPara1Repository> mockRepo,
    CreateParameterCommandHandler sut) // ✅ System Under Test
{
    // Teste já começa com tudo configurado!
}
```

### 4️⃣ **Theory para Múltiplos Cenários**

```csharp
// ❌ BAD: Duplicar testes
[Fact]
public void Validate_Descricao50Chars_ShouldBeValid() { /** ... **/ }

[Fact]
public void Validate_Descricao100Chars_ShouldBeValid() { /** ... **/ }

[Fact]
public void Validate_Descricao101Chars_ShouldBeInvalid() { /** ... **/ }

// ✅ GOOD: Theory com InlineData
[Theory]
[InlineData(50, false)]   // tamanho, deveSerInvalido
[InlineData(100, false)]
[InlineData(101, true)]
[InlineData(200, true)]
public void Validate_DescricaoLength_ShouldValidateCorrectly(
    int length, 
    bool shouldHaveError)
{
    // Um teste para múltiplos cenários!
}
```

### 5️⃣ **FluentAssertions para Legibilidade**

```csharp
// ❌ BAD: Assertions nativas
Assert.NotNull(result);
Assert.Equal(expected.Descricao, result.Descricao);
Assert.True(result.IsActive);

// ✅ GOOD: FluentAssertions
result.Should().NotBeNull("o handler deve retornar um resultado");
result.Descricao.Should().Be(expected.Descricao);
result.IsActive.Should().BeTrue();

// ✅ EXCELLENT: Comparação de objetos complexos
result.Should().BeEquivalentTo(expected, options => options
    .Excluding(x => x.CreatedAt) // Excluir campos específicos
    .Excluding(x => x.UpdatedAt));
```

### 6️⃣ **Moq: Verificar Chamadas**

```csharp
// ✅ Setup de Mock
mockRepository
    .Setup(r => r.AddAsync(It.IsAny<Para1>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(savedEntity);

// ✅ Verificar que foi chamado
mockRepository.Verify(
    r => r.AddAsync(It.IsAny<Para1>(), It.IsAny<CancellationToken>()),
    Times.Once, // Exatamente uma vez
    "o repositório deve ser chamado para salvar");

// ✅ Verificar argumento específico
mockRepository.Verify(
    r => r.AddAsync(
        It.Is<Para1>(p => p.Descricao == "Test"), // ✅ Verificar propriedade
        It.IsAny<CancellationToken>()),
    Times.Once);
```

### 7️⃣ **Capturar Argumentos do Mock**

```csharp
// ✅ Capturar entidade passada ao repositório
Para1? capturedEntity = null;

mockRepository
    .Setup(r => r.AddAsync(It.IsAny<Para1>(), It.IsAny<CancellationToken>()))
    .Callback<Para1, CancellationToken>((entity, _) => capturedEntity = entity)
    .ReturnsAsync((Para1 entity, CancellationToken _) => entity);

// Act
await handler.Handle(command, CancellationToken.None);

// Assert
capturedEntity.Should().NotBeNull();
capturedEntity!.Descricao.Should().Be("Expected Value");
```

### 8️⃣ **Testar Exceções**

```csharp
// ✅ GOOD: FluentAssertions para exceções
var act = async () => await handler.Handle(command, CancellationToken.None);

await act.Should().ThrowAsync<ValidationException>()
    .WithMessage("*obrigatória*") // Partial match
    .Where(ex => ex.Errors.Count() > 0);
```

### 9️⃣ **Frozen Mocks (Dependency Injection)**

```csharp
// ✅ [Frozen] garante que o mesmo mock é injetado no SUT
[Theory]
[AutoMoqData]
public async Task Test(
    [Frozen] Mock<IPara1Repository> mockRepo, // ✅ Mock
    CreateParameterCommandHandler sut)        // ✅ Handler recebe o mock acima
{
    // mockRepo já está injetado no sut automaticamente!
}
```

### 🔟 **Testes de Validators com FluentValidation.TestHelper**

```csharp
[Fact]
public void Validate_EmptyDescricao_ShouldHaveError()
{
    // Arrange
    var validator = new CreateParameterCommandValidator();
    var command = new CreateParameterCommand(
        new CreateParameterInputDTO { Descricao = "" }, 
        "user");

    // Act & Assert
    var result = validator.TestValidate(command);
    
    result.ShouldHaveValidationErrorFor(c => c.Dto.Descricao)
        .WithErrorMessage("*obrigatória*")
        .Only(); // ✅ Garante que é o único erro
}

[Fact]
public void Validate_ValidCommand_ShouldNotHaveErrors()
{
    var validator = new CreateParameterCommandValidator();
    var command = /** ... **/;
    
    var result = validator.TestValidate(command);
    result.ShouldNotHaveAnyValidationErrors();
}
```

---

## 💡 Exemplos de Uso

### 📝 **Exemplo 1: Unit Test Básico**

```csharp
[Theory]
[AutoMoqData]
public async Task Handle_ValidCommand_ShouldCreateParameter(
    [Frozen] Mock<IPara1Repository> mockRepo,
    CreateParameterCommandHandler sut,
    CreateParameterInputDTO inputDto,
    Para1 savedEntity)
{
    // Arrange
    mockRepo
        .Setup(r => r.AddAsync(It.IsAny<Para1>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(savedEntity);

    var command = new CreateParameterCommand(inputDto, "test_user");

    // Act
    var result = await sut.Handle(command, CancellationToken.None);

    // Assert
    result.Should().NotBeNull();
    result.Para1Stamp.Should().Be(savedEntity.Para1Stamp);
    
    mockRepo.Verify(
        r => r.AddAsync(It.IsAny<Para1>(), It.IsAny<CancellationToken>()),
        Times.Once);
}
```

### 📝 **Exemplo 2: Teste Parametrizado (Theory)**

```csharp
[Theory]
[InlineAutoMoqData("Parâmetro 1")]
[InlineAutoMoqData("Parameter with special chars !@#")]
[InlineAutoMoqData("Parâmetro com acentuação")]
public async Task Handle_DifferentDescriptions_ShouldWork(
    string descricao,
    [Frozen] Mock<IPara1Repository> mockRepo,
    CreateParameterCommandHandler sut)
{
    // Arrange
    var inputDto = new CreateParameterInputDTO { Descricao = descricao, /** ... **/ };
    mockRepo.Setup(/** ... **/);

    // Act
    var result = await sut.Handle(new CreateParameterCommand(inputDto, "user"), default);

    // Assert
    result.Descricao.Should().Be(descricao);
}
```

### 📝 **Exemplo 3: Teste de Validator**

```csharp
[Theory]
[InlineData(50, false)]   // length, shouldHaveError
[InlineData(100, false)]
[InlineData(101, true)]
public void Validate_DescricaoLength_ShouldValidateCorrectly(
    int length, 
    bool shouldHaveError)
{
    // Arrange
    var validator = new CreateParameterCommandValidator();
    var dto = new CreateParameterInputDTO 
    { 
        Descricao = new string('A', length),
        /** ... **/
    };
    var command = new CreateParameterCommand(dto, "user");

    // Act
    var result = validator.TestValidate(command);

    // Assert
    if (shouldHaveError)
        result.ShouldHaveValidationErrorFor(c => c.Dto.Descricao);
    else
        result.ShouldNotHaveValidationErrorFor(c => c.Dto.Descricao);
}
```

### 📝 **Exemplo 4: Teste de Coleções**

```csharp
[Theory]
[AutoMoqData]
public async Task Handle_MultipleItems_ShouldReturnAll(
    [Frozen] Mock<IPara1Repository> mockRepo,
    GetAllParametersQueryHandler sut,
    List<Para1> entities)
{
    // Arrange
    mockRepo
        .Setup(r => r.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(entities);

    // Act
    var result = await sut.Handle(new GetAllParametersQuery(false), default);

    // Assert
    result.Should().HaveCount(entities.Count);
    result.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Descricao));
}
```

---

## 📊 Coverage

### Gerar Relatório de Coverage

```powershell
# 1. Executar testes com coverage
dotnet test --collect:"XPlat Code Coverage"

# 2. Instalar ReportGenerator (uma vez)
dotnet tool install -g dotnet-reportgenerator-globaltool

# 3. Gerar relatório HTML
reportgenerator `
  -reports:"**/coverage.cobertura.xml" `
  -targetdir:"CoverageReport" `
  -reporttypes:Html

# 4. Abrir relatório
start CoverageReport/index.html
```

### Metas de Coverage

| Tipo | Meta | Crítico |
|------|------|---------|
| **Handlers** | 90%+ | ✅ Critical |
| **Validators** | 95%+ | ✅ Critical |
| **Mappers** | 80%+ | ⚠️ Medium |
| **DTOs** | 70%+ | ℹ️ Low |

---

## 🔄 CI/CD Integration

### GitHub Actions (exemplo)

```yaml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore
      
      - name: Test
        run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
      
      - name: Code Coverage Report
        uses: codecov/codecov-action@v3
```

---

## 🎯 Estrutura de Projetos de Teste

```
tests/
├── Parameters.Application.Tests/
│   ├── Fixtures/
│   │   └── AutoMoqDataAttribute.cs        ← Custom AutoData
│   ├── Features/
│   │   ├── CreateParameter/
│   │   │   ├── CreateParameterCommandHandlerTests.cs
│   │   │   └── CreateParameterCommandValidatorTests.cs
│   │   ├── GetAllParameters/
│   │   │   └── GetAllParametersQueryHandlerTests.cs
│   │   └── UpdateParameter/
│   │       └── UpdateParameterCommandHandlerTests.cs
│   └── Parameters.Application.Tests.csproj
│
├── Parameters.Domain.Tests/
│   ├── Entities/
│   │   └── Para1Tests.cs                  ← Testes de entidades
│   └── Parameters.Domain.Tests.csproj
│
└── Parameters.Infrastructure.Tests/
    ├── Repositories/
    │   └── Para1RepositoryTests.cs        ← Testes de integração
    └── Parameters.Infrastructure.Tests.csproj
```

---

## 🚨 Troubleshooting

### Problema: "Test discovery failed"

**Solução:**
```powershell
dotnet clean
dotnet build
dotnet test
```

### Problema: AutoFixture não gera valores

**Solução:** Criar customização específica
```csharp
fixture.Customize<MyClass>(c => c
    .FromFactory(() => new MyClass(/** custom values **/))
    .OmitAutoProperties());
```

### Problema: Mock não está sendo chamado

**Solução:** Verificar setup
```csharp
// ✅ Use It.IsAny<> para permitir qualquer valor
mockRepo.Setup(r => r.Method(It.IsAny<Type>()))

// ❌ Evite valores específicos a menos que necessário
mockRepo.Setup(r => r.Method(specificValue))
```

---

## 📚 Recursos Adicionais

- [xUnit Documentation](https://xunit.net/)
- [AutoFixture Cheat Sheet](https://github.com/AutoFixture/AutoFixture/wiki/Cheat-Sheet)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [FluentAssertions Documentation](https://fluentassertions.com/introduction)
- [Test Naming Conventions](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

---

**✅ Com essa estrutura, você tem testes profissionais, manuteníveis e de alta qualidade!**
# Executar todos os testes
cd tests/Parameters.Application.Tests
dotnet test

# Com detalhes
dotnet test --verbosity detailed

# Watch mode (auto-rerun ao salvar)
dotnet watch test

# Coverage report
dotnet test --collect:"XPlat Code Coverage"
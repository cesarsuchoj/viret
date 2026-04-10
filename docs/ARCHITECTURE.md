# Arquitetura do Viret

## Visão Geral

O **Viret** adota uma **arquitetura em camadas** (Layered Architecture) combinada com o padrão **MVVM** (Model-View-ViewModel) na camada de apresentação. Esta abordagem promove separação de responsabilidades, testabilidade e manutenibilidade.

```
┌──────────────────────────────────────────┐
│           Viret.Maui (UI / MVVM)         │
│   Views ──► ViewModels ──► Services      │
├──────────────────────────────────────────┤
│            Viret.Core (Domínio)          │
│   Models │ Interfaces │ Services         │
├──────────────────────────────────────────┤
│         Viret.Data (Persistência)        │
│ ViretDbContext │ Repositories (EF Core)  │
└──────────────────────────────────────────┘
```

---

## Projetos e Responsabilidades

### `Viret.Core`
Contém a **lógica de negócio central** do sistema. Não possui dependências de frameworks externos (apenas .NET BCL).

| Pasta | Conteúdo |
|---|---|
| `Models/` | Entidades do domínio (`Transaction`, `Family`, `User`, `FamilyMember`, `TransactionType`, `FamilyRole`) |
| `Interfaces/` | Contratos para repositórios e serviços (`IRepository<T>`, `ITransactionRepository`, `IFamilyRepository`, `IUserRepository`, `IFamilyMemberRepository`, `ITransactionService`, `IFamilyService`, `IUserService`) |
| `Services/` | Implementações da lógica de negócio (`TransactionService`, `FamilyService`, `UserService`) |

### `Viret.Data`
Responsável pela **persistência de dados** usando Entity Framework Core com SQLite.

| Pasta / Arquivo | Conteúdo |
|---|---|
| `ViretDbContext.cs` | Contexto EF Core com configuração das entidades |
| `Migrations/` | Migrações EF Core aplicadas automaticamente na inicialização |
| `ServiceCollectionExtensions.cs` | Registro de DI, migração automática e seed inicial |
| `Repositories/` | Implementações concretas de `ITransactionRepository`, `IFamilyRepository`, `IUserRepository` e `IFamilyMemberRepository` |

### `Viret.Maui`
Camada de **interface de usuário** construída com .NET MAUI seguindo o padrão MVVM.

| Pasta / Arquivo | Conteúdo |
|---|---|
| `MauiProgram.cs` | Ponto de entrada; configura o `MauiApp` e registra todos os serviços via DI |
| `ViewModels/` | `BaseViewModel`, `MainViewModel`, `LoginViewModel`, `RegisterViewModel`, `FamilySelectionViewModel` |
| `Views/` | Páginas XAML ligadas aos ViewModels |

### `Viret.Tests`
Testes unitários com **xUnit** e **Moq**. Cobrem os serviços de negócio da camada `Viret.Core`.

---

## Padrões de Projeto Utilizados

### MVVM (Model-View-ViewModel)
- **View**: páginas XAML, sem lógica de negócio
- **ViewModel**: herda de `BaseViewModel` (`ObservableObject`); expõe propriedades observáveis via `[ObservableProperty]` e comandos via `[RelayCommand]` (CommunityToolkit.Mvvm)
- **Model**: entidades em `Viret.Core.Models`

### Repository Pattern
Isola o acesso a dados. Os serviços dependem das interfaces de repositório (`ITransactionRepository`, `IFamilyRepository`, `IUserRepository`, `IFamilyMemberRepository`) e não das implementações concretas.

### Dependency Injection (DI)
A injeção de dependências é configurada em `MauiProgram.cs` usando o contêiner nativo do .NET (`Microsoft.Extensions.DependencyInjection`):

```csharp
// Dados (DbContext + Repositórios)
builder.Services.AddViretData(dbPath);

// Serviços de negócio
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IFamilyService, FamilyService>();
builder.Services.AddScoped<IUserService, UserService>();

// ViewModels
builder.Services.AddTransient<MainViewModel>();
builder.Services.AddTransient<LoginViewModel>();
builder.Services.AddTransient<RegisterViewModel>();
builder.Services.AddTransient<FamilySelectionViewModel>();
```

---

## Diagrama de Dependências

```
Viret.Maui
  └── Viret.Core   (Interfaces + Models)
  └── Viret.Data   (Repositórios + DbContext)
        └── Viret.Core

Viret.Tests
  └── Viret.Core
  └── Viret.Data
```

A camada `Viret.Core` **não depende** de nenhuma outra camada do projeto, garantindo que a lógica de negócio seja isolada e facilmente testável.

---

## Stack Tecnológico

| Tecnologia | Versão | Uso |
|---|---|---|
| .NET MAUI | 8.0 | Interface multiplataforma |
| CommunityToolkit.Mvvm | 8.2.2 | Suporte MVVM com source generators |
| Entity Framework Core | 8.0 | ORM para acesso a dados |
| SQLite | via EF Core | Banco de dados local embarcado |
| xUnit | 2.5.3 | Testes unitários |
| Moq | 4.20.70 | Mocks para testes |

---

## Decisões Arquiteturais

### ADR-001 – Separação em projetos independentes
**Contexto:** A aplicação precisa ser testável e manutenível.  
**Decisão:** Dividir em `Core`, `Data` e `Maui`, seguindo o princípio de inversão de dependências.  
**Consequência:** `Core` pode ser testado sem dependência de banco de dados ou UI.

### ADR-002 – CommunityToolkit.Mvvm para MVVM
**Contexto:** Reduzir boilerplate de `INotifyPropertyChanged` e comandos.  
**Decisão:** Usar `[ObservableProperty]` e `[RelayCommand]` do CommunityToolkit.  
**Consequência:** ViewModels mais concisos e com menos código manual.

### ADR-003 – SQLite como banco de dados local
**Contexto:** A aplicação precisa funcionar offline em dispositivos móveis e desktop.  
**Decisão:** SQLite via EF Core Sqlite provider, com caminho configurável via DI.  
**Consequência:** Sem necessidade de servidor externo; dados ficam no dispositivo.

### ADR-004 – Generic Repository com interfaces no Core
**Contexto:** Desacoplar lógica de negócio do acesso a dados.  
**Decisão:** Definir `IRepository<TEntity, TId>` em `Viret.Core.Interfaces`, implementado em `Viret.Data`.  
**Consequência:** Serviços dependem de abstrações, facilitando testes com mocks.

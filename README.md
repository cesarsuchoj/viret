# Viret

> Um software para gestão financeira familiar, controle de contas da casa.

[![Build](https://github.com/cesarsuchoj/viret/actions/workflows/ci.yml/badge.svg)](https://github.com/cesarsuchoj/viret/actions/workflows/ci.yml)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](LICENSE)

## Sobre o Projeto

O **Viret** é uma aplicação multiplataforma desenvolvida com **.NET MAUI** para ajudar famílias a gerenciar suas finanças de forma simples e eficiente. Com ele é possível registrar ganhos e gastos, acompanhar o saldo disponível e compartilhar informações financeiras entre os membros da família.

## Funcionalidades Principais

- 💰 **Rastreamento de ganhos** – registre receitas previstas e efetivas
- 💸 **Rastreamento de gastos** – controle despesas previstas e efetivas
- 📊 **Visão de recursos disponíveis** – acompanhe o saldo em tempo real
- 👨‍👩‍👧 **Controle por família** – suporte a múltiplas famílias por usuário
- 🔗 **Compartilhamento familiar** – compartilhe dados financeiros com familiares
- 📈 **Relatórios e dashboards** – visualize sua saúde financeira
- 🗄️ **Persistência local** – dados armazenados com SQLite no dispositivo

## Stack Tecnológico

| Tecnologia | Uso |
|---|---|
| [.NET MAUI](https://learn.microsoft.com/dotnet/maui/) | Interface multiplataforma (Android, iOS, Windows, macOS) |
| [Entity Framework Core](https://learn.microsoft.com/ef/core/) | ORM para acesso a dados |
| [SQLite](https://www.sqlite.org/) | Banco de dados local |
| [MVVM Community Toolkit](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) | Padrão MVVM e geração de código |
| [xUnit](https://xunit.net/) | Testes unitários |

## Arquitetura

O projeto segue uma arquitetura em camadas com os seguintes subprojetos:

| Projeto | Descrição |
|---|---|
| `Viret.Core` | Lógica de negócio, entidades e interfaces |
| `Viret.Data` | Persistência de dados com EF Core e SQLite |
| `Viret.Maui` | Interface de usuário .NET MAUI |
| `Viret.Tests` | Testes unitários com xUnit |

## Estrutura de Pastas

```
viret/
├── src/
│   ├── Viret.Core/          # Lógica de negócio e entidades
│   ├── Viret.Data/          # Acesso a dados e migrações
│   └── Viret.Maui/          # Aplicação MAUI (UI)
├── tests/
│   └── Viret.Tests/         # Testes unitários
├── docs/                    # Documentação adicional
├── .github/
│   └── workflows/           # Pipelines de CI/CD
├── Viret.slnx               # Solução .NET
├── .gitignore
├── LICENSE
└── README.md
```

## Como Começar

### Pré-requisitos

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) com carga de trabalho **.NET MAUI** instalada  
  _ou_ [Visual Studio Code](https://code.visualstudio.com/) com a extensão [.NET MAUI](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-maui)
- Workload MAUI instalada via CLI: `dotnet workload install maui`

### Instalação

```bash
# Clone o repositório
git clone https://github.com/cesarsuchoj/viret.git
cd viret

# Restaure as dependências
dotnet restore

# Execute os testes
dotnet test

# Execute a aplicação (Android)
dotnet run --project src/Viret.Maui -f net8.0-android

# Execute a aplicação (Windows)
dotnet run --project src/Viret.Maui -f net8.0-windows10.0.19041.0
```

## Contribuição

Contribuições são bem-vindas! Consulte o arquivo [CONTRIBUTING.md](CONTRIBUTING.md) (em breve) para diretrizes de como contribuir com o projeto.

1. Faça um fork do repositório
2. Crie um branch para sua funcionalidade (`git checkout -b feature/minha-funcionalidade`)
3. Commit suas alterações (`git commit -m 'feat: adiciona minha funcionalidade'`)
4. Faça push para o branch (`git push origin feature/minha-funcionalidade`)
5. Abra um Pull Request

## Licença

Este projeto está licenciado sob a **GNU General Public License v3.0** – veja o arquivo [LICENSE](LICENSE) para mais detalhes.

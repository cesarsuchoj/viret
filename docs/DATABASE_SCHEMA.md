# DATABASE_SCHEMA

## Banco local

- **Provider:** SQLite
- **ORM:** Entity Framework Core
- **Contexto:** `ViretDbContext`
- **Migrações:** aplicadas automaticamente com `Database.Migrate()` na inicialização (`InitializeViretData`)

## Tabelas

### `Families`

| Coluna | Tipo | Restrições |
|---|---|---|
| `Id` | `INTEGER` | PK, autoincrement |
| `Name` | `TEXT` | obrigatório, máximo 100 |

### `Transactions`

| Coluna | Tipo | Restrições |
|---|---|---|
| `Id` | `INTEGER` | PK, autoincrement |
| `Description` | `TEXT` | obrigatório, máximo 200 |
| `Amount` | `TEXT` | obrigatório, precisão 18,2 |
| `Date` | `TEXT` | obrigatório |
| `Type` | `INTEGER` | obrigatório (`TransactionType`) |
| `FamilyId` | `INTEGER` | FK -> `Families.Id`, delete cascade |

### `Users`

| Coluna | Tipo | Restrições |
|---|---|---|
| `Id` | `INTEGER` | PK, autoincrement |
| `Name` | `TEXT` | obrigatório, máximo 100 |
| `Email` | `TEXT` | obrigatório, máximo 200, único |
| `PasswordHash` | `TEXT` | obrigatório, máximo 500 |

### `FamilyMembers`

| Coluna | Tipo | Restrições |
|---|---|---|
| `UserId` | `INTEGER` | PK composta, FK -> `Users.Id`, delete cascade |
| `FamilyId` | `INTEGER` | PK composta, FK -> `Families.Id`, delete cascade |
| `Role` | `INTEGER` | obrigatório (`FamilyRole`) |

## Seed inicial

Dados de exemplo inseridos na inicialização (`InitializeViretData(seedSampleData: true)`) quando o banco está vazio:

- Família `Família Exemplo`
- Transação de receita: `Salário`, `3500`
- Transação de despesa: `Mercado`, `420`

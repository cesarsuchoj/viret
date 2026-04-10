using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using Viret.Core.Interfaces;
using Viret.Core.Models;
using Viret.Core.Services;
using Viret.Data;

namespace Viret.Tests;

public class LocalPersistenceTests
{
    [Fact]
    public async Task InitializeViretData_WithSeedEnabled_AppliesMigrationsAndSeedsData()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"viret-{Guid.NewGuid():N}.db");

        try
        {
            var services = new ServiceCollection();
            services.AddViretData(dbPath);

            using (var serviceProvider = services.BuildServiceProvider())
            {
                serviceProvider.InitializeViretData(seedSampleData: true);

                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ViretDbContext>();

                    Assert.True(await dbContext.Database.CanConnectAsync());
                    Assert.True(await dbContext.Families.AnyAsync());
                    Assert.True(await dbContext.Transactions.AnyAsync());
                }
            }
        }
        finally
        {
            TryDeleteDatabaseFile(dbPath);
        }
    }

    [Fact]
    public async Task Repositories_PersistDataAcrossScopesWithSQLite()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"viret-{Guid.NewGuid():N}.db");

        try
        {
            var services = new ServiceCollection();
            services.AddViretData(dbPath);

            using (var serviceProvider = services.BuildServiceProvider())
            {
                serviceProvider.InitializeViretData();

                using (var initialScope = serviceProvider.CreateScope())
                {
                    var dbContext = initialScope.ServiceProvider.GetRequiredService<ViretDbContext>();
                    Assert.False(await dbContext.Families.AnyAsync());
                }

                using (var writeScope = serviceProvider.CreateScope())
                {
                    var familyRepository = writeScope.ServiceProvider.GetRequiredService<IFamilyRepository>();
                    var transactionRepository = writeScope.ServiceProvider.GetRequiredService<ITransactionRepository>();

                    var family = new Family { Name = "Família Persistência" };
                    await familyRepository.AddAsync(family);

                    await transactionRepository.AddAsync(new Transaction
                    {
                        Description = "Conta de luz",
                        Amount = 180m,
                        Date = DateTime.Today,
                        Type = TransactionType.Expense,
                        FamilyId = family.Id
                    });
                }

                using (var readScope = serviceProvider.CreateScope())
                {
                    var transactionRepository = readScope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                    var familyRepository = readScope.ServiceProvider.GetRequiredService<IFamilyRepository>();

                    var family = (await familyRepository.GetAllAsync()).Single(f => f.Name == "Família Persistência");
                    var transactions = await transactionRepository.GetByFamilyIdAsync(family.Id);

                    Assert.Single(transactions);
                    Assert.Equal(-180m, await transactionRepository.GetBalanceByFamilyIdAsync(family.Id));
                }
            }
        }
        finally
        {
            TryDeleteDatabaseFile(dbPath);
        }
    }

    [Fact]
    public async Task Repositories_PersistUserFamilyAssociationAcrossScopesWithSQLite()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"viret-{Guid.NewGuid():N}.db");

        try
        {
            var services = new ServiceCollection();
            services.AddViretData(dbPath);

            using (var serviceProvider = services.BuildServiceProvider())
            {
                serviceProvider.InitializeViretData();

                using (var writeScope = serviceProvider.CreateScope())
                {
                    var familyRepository = writeScope.ServiceProvider.GetRequiredService<IFamilyRepository>();
                    var userRepository = writeScope.ServiceProvider.GetRequiredService<IUserRepository>();
                    var familyMemberRepository = writeScope.ServiceProvider.GetRequiredService<IFamilyMemberRepository>();

                    var family = new Family { Name = "Família Associação" };
                    await familyRepository.AddAsync(family);

                    var user = new User
                    {
                        Name = "Ana",
                        Email = $"ana-{Guid.NewGuid():N}@example.com",
                        PasswordHash = "hash"
                    };
                    await userRepository.AddAsync(user);

                    await familyMemberRepository.AddAsync(new FamilyMember
                    {
                        UserId = user.Id,
                        FamilyId = family.Id,
                        Role = FamilyRole.Admin
                    });
                }

                using (var readScope = serviceProvider.CreateScope())
                {
                    var userRepository = readScope.ServiceProvider.GetRequiredService<IUserRepository>();
                    var familyMemberRepository = readScope.ServiceProvider.GetRequiredService<IFamilyMemberRepository>();

                    var user = (await userRepository.GetAllAsync()).Single(u => u.Name == "Ana");
                    var families = await familyMemberRepository.GetFamiliesByUserIdAsync(user.Id);

                    var family = Assert.Single(families);
                    Assert.Equal("Família Associação", family.Name);
                    Assert.True(await familyMemberRepository.ExistsAsync(user.Id, family.Id));
                }
            }
        }
        finally
        {
            TryDeleteDatabaseFile(dbPath);
        }
    }

    [Fact]
    public async Task FinancialPlanning_FlowPersistsAndReturnsFamilyOverview()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"viret-{Guid.NewGuid():N}.db");

        try
        {
            var services = new ServiceCollection();
            services.AddViretData(dbPath);

            using var serviceProvider = services.BuildServiceProvider();
            serviceProvider.InitializeViretData();

            int familyId;
            int userId;
            int categoryId;

            using (var writeScope = serviceProvider.CreateScope())
            {
                var familyRepository = writeScope.ServiceProvider.GetRequiredService<IFamilyRepository>();
                var userRepository = writeScope.ServiceProvider.GetRequiredService<IUserRepository>();
                var familyMemberRepository = writeScope.ServiceProvider.GetRequiredService<IFamilyMemberRepository>();
                var budgetCategoryRepository = writeScope.ServiceProvider.GetRequiredService<IBudgetCategoryRepository>();
                var incomeRepository = writeScope.ServiceProvider.GetRequiredService<IIncomeRepository>();
                var expenseRepository = writeScope.ServiceProvider.GetRequiredService<IExpenseRepository>();

                var family = new Family { Name = "Família Planejamento" };
                await familyRepository.AddAsync(family);
                familyId = family.Id;

                var user = new User
                {
                    Name = "Carlos",
                    Email = $"carlos-{Guid.NewGuid():N}@example.com",
                    PasswordHash = "hash"
                };
                await userRepository.AddAsync(user);
                userId = user.Id;

                await familyMemberRepository.AddAsync(new FamilyMember
                {
                    UserId = userId,
                    FamilyId = familyId,
                    Role = FamilyRole.Admin
                });

                var category = new BudgetCategory
                {
                    Name = "Moradia",
                    PlannedLimit = 900m,
                    FamilyId = familyId,
                    UserId = userId
                };
                await budgetCategoryRepository.AddAsync(category);
                categoryId = category.Id;

                await incomeRepository.AddAsync(new Income
                {
                    Description = "Salário",
                    PlannedAmount = 3000m,
                    ActualAmount = 2900m,
                    Date = DateTime.Today,
                    FamilyId = familyId,
                    UserId = userId,
                    BudgetCategoryId = categoryId
                });

                await expenseRepository.AddAsync(new Expense
                {
                    Description = "Aluguel",
                    PlannedAmount = 850m,
                    ActualAmount = 870m,
                    Date = DateTime.Today,
                    FamilyId = familyId,
                    UserId = userId,
                    BudgetCategoryId = categoryId
                });
            }

            using (var readScope = serviceProvider.CreateScope())
            {
                var service = new FinancialPlanningService(
                    readScope.ServiceProvider.GetRequiredService<IBudgetCategoryRepository>(),
                    readScope.ServiceProvider.GetRequiredService<IIncomeRepository>(),
                    readScope.ServiceProvider.GetRequiredService<IExpenseRepository>(),
                    readScope.ServiceProvider.GetRequiredService<IFamilyRepository>(),
                    readScope.ServiceProvider.GetRequiredService<IUserRepository>(),
                    readScope.ServiceProvider.GetRequiredService<IFamilyMemberRepository>());

                var overview = await service.GetBudgetOverviewAsync(familyId);

                Assert.Equal(3000m, overview.PlannedIncome);
                Assert.Equal(2900m, overview.ActualIncome);
                Assert.Equal(850m, overview.PlannedExpense);
                Assert.Equal(870m, overview.ActualExpense);
                Assert.Equal(2150m, overview.PlannedAvailable);
                Assert.Equal(2030m, overview.ActualAvailable);

                var categorySummary = Assert.Single(overview.CategorySummaries);
                Assert.Equal("Moradia", categorySummary.CategoryName);
                Assert.Equal(50m, categorySummary.PlannedAvailable);
                Assert.Equal(30m, categorySummary.ActualAvailable);
            }
        }
        finally
        {
            TryDeleteDatabaseFile(dbPath);
        }
    }

    private static void TryDeleteDatabaseFile(string dbPath)
    {
        if (!File.Exists(dbPath))
        {
            return;
        }

        SqliteConnection.ClearAllPools();

        try
        {
            File.Delete(dbPath);
        }
        catch (IOException)
        {
            // Cleanup best-effort only; do not fail functional assertions for temp file locks.
        }
    }
}

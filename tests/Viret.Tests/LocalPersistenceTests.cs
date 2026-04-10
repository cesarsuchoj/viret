using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Viret.Core.Interfaces;
using Viret.Core.Models;
using Viret.Data;

namespace Viret.Tests;

public class LocalPersistenceTests
{
    [Fact]
    public async Task InitializeViretData_AppliesMigrationsAndSeedsData()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"viret-{Guid.NewGuid():N}.db");

        try
        {
            var services = new ServiceCollection();
            services.AddViretData(dbPath);

            using var serviceProvider = services.BuildServiceProvider();
            serviceProvider.InitializeViretData();

            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ViretDbContext>();

            Assert.True(await dbContext.Database.CanConnectAsync());
            Assert.True(await dbContext.Families.AnyAsync());
            Assert.True(await dbContext.Transactions.AnyAsync());
        }
        finally
        {
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
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

            using var serviceProvider = services.BuildServiceProvider();
            serviceProvider.InitializeViretData();

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
        finally
        {
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }
    }
}

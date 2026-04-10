using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Viret.Core.Interfaces;
using Viret.Core.Models;
using Viret.Data.Repositories;

namespace Viret.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViretData(this IServiceCollection services, string dbPath)
    {
        services.AddDbContext<ViretDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IFamilyRepository, FamilyRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFamilyMemberRepository, FamilyMemberRepository>();

        return services;
    }

    public static void InitializeViretData(this IServiceProvider serviceProvider, bool seedSampleData = false)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ViretDbContext>();
        dbContext.Database.Migrate();

        if (seedSampleData)
        {
            SeedIfNeeded(dbContext);
        }
    }

    private static void SeedIfNeeded(ViretDbContext dbContext)
    {
        if (dbContext.Families.Any())
        {
            return;
        }

        var family = new Family
        {
            Name = "Família Exemplo"
        };

        dbContext.Families.Add(family);
        dbContext.SaveChanges();

        dbContext.Transactions.AddRange(
            new Transaction
            {
                Description = "Salário",
                Amount = 3500m,
                Date = new DateTime(2025, 1, 5),
                Type = TransactionType.Income,
                FamilyId = family.Id
            },
            new Transaction
            {
                Description = "Mercado",
                Amount = 420m,
                Date = new DateTime(2025, 1, 8),
                Type = TransactionType.Expense,
                FamilyId = family.Id
            });

        dbContext.SaveChanges();
    }
}

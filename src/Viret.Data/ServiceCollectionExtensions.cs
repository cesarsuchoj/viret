using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Viret.Core.Interfaces;
using Viret.Data.Repositories;

namespace Viret.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViretData(this IServiceCollection services, string dbPath)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IFamilyRepository, FamilyRepository>();

        return services;
    }
}

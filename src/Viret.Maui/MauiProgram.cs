using Microsoft.Extensions.DependencyInjection;
using Viret.Core.Interfaces;
using Viret.Core.Services;
using Viret.Data;
using Viret.Maui.ViewModels;

namespace Viret.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMaui();

        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "viret.db");

        builder.Services.AddViretData(dbPath);

        builder.Services.AddScoped<ITransactionService, TransactionService>();
        builder.Services.AddScoped<IFamilyService, FamilyService>();

        builder.Services.AddTransient<MainViewModel>();

        var app = builder.Build();
        app.Services.InitializeViretData();

        return app;
    }
}

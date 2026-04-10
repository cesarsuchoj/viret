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
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<FamilySelectionViewModel>();

        var app = builder.Build();
        app.Services.InitializeViretData();

        return app;
    }
}

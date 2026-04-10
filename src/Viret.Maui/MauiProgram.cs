using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Viret.Core.Interfaces;
using Viret.Core.Services;
using Viret.Data;
using Viret.Maui.ViewModels;
using Viret.Maui.Views;

namespace Viret.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

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
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<FamilySelectionPage>();

        var app = builder.Build();
        app.Services.InitializeViretData();

        return app;
    }
}

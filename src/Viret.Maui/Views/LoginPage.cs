using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Extensions.DependencyInjection;
using Viret.Maui.ViewModels;

namespace Viret.Maui.Views;

public class LoginPage : ContentPage
{
    private readonly IServiceProvider _serviceProvider;

    public LoginPage(LoginViewModel viewModel, RegisterPage registerPage, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        BindingContext = viewModel;
        SetBinding(TitleProperty, new Binding(nameof(LoginViewModel.Title)));

        var emailEntry = new Entry { Placeholder = "E-mail", Keyboard = Keyboard.Email };
        emailEntry.SetBinding(Entry.TextProperty, nameof(LoginViewModel.Email));

        var passwordEntry = new Entry { Placeholder = "Senha", IsPassword = true };
        passwordEntry.SetBinding(Entry.TextProperty, nameof(LoginViewModel.Password));

        var loginButton = new Button { Text = "Entrar" };
        loginButton.Clicked += async (_, _) =>
        {
            await viewModel.LoginCommand.ExecuteAsync(null);

            if (viewModel.AuthenticatedUserId is int userId)
            {
                var appShell = _serviceProvider.GetRequiredService<AppShell>();
                appShell.SetCurrentUser(userId);
                if (Application.Current is null)
                {
                    await DisplayAlert("Erro", "Não foi possível iniciar o menu principal.", "OK");
                    return;
                }

                Application.Current.MainPage = appShell;
                await appShell.GoToAsync("//dashboard");
            }
        };

        var registerButton = new Button { Text = "Criar conta" };
        registerButton.Clicked += async (_, _) => await Navigation.PushAsync(registerPage);

        var loadingIndicator = new ActivityIndicator();
        loadingIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(LoginViewModel.IsBusy));
        loadingIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, nameof(LoginViewModel.IsBusy));

        var errorLabel = new Label { TextColor = Colors.Red };
        errorLabel.SetBinding(Label.TextProperty, nameof(LoginViewModel.ErrorMessage));

        Content = new VerticalStackLayout
        {
            Padding = 24,
            Spacing = 12,
            Children = { emailEntry, passwordEntry, loginButton, registerButton, loadingIndicator, errorLabel }
        };
    }
}

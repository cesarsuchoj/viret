using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Extensions.DependencyInjection;
using Viret.Maui.ViewModels;

namespace Viret.Maui.Views;

public class LoginPage : ContentPage
{
    private readonly IServiceProvider _serviceProvider;

    public LoginPage(LoginViewModel viewModel, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        BindingContext = viewModel;
        SetBinding(TitleProperty, new Binding(nameof(LoginViewModel.Title)));

        var emailEntry = new Entry { Placeholder = "E-mail", Keyboard = Keyboard.Email, ReturnType = ReturnType.Next, TabIndex = 0, IsTabStop = true };
        emailEntry.SetBinding(Entry.TextProperty, nameof(LoginViewModel.Email));

        var passwordEntry = new Entry { Placeholder = "Senha", IsPassword = true, ReturnType = ReturnType.Done, TabIndex = 1, IsTabStop = true };
        passwordEntry.SetBinding(Entry.TextProperty, nameof(LoginViewModel.Password));

        var loginButton = new Button { Text = "Entrar", TabIndex = 2, IsTabStop = true };

        async Task ExecuteLoginAsync()
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
        }

        emailEntry.Completed += (_, _) => passwordEntry.Focus();
        passwordEntry.Completed += async (_, _) => await ExecuteLoginAsync();
        loginButton.Clicked += async (_, _) => await ExecuteLoginAsync();

        var registerButton = new Button { Text = "Criar conta", TabIndex = 3, IsTabStop = true };
        var isNavigatingToRegister = false;
        registerButton.Clicked += async (_, _) =>
        {
            if (isNavigatingToRegister)
            {
                return;
            }

            isNavigatingToRegister = true;
            registerButton.IsEnabled = false;

            try
            {
                var registerPage = _serviceProvider.GetRequiredService<RegisterPage>();
                await Navigation.PushAsync(registerPage);
            }
            finally
            {
                registerButton.IsEnabled = true;
                isNavigatingToRegister = false;
            }
        };

        Loaded += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(viewModel.Email))
            {
                emailEntry.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.Password))
            {
                passwordEntry.Focus();
                return;
            }

            loginButton.Focus();
        };

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

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Viret.Maui.ViewModels;

namespace Viret.Maui.Views;

public class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        BindingContext = viewModel;
        SetBinding(TitleProperty, new Binding(nameof(LoginViewModel.Title)));

        var emailEntry = new Entry { Placeholder = "E-mail", Keyboard = Keyboard.Email };
        emailEntry.SetBinding(Entry.TextProperty, nameof(LoginViewModel.Email));

        var passwordEntry = new Entry { Placeholder = "Senha", IsPassword = true };
        passwordEntry.SetBinding(Entry.TextProperty, nameof(LoginViewModel.Password));

        var loginButton = new Button { Text = "Entrar" };
        loginButton.SetBinding(Button.CommandProperty, nameof(LoginViewModel.LoginCommand));

        var errorLabel = new Label { TextColor = Colors.Red };
        errorLabel.SetBinding(Label.TextProperty, nameof(LoginViewModel.ErrorMessage));

        Content = new VerticalStackLayout
        {
            Padding = 24,
            Spacing = 12,
            Children = { emailEntry, passwordEntry, loginButton, errorLabel }
        };
    }
}

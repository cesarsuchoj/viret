using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Viret.Maui.ViewModels;

namespace Viret.Maui.Views;

public class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        BindingContext = viewModel;
        SetBinding(TitleProperty, new Binding(nameof(RegisterViewModel.Title)));

        var nameEntry = new Entry { Placeholder = "Nome" };
        nameEntry.SetBinding(Entry.TextProperty, nameof(RegisterViewModel.Name));

        var emailEntry = new Entry { Placeholder = "E-mail", Keyboard = Keyboard.Email };
        emailEntry.SetBinding(Entry.TextProperty, nameof(RegisterViewModel.Email));

        var passwordEntry = new Entry { Placeholder = "Senha", IsPassword = true };
        passwordEntry.SetBinding(Entry.TextProperty, nameof(RegisterViewModel.Password));

        var registerButton = new Button { Text = "Criar conta" };
        registerButton.SetBinding(Button.CommandProperty, nameof(RegisterViewModel.RegisterCommand));

        var errorLabel = new Label { TextColor = Colors.Red };
        errorLabel.SetBinding(Label.TextProperty, nameof(RegisterViewModel.ErrorMessage));

        Content = new VerticalStackLayout
        {
            Padding = 24,
            Spacing = 12,
            Children = { nameEntry, emailEntry, passwordEntry, registerButton, errorLabel }
        };
    }
}

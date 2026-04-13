using Microsoft.Maui.Controls;
using Viret.Maui.ViewModels;

namespace Viret.Maui.Views;

public class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        BindingContext = viewModel;
        SetBinding(TitleProperty, new Binding(nameof(RegisterViewModel.Title)));

        var nameEntry = new Entry { Placeholder = "Nome", ReturnType = ReturnType.Next, TabIndex = 0, IsTabStop = true };
        nameEntry.SetBinding(Entry.TextProperty, nameof(RegisterViewModel.Name));

        var emailEntry = new Entry { Placeholder = "E-mail", Keyboard = Keyboard.Email, ReturnType = ReturnType.Next, TabIndex = 1, IsTabStop = true };
        emailEntry.SetBinding(Entry.TextProperty, nameof(RegisterViewModel.Email));

        var passwordEntry = new Entry { Placeholder = "Senha", IsPassword = true, ReturnType = ReturnType.Done, TabIndex = 2, IsTabStop = true };
        passwordEntry.SetBinding(Entry.TextProperty, nameof(RegisterViewModel.Password));

        var registerButton = new Button { Text = "Criar conta", TabIndex = 3, IsTabStop = true };
        registerButton.SetBinding(Button.CommandProperty, nameof(RegisterViewModel.RegisterCommand));
        registerButton.SetBinding(VisualElement.IsEnabledProperty, nameof(RegisterViewModel.IsNotBusy));

        nameEntry.Completed += (_, _) => emailEntry.Focus();
        emailEntry.Completed += (_, _) => passwordEntry.Focus();
        passwordEntry.Completed += async (_, _) => await viewModel.RegisterCommand.ExecuteAsync(null);

        Loaded += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(viewModel.Name))
            {
                nameEntry.Focus();
                return;
            }

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

            registerButton.Focus();
        };

        var loadingFeedback = FeedbackUi.CreateLoadingFeedback();
        var successLabel = FeedbackUi.CreateSuccessLabel(nameof(RegisterViewModel.SuccessMessage));
        var errorLabel = FeedbackUi.CreateErrorLabel(nameof(RegisterViewModel.ErrorMessage));

        var content = new VerticalStackLayout
        {
            Padding = 24,
            Spacing = 12,
            Children = { nameEntry, emailEntry, passwordEntry, registerButton, loadingFeedback, successLabel, errorLabel }
        };

        var scrollView = new ScrollView { Content = content };
        AccessibilityUi.ApplyToView(scrollView);
        Content = scrollView;
    }
}

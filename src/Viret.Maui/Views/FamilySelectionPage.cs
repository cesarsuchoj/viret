using Microsoft.Maui.Controls;
using Viret.Maui.ViewModels;

namespace Viret.Maui.Views;

public class FamilySelectionPage : ContentPage
{
    public FamilySelectionPage(FamilySelectionViewModel viewModel, MainPage mainPage)
    {
        BindingContext = viewModel;
        SetBinding(TitleProperty, new Binding(nameof(FamilySelectionViewModel.Title)));

        var userIdEntry = new Entry { Placeholder = "ID do usuário", Keyboard = Keyboard.Numeric };
        userIdEntry.SetBinding(Entry.TextProperty, nameof(FamilySelectionViewModel.UserIdText));

        var loadButton = new Button { Text = "Carregar famílias" };
        loadButton.SetBinding(Button.CommandProperty, nameof(FamilySelectionViewModel.LoadFamiliesCommand));
        loadButton.SetBinding(VisualElement.IsEnabledProperty, nameof(FamilySelectionViewModel.IsNotBusy));

        var picker = new Picker { Title = "Família" };
        picker.SetBinding(Picker.ItemsSourceProperty, nameof(FamilySelectionViewModel.Families));
        picker.ItemDisplayBinding = new Binding("Name");
        picker.SetBinding(Picker.SelectedItemProperty, nameof(FamilySelectionViewModel.SelectedFamily));

        var noFamilyLabel = new Label { Text = "Nenhuma família encontrada para este usuário." };
        noFamilyLabel.SetBinding(VisualElement.IsVisibleProperty, nameof(FamilySelectionViewModel.ShowCreateFamilyOption));

        var newFamilyNameEntry = new Entry { Placeholder = "Nome da nova família" };
        newFamilyNameEntry.SetBinding(Entry.TextProperty, nameof(FamilySelectionViewModel.NewFamilyName));
        newFamilyNameEntry.SetBinding(VisualElement.IsVisibleProperty, nameof(FamilySelectionViewModel.ShowCreateFamilyOption));

        var createFamilyButton = new Button { Text = "Criar nova família" };
        createFamilyButton.SetBinding(Button.CommandProperty, nameof(FamilySelectionViewModel.CreateFamilyCommand));
        createFamilyButton.SetBinding(VisualElement.IsVisibleProperty, nameof(FamilySelectionViewModel.ShowCreateFamilyOption));
        createFamilyButton.SetBinding(VisualElement.IsEnabledProperty, nameof(FamilySelectionViewModel.IsNotBusy));

        var selectButton = new Button { Text = "Entrar na família" };
        selectButton.SetBinding(VisualElement.IsEnabledProperty, nameof(FamilySelectionViewModel.IsNotBusy));
        selectButton.Clicked += async (_, _) =>
        {
            await viewModel.SelectFamilyCommand.ExecuteAsync(null);

            if (viewModel.HasAccessToSelectedFamily && viewModel.SelectedFamily is not null)
            {
                var selectedFamilyId = viewModel.SelectedFamily.Id;

                if (Shell.Current is AppShell appShell)
                {
                    appShell.SetCurrentFamily(selectedFamilyId);
                    await appShell.GoToAsync("//dashboard");
                    return;
                }

                if (mainPage.BindingContext is MainViewModel mainViewModel)
                    mainViewModel.FamilyId = selectedFamilyId;

                await Navigation.PushAsync(mainPage);
            }
        };

        var accessLabel = new Label();
        accessLabel.SetBinding(Label.TextProperty, nameof(FamilySelectionViewModel.HasAccessToSelectedFamily), stringFormat: "Acesso autorizado: {0}");

        var loadingFeedback = FeedbackUi.CreateLoadingFeedback();
        var successLabel = FeedbackUi.CreateSuccessLabel(nameof(FamilySelectionViewModel.SuccessMessage));
        var errorLabel = FeedbackUi.CreateErrorLabel(nameof(FamilySelectionViewModel.ErrorMessage));

        var content = new VerticalStackLayout
        {
            Padding = 24,
            Spacing = 12,
            Children =
            {
                userIdEntry, loadButton, picker, noFamilyLabel, newFamilyNameEntry, createFamilyButton, selectButton,
                loadingFeedback, successLabel, accessLabel, errorLabel
            }
        };

        var scrollView = new ScrollView { Content = content };
        AccessibilityUi.ApplyToView(scrollView);
        Content = scrollView;
    }
}

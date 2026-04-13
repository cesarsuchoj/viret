using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
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

        var selectButton = new Button { Text = "Entrar na família" };
        selectButton.Clicked += async (_, _) =>
        {
            await viewModel.SelectFamilyCommand.ExecuteAsync(null);

            if (viewModel.HasAccessToSelectedFamily && viewModel.SelectedFamily is not null)
            {
                var selectedFamilyId = viewModel.SelectedFamily.Id;

                if (mainPage.BindingContext is MainViewModel mainViewModel)
                    mainViewModel.FamilyId = selectedFamilyId;

                if (Shell.Current is AppShell appShell)
                {
                    appShell.SetCurrentFamily(selectedFamilyId);
                    await Shell.Current.GoToAsync("//dashboard");
                    return;
                }

                await Navigation.PushAsync(mainPage);
            }
        };

        var accessLabel = new Label();
        accessLabel.SetBinding(Label.TextProperty, nameof(FamilySelectionViewModel.HasAccessToSelectedFamily), stringFormat: "Acesso autorizado: {0}");

        var errorLabel = new Label { TextColor = Colors.Red };
        errorLabel.SetBinding(Label.TextProperty, nameof(FamilySelectionViewModel.ErrorMessage));

        Content = new VerticalStackLayout
        {
            Padding = 24,
            Spacing = 12,
            Children = { userIdEntry, loadButton, picker, noFamilyLabel, newFamilyNameEntry, createFamilyButton, selectButton, accessLabel, errorLabel }
        };
    }
}

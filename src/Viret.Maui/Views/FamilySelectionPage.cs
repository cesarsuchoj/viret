using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Viret.Maui.ViewModels;

namespace Viret.Maui.Views;

public class FamilySelectionPage : ContentPage
{
    public FamilySelectionPage(FamilySelectionViewModel viewModel)
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

        var selectButton = new Button { Text = "Entrar na família" };
        selectButton.SetBinding(Button.CommandProperty, nameof(FamilySelectionViewModel.SelectFamilyCommand));

        var accessLabel = new Label();
        accessLabel.SetBinding(Label.TextProperty, nameof(FamilySelectionViewModel.HasAccessToSelectedFamily), stringFormat: "Acesso autorizado: {0}");

        var errorLabel = new Label { TextColor = Colors.Red };
        errorLabel.SetBinding(Label.TextProperty, nameof(FamilySelectionViewModel.ErrorMessage));

        Content = new VerticalStackLayout
        {
            Padding = 24,
            Spacing = 12,
            Children = { userIdEntry, loadButton, picker, selectButton, accessLabel, errorLabel }
        };
    }
}

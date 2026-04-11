using Microsoft.Maui.Controls;
using Viret.Maui.ViewModels;

namespace Viret.Maui.Views;

public class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        BindingContext = viewModel;
        SetBinding(TitleProperty, new Binding(nameof(MainViewModel.Title)));

        var familyIdEntry = new Entry { Placeholder = "ID da família", Keyboard = Keyboard.Numeric };
        familyIdEntry.SetBinding(Entry.TextProperty, nameof(MainViewModel.FamilyIdText));

        var loadButton = new Button { Text = "Atualizar dados" };
        loadButton.SetBinding(Button.CommandProperty, nameof(MainViewModel.LoadDataCommand));

        var balanceLabel = new Label { FontSize = 18, FontAttributes = FontAttributes.Bold };
        balanceLabel.SetBinding(Label.TextProperty, nameof(MainViewModel.Balance), stringFormat: "Saldo: {0:C}");

        var transactionsView = new CollectionView();
        transactionsView.SetBinding(ItemsView.ItemsSourceProperty, nameof(MainViewModel.Transactions));
        transactionsView.ItemTemplate = new DataTemplate(() =>
        {
            var descriptionLabel = new Label { FontAttributes = FontAttributes.Bold };
            descriptionLabel.SetBinding(Label.TextProperty, "Description");

            var metaLabel = new Label { FontSize = 12 };
            metaLabel.SetBinding(Label.TextProperty, new MultiBinding
            {
                StringFormat = "{0:dd/MM/yyyy} | {1} | {2:C}",
                Bindings =
                {
                    new Binding("Date"),
                    new Binding("Type"),
                    new Binding("Amount")
                }
            });

            return new VerticalStackLayout
            {
                Padding = new Thickness(0, 6),
                Children = { descriptionLabel, metaLabel }
            };
        });

        Content = new VerticalStackLayout
        {
            Padding = 24,
            Spacing = 12,
            Children = { familyIdEntry, loadButton, balanceLabel, transactionsView }
        };
    }
}

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Viret.Maui.ViewModels;

namespace Viret.Maui.Views;

public class ExpenseEntryPage : ContentPage
{
    public ExpenseEntryPage(ExpenseEntryViewModel viewModel)
    {
        BindingContext = viewModel;
        SetBinding(TitleProperty, new Binding(nameof(ExpenseEntryViewModel.Title)));

        var familyIdEntry = new Entry { Placeholder = "ID da família", Keyboard = Keyboard.Numeric };
        familyIdEntry.SetBinding(Entry.TextProperty, nameof(ExpenseEntryViewModel.FamilyIdText));

        var userIdEntry = new Entry { Placeholder = "ID do usuário", Keyboard = Keyboard.Numeric };
        userIdEntry.SetBinding(Entry.TextProperty, nameof(ExpenseEntryViewModel.UserIdText));

        var descriptionEntry = new Entry { Placeholder = "Descrição do gasto" };
        descriptionEntry.SetBinding(Entry.TextProperty, nameof(ExpenseEntryViewModel.Description));

        var plannedAmountEntry = new Entry { Placeholder = "Valor previsto", Keyboard = Keyboard.Numeric };
        plannedAmountEntry.SetBinding(Entry.TextProperty, nameof(ExpenseEntryViewModel.PlannedAmountText));

        var actualAmountEntry = new Entry { Placeholder = "Valor efetivo", Keyboard = Keyboard.Numeric };
        actualAmountEntry.SetBinding(Entry.TextProperty, nameof(ExpenseEntryViewModel.ActualAmountText));

        var datePicker = new DatePicker();
        datePicker.SetBinding(DatePicker.DateProperty, nameof(ExpenseEntryViewModel.Date));

        var loadCategoriesButton = new Button { Text = "Carregar categorias" };
        loadCategoriesButton.SetBinding(Button.CommandProperty, nameof(ExpenseEntryViewModel.LoadCategoriesCommand));

        var categoryPicker = new Picker { Title = "Categoria" };
        categoryPicker.SetBinding(Picker.ItemsSourceProperty, nameof(ExpenseEntryViewModel.Categories));
        categoryPicker.SetBinding(Picker.SelectedItemProperty, nameof(ExpenseEntryViewModel.SelectedCategory));
        categoryPicker.ItemDisplayBinding = new Binding("Name");

        var saveButton = new Button { Text = "Registrar gasto" };
        saveButton.SetBinding(Button.CommandProperty, nameof(ExpenseEntryViewModel.SaveExpenseCommand));

        var successLabel = new Label { TextColor = Colors.Green };
        successLabel.SetBinding(Label.TextProperty, nameof(ExpenseEntryViewModel.SuccessMessage));

        var errorLabel = new Label { TextColor = Colors.Red };
        errorLabel.SetBinding(Label.TextProperty, nameof(ExpenseEntryViewModel.ErrorMessage));

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 24,
                Spacing = 12,
                Children =
                {
                    familyIdEntry, userIdEntry, descriptionEntry, plannedAmountEntry, actualAmountEntry, datePicker,
                    loadCategoriesButton, categoryPicker, saveButton, successLabel, errorLabel
                }
            }
        };
    }
}

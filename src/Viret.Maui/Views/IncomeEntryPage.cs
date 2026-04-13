using Microsoft.Maui.Controls;
using Viret.Maui.ViewModels;

namespace Viret.Maui.Views;

public class IncomeEntryPage : ContentPage
{
    public IncomeEntryPage(IncomeEntryViewModel viewModel)
    {
        BindingContext = viewModel;
        SetBinding(TitleProperty, new Binding(nameof(IncomeEntryViewModel.Title)));

        var familyIdEntry = new Entry { Placeholder = "ID da família", Keyboard = Keyboard.Numeric };
        familyIdEntry.SetBinding(Entry.TextProperty, nameof(IncomeEntryViewModel.FamilyIdText));

        var userIdEntry = new Entry { Placeholder = "ID do usuário", Keyboard = Keyboard.Numeric };
        userIdEntry.SetBinding(Entry.TextProperty, nameof(IncomeEntryViewModel.UserIdText));

        var descriptionEntry = new Entry { Placeholder = "Descrição do ganho" };
        descriptionEntry.SetBinding(Entry.TextProperty, nameof(IncomeEntryViewModel.Description));

        var plannedAmountEntry = new Entry { Placeholder = "Valor previsto", Keyboard = Keyboard.Numeric };
        plannedAmountEntry.SetBinding(Entry.TextProperty, nameof(IncomeEntryViewModel.PlannedAmountText));

        var actualAmountEntry = new Entry { Placeholder = "Valor efetivo", Keyboard = Keyboard.Numeric };
        actualAmountEntry.SetBinding(Entry.TextProperty, nameof(IncomeEntryViewModel.ActualAmountText));

        var datePicker = new DatePicker();
        datePicker.SetBinding(DatePicker.DateProperty, nameof(IncomeEntryViewModel.Date));

        var loadCategoriesButton = new Button { Text = "Carregar categorias" };
        loadCategoriesButton.SetBinding(Button.CommandProperty, nameof(IncomeEntryViewModel.LoadCategoriesCommand));
        loadCategoriesButton.SetBinding(VisualElement.IsEnabledProperty, nameof(IncomeEntryViewModel.IsNotBusy));

        var categoryPicker = new Picker { Title = "Categoria" };
        categoryPicker.SetBinding(Picker.ItemsSourceProperty, nameof(IncomeEntryViewModel.Categories));
        categoryPicker.SetBinding(Picker.SelectedItemProperty, nameof(IncomeEntryViewModel.SelectedCategory));
        categoryPicker.ItemDisplayBinding = new Binding("Name");

        var saveButton = new Button { Text = "Registrar ganho" };
        saveButton.SetBinding(Button.CommandProperty, nameof(IncomeEntryViewModel.SaveIncomeCommand));
        saveButton.SetBinding(VisualElement.IsEnabledProperty, nameof(IncomeEntryViewModel.IsNotBusy));

        var loadingFeedback = FeedbackUi.CreateLoadingFeedback();
        var successLabel = FeedbackUi.CreateSuccessLabel(nameof(IncomeEntryViewModel.SuccessMessage));
        var errorLabel = FeedbackUi.CreateErrorLabel(nameof(IncomeEntryViewModel.ErrorMessage));

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 24,
                Spacing = 12,
                Children =
                {
                    familyIdEntry, userIdEntry, descriptionEntry, plannedAmountEntry, actualAmountEntry, datePicker,
                    loadCategoriesButton, categoryPicker, saveButton, loadingFeedback, successLabel, errorLabel
                }
            }
        };
    }
}

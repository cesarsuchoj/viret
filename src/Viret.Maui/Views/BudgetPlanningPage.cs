using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Viret.Maui.ViewModels;

namespace Viret.Maui.Views;

public class BudgetPlanningPage : ContentPage
{
    public BudgetPlanningPage(BudgetPlanningViewModel viewModel)
    {
        BindingContext = viewModel;
        SetBinding(TitleProperty, new Binding(nameof(BudgetPlanningViewModel.Title)));

        var familyIdEntry = new Entry { Placeholder = "ID da família", Keyboard = Keyboard.Numeric };
        familyIdEntry.SetBinding(Entry.TextProperty, nameof(BudgetPlanningViewModel.FamilyIdText));

        var userIdEntry = new Entry { Placeholder = "ID do usuário", Keyboard = Keyboard.Numeric };
        userIdEntry.SetBinding(Entry.TextProperty, nameof(BudgetPlanningViewModel.UserIdText));

        var categoryNameEntry = new Entry { Placeholder = "Nova categoria" };
        categoryNameEntry.SetBinding(Entry.TextProperty, nameof(BudgetPlanningViewModel.CategoryName));

        var categoryLimitEntry = new Entry { Placeholder = "Limite previsto da categoria", Keyboard = Keyboard.Numeric };
        categoryLimitEntry.SetBinding(Entry.TextProperty, nameof(BudgetPlanningViewModel.CategoryPlannedLimitText));

        var createCategoryButton = new Button { Text = "Criar categoria" };
        createCategoryButton.SetBinding(Button.CommandProperty, nameof(BudgetPlanningViewModel.CreateCategoryCommand));

        var loadButton = new Button { Text = "Atualizar orçamento" };
        loadButton.SetBinding(Button.CommandProperty, nameof(BudgetPlanningViewModel.LoadOverviewCommand));

        var plannedIncomeLabel = new Label();
        plannedIncomeLabel.SetBinding(Label.TextProperty, nameof(BudgetPlanningViewModel.PlannedIncome), stringFormat: "Ganhos previstos: {0:C}");

        var actualIncomeLabel = new Label();
        actualIncomeLabel.SetBinding(Label.TextProperty, nameof(BudgetPlanningViewModel.ActualIncome), stringFormat: "Ganhos efetivos: {0:C}");

        var plannedExpenseLabel = new Label();
        plannedExpenseLabel.SetBinding(Label.TextProperty, nameof(BudgetPlanningViewModel.PlannedExpense), stringFormat: "Gastos previstos: {0:C}");

        var actualExpenseLabel = new Label();
        actualExpenseLabel.SetBinding(Label.TextProperty, nameof(BudgetPlanningViewModel.ActualExpense), stringFormat: "Gastos efetivos: {0:C}");

        var plannedAvailableLabel = new Label();
        plannedAvailableLabel.SetBinding(Label.TextProperty, nameof(BudgetPlanningViewModel.PlannedAvailable), stringFormat: "Disponível previsto: {0:C}");

        var actualAvailableLabel = new Label();
        actualAvailableLabel.SetBinding(Label.TextProperty, nameof(BudgetPlanningViewModel.ActualAvailable), stringFormat: "Disponível efetivo: {0:C}");

        var categoryList = new CollectionView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                var categoryNameLabel = new Label();
                categoryNameLabel.SetBinding(Label.TextProperty, nameof(Viret.Core.Models.BudgetCategorySummary.CategoryName));

                var plannedLimitLabel = new Label();
                plannedLimitLabel.SetBinding(Label.TextProperty, nameof(Viret.Core.Models.BudgetCategorySummary.PlannedLimit), stringFormat: "Limite: {0:C}");

                var plannedExpenseItemLabel = new Label();
                plannedExpenseItemLabel.SetBinding(Label.TextProperty, nameof(Viret.Core.Models.BudgetCategorySummary.PlannedExpense), stringFormat: "Previsto: {0:C}");

                var actualExpenseItemLabel = new Label();
                actualExpenseItemLabel.SetBinding(Label.TextProperty, nameof(Viret.Core.Models.BudgetCategorySummary.ActualExpense), stringFormat: "Efetivo: {0:C}");

                return new HorizontalStackLayout
                {
                    Spacing = 6,
                    Children =
                    {
                        categoryNameLabel,
                        new Label { Text = " | " },
                        plannedLimitLabel,
                        new Label { Text = " | " },
                        plannedExpenseItemLabel,
                        new Label { Text = " | " },
                        actualExpenseItemLabel
                    }
                };
            })
        };
        categoryList.SetBinding(ItemsView.ItemsSourceProperty, nameof(BudgetPlanningViewModel.CategorySummaries));

        var successLabel = new Label { TextColor = Colors.Green };
        successLabel.SetBinding(Label.TextProperty, nameof(BudgetPlanningViewModel.SuccessMessage));

        var errorLabel = new Label { TextColor = Colors.Red };
        errorLabel.SetBinding(Label.TextProperty, nameof(BudgetPlanningViewModel.ErrorMessage));

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 24,
                Spacing = 12,
                Children =
                {
                    familyIdEntry, userIdEntry, categoryNameEntry, categoryLimitEntry, createCategoryButton, loadButton,
                    plannedIncomeLabel, actualIncomeLabel, plannedExpenseLabel, actualExpenseLabel, plannedAvailableLabel, actualAvailableLabel,
                    categoryList, successLabel, errorLabel
                }
            }
        };
    }
}

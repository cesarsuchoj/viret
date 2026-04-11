using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Viret.Maui.ViewModels;

namespace Viret.Maui.Views;

public class BudgetPlanningPage : ContentPage
{
    private const string SeparatorText = " | ";

    public BudgetPlanningPage(BudgetPlanningViewModel viewModel)
    {
        BindingContext = viewModel;
        SetBinding(TitleProperty, new Binding(nameof(BudgetPlanningViewModel.Title)));

        var familyIdEntry = new Entry { Placeholder = "ID da família", Keyboard = Keyboard.Numeric };
        familyIdEntry.SetBinding(Entry.TextProperty, nameof(BudgetPlanningViewModel.FamilyIdText));

        var userIdEntry = new Entry { Placeholder = "ID do usuário", Keyboard = Keyboard.Numeric };
        userIdEntry.SetBinding(Entry.TextProperty, nameof(BudgetPlanningViewModel.UserIdText));

        var reportUserIdEntry = new Entry { Placeholder = "Filtrar por usuário (opcional)", Keyboard = Keyboard.Numeric };
        reportUserIdEntry.SetBinding(Entry.TextProperty, nameof(BudgetPlanningViewModel.ReportUserIdText));
        AutomationProperties.SetName(reportUserIdEntry, "Filtro por usuário");
        AutomationProperties.SetHelpText(reportUserIdEntry, "Informe o ID do usuário para filtrar o relatório");

        var startDateLabel = new Label { Text = "Data inicial" };
        var startDatePicker = new DatePicker();
        startDatePicker.SetBinding(DatePicker.DateProperty, nameof(BudgetPlanningViewModel.StartDate));
        AutomationProperties.SetName(startDatePicker, "Data inicial");
        AutomationProperties.SetHelpText(startDatePicker, "Selecione a data inicial do filtro");

        var endDateLabel = new Label { Text = "Data final" };
        var endDatePicker = new DatePicker();
        endDatePicker.SetBinding(DatePicker.DateProperty, nameof(BudgetPlanningViewModel.EndDate));
        AutomationProperties.SetName(endDatePicker, "Data final");
        AutomationProperties.SetHelpText(endDatePicker, "Selecione a data final do filtro");

        var snapshotCountEntry = new Entry { Placeholder = "Qtd. de snapshots", Keyboard = Keyboard.Numeric };
        snapshotCountEntry.SetBinding(Entry.TextProperty, nameof(BudgetPlanningViewModel.SnapshotCountText));
        AutomationProperties.SetName(snapshotCountEntry, "Quantidade de snapshots");
        AutomationProperties.SetHelpText(snapshotCountEntry, "Informe quantos snapshots históricos devem ser exibidos");

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
                        new Label { Text = SeparatorText },
                        plannedLimitLabel,
                        new Label { Text = SeparatorText },
                        plannedExpenseItemLabel,
                        new Label { Text = SeparatorText },
                        actualExpenseItemLabel
                    }
                };
            })
        };
        categoryList.SetBinding(ItemsView.ItemsSourceProperty, nameof(BudgetPlanningViewModel.CategorySummaries));

        var periodChartTitleLabel = new Label { Text = "Gráfico de ganhos x gastos por período" };
        var periodChartList = new CollectionView();
        periodChartList.SetBinding(ItemsView.ItemsSourceProperty, nameof(BudgetPlanningViewModel.PeriodChartLines));
        periodChartList.ItemTemplate = new DataTemplate(() =>
        {
            var lineLabel = new Label
            {
                FontFamily = "Courier New",
                LineBreakMode = LineBreakMode.WordWrap
            };
            lineLabel.SetBinding(Label.TextProperty, ".");
            return lineLabel;
        });

        var periodList = new CollectionView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                var periodLabel = new Label();
                periodLabel.SetBinding(Label.TextProperty, nameof(Viret.Core.Models.BudgetPeriodSummary.PeriodLabel));

                var availableLabel = new Label();
                availableLabel.SetBinding(Label.TextProperty, nameof(Viret.Core.Models.BudgetPeriodSummary.ActualAvailable), stringFormat: "Saldo: {0:C}");

                return new HorizontalStackLayout
                {
                    Spacing = 8,
                    Children =
                    {
                        periodLabel,
                        new Label { Text = SeparatorText },
                        availableLabel
                    }
                };
            })
        };
        periodList.SetBinding(ItemsView.ItemsSourceProperty, nameof(BudgetPlanningViewModel.PeriodSummaries));

        var snapshotTitleLabel = new Label { Text = "Snapshots históricos" };
        var snapshotList = new CollectionView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                var labelLabel = new Label();
                labelLabel.SetBinding(Label.TextProperty, nameof(Viret.Core.Models.BudgetSnapshot.Label));

                var snapshotBalanceLabel = new Label();
                snapshotBalanceLabel.SetBinding(Label.TextProperty, nameof(Viret.Core.Models.BudgetSnapshot.ActualAvailable), stringFormat: "Saldo efetivo: {0:C}");

                return new HorizontalStackLayout
                {
                    Spacing = 8,
                    Children =
                    {
                        labelLabel,
                        new Label { Text = SeparatorText },
                        snapshotBalanceLabel
                    }
                };
            })
        };
        snapshotList.SetBinding(ItemsView.ItemsSourceProperty, nameof(BudgetPlanningViewModel.Snapshots));

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
                    familyIdEntry, userIdEntry, reportUserIdEntry, startDateLabel, startDatePicker, endDateLabel, endDatePicker, snapshotCountEntry,
                    categoryNameEntry, categoryLimitEntry, createCategoryButton, loadButton,
                    plannedIncomeLabel, actualIncomeLabel, plannedExpenseLabel, actualExpenseLabel, plannedAvailableLabel, actualAvailableLabel,
                    periodChartTitleLabel, periodChartList, periodList, categoryList, snapshotTitleLabel, snapshotList, successLabel, errorLabel
                }
            }
        };
    }
}

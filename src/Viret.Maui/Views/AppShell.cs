using Microsoft.Maui.Controls;
using Viret.Maui.ViewModels;

namespace Viret.Maui.Views;

public class AppShell : Shell
{
    private readonly MainPage _mainPage;
    private readonly IncomeEntryPage _incomeEntryPage;
    private readonly ExpenseEntryPage _expenseEntryPage;
    private readonly FamilySelectionPage _familySelectionPage;
    private readonly BudgetPlanningPage _budgetPlanningPage;

    public AppShell(
        MainPage mainPage,
        IncomeEntryPage incomeEntryPage,
        ExpenseEntryPage expenseEntryPage,
        FamilySelectionPage familySelectionPage,
        BudgetPlanningPage budgetPlanningPage,
        SettingsPage settingsPage)
    {
        _mainPage = mainPage;
        _incomeEntryPage = incomeEntryPage;
        _expenseEntryPage = expenseEntryPage;
        _familySelectionPage = familySelectionPage;
        _budgetPlanningPage = budgetPlanningPage;

        FlyoutBehavior = FlyoutBehavior.Flyout;

        Items.Add(CreateMenuItem("Dashboard", "dashboard", _mainPage));
        Items.Add(CreateMenuItem("Ganhos", "ganhos", _incomeEntryPage));
        Items.Add(CreateMenuItem("Gastos", "gastos", _expenseEntryPage));
        Items.Add(CreateMenuItem("Famílias", "familias", _familySelectionPage));
        Items.Add(CreateMenuItem("Relatórios", "relatorios", _budgetPlanningPage));
        Items.Add(CreateMenuItem("Configurações", "configuracoes", settingsPage));
    }

    public void SetCurrentUser(int userId)
    {
        var userIdText = userId.ToString();

        if (_familySelectionPage.BindingContext is FamilySelectionViewModel familySelectionViewModel)
            familySelectionViewModel.UserIdText = userIdText;

        if (_incomeEntryPage.BindingContext is IncomeEntryViewModel incomeEntryViewModel)
            incomeEntryViewModel.UserIdText = userIdText;

        if (_expenseEntryPage.BindingContext is ExpenseEntryViewModel expenseEntryViewModel)
            expenseEntryViewModel.UserIdText = userIdText;

        if (_budgetPlanningPage.BindingContext is BudgetPlanningViewModel budgetPlanningViewModel)
            budgetPlanningViewModel.UserIdText = userIdText;
    }

    public void SetCurrentFamily(int familyId)
    {
        var familyIdText = familyId.ToString();

        if (_mainPage.BindingContext is MainViewModel mainViewModel)
            mainViewModel.FamilyId = familyId;

        if (_incomeEntryPage.BindingContext is IncomeEntryViewModel incomeEntryViewModel)
            incomeEntryViewModel.FamilyIdText = familyIdText;

        if (_expenseEntryPage.BindingContext is ExpenseEntryViewModel expenseEntryViewModel)
            expenseEntryViewModel.FamilyIdText = familyIdText;

        if (_budgetPlanningPage.BindingContext is BudgetPlanningViewModel budgetPlanningViewModel)
            budgetPlanningViewModel.FamilyIdText = familyIdText;
    }

    private static FlyoutItem CreateMenuItem(string title, string route, Page page)
        => new()
        {
            Title = title,
            Route = route,
            Items =
            {
                new ShellContent
                {
                    Title = title,
                    Route = route,
                    Content = page
                }
            }
        };
}

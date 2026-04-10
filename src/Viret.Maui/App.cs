using Microsoft.Maui.Controls;
using Viret.Maui.Views;

namespace Viret.Maui;

public class App : Application
{
    public App(
        LoginPage loginPage,
        RegisterPage registerPage,
        FamilySelectionPage familySelectionPage,
        IncomeEntryPage incomeEntryPage,
        ExpenseEntryPage expenseEntryPage,
        BudgetPlanningPage budgetPlanningPage)
    {
        MainPage = new NavigationPage(new TabbedPage
        {
            Children =
            {
                loginPage,
                registerPage,
                familySelectionPage,
                incomeEntryPage,
                expenseEntryPage,
                budgetPlanningPage
            }
        });
    }
}

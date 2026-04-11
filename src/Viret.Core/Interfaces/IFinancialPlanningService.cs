using Viret.Core.Models;

namespace Viret.Core.Interfaces;

public interface IFinancialPlanningService
{
    Task<BudgetCategory> AddBudgetCategoryAsync(BudgetCategory category);
    Task<IEnumerable<BudgetCategory>> GetBudgetCategoriesByFamilyAsync(int familyId);
    Task<Income> AddIncomeAsync(Income income);
    Task<Expense> AddExpenseAsync(Expense expense);
    Task<BudgetOverview> GetBudgetOverviewAsync(int userId, int familyId);
}

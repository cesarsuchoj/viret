using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Core.Services;

public class FinancialPlanningService : IFinancialPlanningService
{
    private readonly IBudgetCategoryRepository _budgetCategoryRepository;
    private readonly IIncomeRepository _incomeRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IFamilyRepository _familyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFamilyMemberRepository _familyMemberRepository;

    public FinancialPlanningService(
        IBudgetCategoryRepository budgetCategoryRepository,
        IIncomeRepository incomeRepository,
        IExpenseRepository expenseRepository,
        IFamilyRepository familyRepository,
        IUserRepository userRepository,
        IFamilyMemberRepository familyMemberRepository)
    {
        _budgetCategoryRepository = budgetCategoryRepository;
        _incomeRepository = incomeRepository;
        _expenseRepository = expenseRepository;
        _familyRepository = familyRepository;
        _userRepository = userRepository;
        _familyMemberRepository = familyMemberRepository;
    }

    public async Task<BudgetCategory> AddBudgetCategoryAsync(BudgetCategory category)
    {
        ValidateCommonFields(category.FamilyId, category.UserId, category.Name, category.PlannedLimit, nameof(category));
        await EnsureUserHasFamilyAccessAsync(category.UserId, category.FamilyId);

        await _budgetCategoryRepository.AddAsync(category);
        return category;
    }

    public Task<IEnumerable<BudgetCategory>> GetBudgetCategoriesByFamilyAsync(int familyId)
        => _budgetCategoryRepository.GetByFamilyIdAsync(familyId);

    public async Task<Income> AddIncomeAsync(Income income)
    {
        ValidateCommonFields(income.FamilyId, income.UserId, income.Description, income.PlannedAmount, nameof(income));
        ValidateActualAmount(income.ActualAmount, nameof(income));
        ValidateCategory(income.BudgetCategoryId, nameof(income));
        await EnsureUserHasFamilyAccessAsync(income.UserId, income.FamilyId);
        await EnsureCategoryBelongsToFamilyAsync(income.BudgetCategoryId, income.FamilyId);

        await _incomeRepository.AddAsync(income);
        return income;
    }

    public async Task<Expense> AddExpenseAsync(Expense expense)
    {
        ValidateCommonFields(expense.FamilyId, expense.UserId, expense.Description, expense.PlannedAmount, nameof(expense));
        ValidateActualAmount(expense.ActualAmount, nameof(expense));
        ValidateCategory(expense.BudgetCategoryId, nameof(expense));
        await EnsureUserHasFamilyAccessAsync(expense.UserId, expense.FamilyId);
        await EnsureCategoryBelongsToFamilyAsync(expense.BudgetCategoryId, expense.FamilyId);

        await _expenseRepository.AddAsync(expense);
        return expense;
    }

    public async Task<BudgetOverview> GetBudgetOverviewAsync(int familyId)
    {
        var incomes = await _incomeRepository.GetByFamilyIdAsync(familyId);
        var expenses = await _expenseRepository.GetByFamilyIdAsync(familyId);
        var categories = await _budgetCategoryRepository.GetByFamilyIdAsync(familyId);

        var plannedIncome = incomes.Sum(i => i.PlannedAmount);
        var actualIncome = incomes.Sum(i => i.ActualAmount);
        var plannedExpense = expenses.Sum(e => e.PlannedAmount);
        var actualExpense = expenses.Sum(e => e.ActualAmount);

        var categorySummaries = categories
            .Select(category =>
            {
                var categoryExpenses = expenses.Where(e => e.BudgetCategoryId == category.Id);
                var plannedCategoryExpense = categoryExpenses.Sum(e => e.PlannedAmount);
                var actualCategoryExpense = categoryExpenses.Sum(e => e.ActualAmount);

                return new BudgetCategorySummary
                {
                    BudgetCategoryId = category.Id,
                    CategoryName = category.Name,
                    PlannedLimit = category.PlannedLimit,
                    PlannedExpense = plannedCategoryExpense,
                    ActualExpense = actualCategoryExpense,
                    PlannedAvailable = category.PlannedLimit - plannedCategoryExpense,
                    ActualAvailable = category.PlannedLimit - actualCategoryExpense
                };
            })
            .ToArray();

        return new BudgetOverview
        {
            PlannedIncome = plannedIncome,
            ActualIncome = actualIncome,
            PlannedExpense = plannedExpense,
            ActualExpense = actualExpense,
            PlannedAvailable = plannedIncome - plannedExpense,
            ActualAvailable = actualIncome - actualExpense,
            CategorySummaries = categorySummaries
        };
    }

    private async Task EnsureUserHasFamilyAccessAsync(int userId, int familyId)
    {
        if (await _userRepository.GetByIdAsync(userId) is null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (await _familyRepository.GetByIdAsync(familyId) is null)
        {
            throw new KeyNotFoundException("Family not found.");
        }

        if (!await _familyMemberRepository.ExistsAsync(userId, familyId))
        {
            throw new UnauthorizedAccessException("User has no access to this family.");
        }
    }

    private async Task EnsureCategoryBelongsToFamilyAsync(int? categoryId, int familyId)
    {
        if (!categoryId.HasValue)
        {
            return;
        }

        var category = await _budgetCategoryRepository.GetByIdAsync(categoryId.Value);
        if (category is null || category.FamilyId != familyId)
        {
            throw new InvalidOperationException("Budget category does not belong to the informed family.");
        }
    }

    private static void ValidateCommonFields(int familyId, int userId, string description, decimal plannedAmount, string parameterName)
    {
        if (familyId <= 0)
        {
            throw new ArgumentException("Family id must be greater than zero.", parameterName);
        }

        if (userId <= 0)
        {
            throw new ArgumentException("User id must be greater than zero.", parameterName);
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be empty.", parameterName);
        }

        if (plannedAmount <= 0)
        {
            throw new ArgumentException("Planned amount must be greater than zero.", parameterName);
        }
    }

    private static void ValidateActualAmount(decimal actualAmount, string parameterName)
    {
        if (actualAmount < 0)
        {
            throw new ArgumentException("Actual amount cannot be negative.", parameterName);
        }
    }

    private static void ValidateCategory(int? categoryId, string parameterName)
    {
        if (!categoryId.HasValue || categoryId.Value <= 0)
        {
            throw new ArgumentException("Budget category is required.", parameterName);
        }
    }
}

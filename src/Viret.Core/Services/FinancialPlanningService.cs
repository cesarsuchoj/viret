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

    public async Task<BudgetOverview> GetBudgetOverviewAsync(
        int userId,
        int familyId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? filteredUserId = null,
        int snapshotCount = 6)
    {
        if (startDate.HasValue && endDate.HasValue && startDate.Value.Date > endDate.Value.Date)
        {
            throw new ArgumentException("Start date cannot be greater than end date.", nameof(startDate));
        }

        if (filteredUserId.HasValue && filteredUserId.Value <= 0)
        {
            throw new ArgumentException("Filtered user id must be greater than zero.", nameof(filteredUserId));
        }

        if (snapshotCount <= 0)
        {
            throw new ArgumentException("Snapshot count must be greater than zero.", nameof(snapshotCount));
        }

        await EnsureUserHasFamilyAccessAsync(userId, familyId);

        if (filteredUserId.HasValue)
        {
            await EnsureUserHasFamilyAccessAsync(filteredUserId.Value, familyId);
        }

        var incomes = await _incomeRepository.GetByFamilyIdAsync(familyId);
        var expenses = await _expenseRepository.GetByFamilyIdAsync(familyId);
        var categories = await _budgetCategoryRepository.GetByFamilyIdAsync(familyId);

        var filteredIncomes = ApplyFilters(incomes, startDate, endDate, filteredUserId).ToArray();
        var filteredExpenses = ApplyFilters(expenses, startDate, endDate, filteredUserId).ToArray();

        var plannedIncome = filteredIncomes.Sum(i => i.PlannedAmount);
        var actualIncome = filteredIncomes.Sum(i => i.ActualAmount);
        var plannedExpense = filteredExpenses.Sum(e => e.PlannedAmount);
        var actualExpense = filteredExpenses.Sum(e => e.ActualAmount);

        var expensesByCategoryId = filteredExpenses
            .GroupBy(e => e.BudgetCategoryId)
            .ToDictionary(group => group.Key, group => group.ToArray());

        var categorySummaries = categories
            .Select(category =>
            {
                var categoryExpenses = expensesByCategoryId.TryGetValue(category.Id, out var groupedExpenses)
                    ? groupedExpenses
                    : Array.Empty<Expense>();
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

        var periodSummaries = BuildPeriodSummaries(filteredIncomes, filteredExpenses, startDate, endDate);
        var snapshots = periodSummaries
            .OrderByDescending(period => period.PeriodStart)
            .Take(snapshotCount)
            .Select(period => new BudgetSnapshot
            {
                CapturedAt = period.PeriodStart,
                Label = period.PeriodLabel,
                PlannedIncome = period.PlannedIncome,
                ActualIncome = period.ActualIncome,
                PlannedExpense = period.PlannedExpense,
                ActualExpense = period.ActualExpense,
                PlannedAvailable = period.PlannedAvailable,
                ActualAvailable = period.ActualAvailable
            })
            .ToArray();

        return new BudgetOverview
        {
            StartDate = startDate?.Date,
            EndDate = endDate?.Date,
            FilteredUserId = filteredUserId,
            PlannedIncome = plannedIncome,
            ActualIncome = actualIncome,
            PlannedExpense = plannedExpense,
            ActualExpense = actualExpense,
            PlannedAvailable = plannedIncome - plannedExpense,
            ActualAvailable = actualIncome - actualExpense,
            CategorySummaries = categorySummaries,
            PeriodSummaries = periodSummaries,
            Snapshots = snapshots
        };
    }

    private static IEnumerable<TTransaction> ApplyFilters<TTransaction>(
        IEnumerable<TTransaction> source,
        DateTime? startDate,
        DateTime? endDate,
        int? filteredUserId)
        where TTransaction : class
    {
        IEnumerable<TTransaction> query = source;

        if (filteredUserId.HasValue)
        {
            query = query.Where(item => GetUserId(item) == filteredUserId.Value);
        }

        if (startDate.HasValue)
        {
            var normalizedStart = startDate.Value.Date;
            query = query.Where(item => GetDate(item).Date >= normalizedStart);
        }

        if (endDate.HasValue)
        {
            var normalizedEnd = endDate.Value.Date;
            query = query.Where(item => GetDate(item).Date <= normalizedEnd);
        }

        return query;
    }

    private static IReadOnlyCollection<BudgetPeriodSummary> BuildPeriodSummaries(
        IReadOnlyCollection<Income> incomes,
        IReadOnlyCollection<Expense> expenses,
        DateTime? startDate,
        DateTime? endDate)
    {
        var incomesWithValidDates = incomes
            .Where(income => income.Date != DateTime.MinValue)
            .ToArray();

        var expensesWithValidDates = expenses
            .Where(expense => expense.Date != DateTime.MinValue)
            .ToArray();

        if (!startDate.HasValue &&
            !endDate.HasValue &&
            incomesWithValidDates.Length == 0 &&
            expensesWithValidDates.Length == 0)
        {
            return Array.Empty<BudgetPeriodSummary>();
        }

        var minDate = new[]
        {
            incomesWithValidDates.Any() ? incomesWithValidDates.Min(i => i.Date).Date : (DateTime?)null,
            expensesWithValidDates.Any() ? expensesWithValidDates.Min(e => e.Date).Date : (DateTime?)null,
            startDate?.Date
        }.Where(date => date.HasValue).Select(date => date!.Value).DefaultIfEmpty(DateTime.Today).Min();

        var maxDate = new[]
        {
            incomesWithValidDates.Any() ? incomesWithValidDates.Max(i => i.Date).Date : (DateTime?)null,
            expensesWithValidDates.Any() ? expensesWithValidDates.Max(e => e.Date).Date : (DateTime?)null,
            endDate?.Date
        }.Where(date => date.HasValue).Select(date => date!.Value).DefaultIfEmpty(DateTime.Today).Max();

        var periodStart = new DateTime(minDate.Year, minDate.Month, 1);
        var periodEnd = new DateTime(maxDate.Year, maxDate.Month, 1);

        var incomeByPeriod = incomesWithValidDates
            .GroupBy(income => new DateTime(income.Date.Year, income.Date.Month, 1))
            .ToDictionary(group => group.Key, group => group.ToArray());

        var expenseByPeriod = expensesWithValidDates
            .GroupBy(expense => new DateTime(expense.Date.Year, expense.Date.Month, 1))
            .ToDictionary(group => group.Key, group => group.ToArray());

        var periods = new List<BudgetPeriodSummary>();
        for (var cursor = periodStart; cursor <= periodEnd; cursor = cursor.AddMonths(1))
        {
            var periodIncomes = incomeByPeriod.TryGetValue(cursor, out var groupedIncomes) ? groupedIncomes : Array.Empty<Income>();
            var periodExpenses = expenseByPeriod.TryGetValue(cursor, out var groupedExpenses) ? groupedExpenses : Array.Empty<Expense>();

            var periodPlannedIncome = periodIncomes.Sum(income => income.PlannedAmount);
            var periodActualIncome = periodIncomes.Sum(income => income.ActualAmount);
            var periodPlannedExpense = periodExpenses.Sum(expense => expense.PlannedAmount);
            var periodActualExpense = periodExpenses.Sum(expense => expense.ActualAmount);

            periods.Add(new BudgetPeriodSummary
            {
                PeriodStart = cursor,
                PeriodLabel = cursor.ToString("yyyy-MM"),
                PlannedIncome = periodPlannedIncome,
                ActualIncome = periodActualIncome,
                PlannedExpense = periodPlannedExpense,
                ActualExpense = periodActualExpense,
                PlannedAvailable = periodPlannedIncome - periodPlannedExpense,
                ActualAvailable = periodActualIncome - periodActualExpense
            });
        }

        return periods;
    }

    private static int GetUserId<TTransaction>(TTransaction transaction)
        where TTransaction : class
        => transaction switch
        {
            Income income => income.UserId,
            Expense expense => expense.UserId,
            _ => throw new InvalidOperationException("Unsupported transaction type.")
        };

    private static DateTime GetDate<TTransaction>(TTransaction transaction)
        where TTransaction : class
        => transaction switch
        {
            Income income => income.Date,
            Expense expense => expense.Date,
            _ => throw new InvalidOperationException("Unsupported transaction type.")
        };

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

    private async Task EnsureCategoryBelongsToFamilyAsync(int categoryId, int familyId)
    {
        var category = await _budgetCategoryRepository.GetByIdAsync(categoryId);
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

    private static void ValidateCategory(int categoryId, string parameterName)
    {
        if (categoryId <= 0)
        {
            throw new ArgumentException("Budget category is required.", parameterName);
        }
    }
}

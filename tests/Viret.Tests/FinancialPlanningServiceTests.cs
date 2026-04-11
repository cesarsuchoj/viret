using Moq;
using Viret.Core.Interfaces;
using Viret.Core.Models;
using Viret.Core.Services;

namespace Viret.Tests;

public class FinancialPlanningServiceTests
{
    private readonly Mock<IBudgetCategoryRepository> _budgetCategoryRepositoryMock;
    private readonly Mock<IIncomeRepository> _incomeRepositoryMock;
    private readonly Mock<IExpenseRepository> _expenseRepositoryMock;
    private readonly Mock<IFamilyRepository> _familyRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IFamilyMemberRepository> _familyMemberRepositoryMock;
    private readonly FinancialPlanningService _sut;

    public FinancialPlanningServiceTests()
    {
        _budgetCategoryRepositoryMock = new Mock<IBudgetCategoryRepository>();
        _incomeRepositoryMock = new Mock<IIncomeRepository>();
        _expenseRepositoryMock = new Mock<IExpenseRepository>();
        _familyRepositoryMock = new Mock<IFamilyRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _familyMemberRepositoryMock = new Mock<IFamilyMemberRepository>();

        _sut = new FinancialPlanningService(
            _budgetCategoryRepositoryMock.Object,
            _incomeRepositoryMock.Object,
            _expenseRepositoryMock.Object,
            _familyRepositoryMock.Object,
            _userRepositoryMock.Object,
            _familyMemberRepositoryMock.Object);
    }

    [Fact]
    public async Task AddIncomeAsync_UserWithFamilyAccess_AddsIncome()
    {
        var income = new Income
        {
            Description = "Freelance",
            PlannedAmount = 1500m,
            ActualAmount = 1450m,
            Date = DateTime.Today,
            FamilyId = 2,
            UserId = 3,
            BudgetCategoryId = 9
        };

        SetupFamilyAccess(userId: 3, familyId: 2, hasAccess: true);
        _budgetCategoryRepositoryMock.Setup(r => r.GetByIdAsync(9))
            .ReturnsAsync(new BudgetCategory { Id = 9, FamilyId = 2 });

        await _sut.AddIncomeAsync(income);

        _incomeRepositoryMock.Verify(r => r.AddAsync(income), Times.Once);
    }

    [Fact]
    public async Task AddExpenseAsync_WithoutCategory_ThrowsArgumentException()
    {
        var expense = new Expense
        {
            Description = "Mercado",
            PlannedAmount = 400m,
            ActualAmount = 390m,
            Date = DateTime.Today,
            FamilyId = 2,
            UserId = 3
        };

        SetupFamilyAccess(userId: 3, familyId: 2, hasAccess: true);

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddExpenseAsync(expense));
    }

    [Fact]
    public async Task AddExpenseAsync_WithCategoryFromOtherFamily_ThrowsInvalidOperationException()
    {
        var expense = new Expense
        {
            Description = "Mercado",
            PlannedAmount = 400m,
            ActualAmount = 390m,
            Date = DateTime.Today,
            FamilyId = 2,
            UserId = 3,
            BudgetCategoryId = 10
        };

        SetupFamilyAccess(userId: 3, familyId: 2, hasAccess: true);
        _budgetCategoryRepositoryMock.Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync(new BudgetCategory { Id = 10, FamilyId = 999 });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddExpenseAsync(expense));
    }

    [Fact]
    public async Task GetBudgetOverviewAsync_ReturnsConsolidatedFamilyTotals()
    {
        SetupFamilyAccess(userId: 3, familyId: 1, hasAccess: true);

        _incomeRepositoryMock.Setup(r => r.GetByFamilyIdAsync(1))
            .ReturnsAsync(new[]
            {
                new Income { FamilyId = 1, PlannedAmount = 1000m, ActualAmount = 900m },
                new Income { FamilyId = 1, PlannedAmount = 500m, ActualAmount = 550m }
            });

        _expenseRepositoryMock.Setup(r => r.GetByFamilyIdAsync(1))
            .ReturnsAsync(new[]
            {
                new Expense { FamilyId = 1, PlannedAmount = 300m, ActualAmount = 320m, BudgetCategoryId = 7 },
                new Expense { FamilyId = 1, PlannedAmount = 150m, ActualAmount = 100m, BudgetCategoryId = 7 }
            });

        _budgetCategoryRepositoryMock.Setup(r => r.GetByFamilyIdAsync(1))
            .ReturnsAsync(new[]
            {
                new BudgetCategory { Id = 7, FamilyId = 1, Name = "Alimentação", PlannedLimit = 600m }
            });

        var overview = await _sut.GetBudgetOverviewAsync(3, 1);

        Assert.Equal(1500m, overview.PlannedIncome);
        Assert.Equal(1450m, overview.ActualIncome);
        Assert.Equal(450m, overview.PlannedExpense);
        Assert.Equal(420m, overview.ActualExpense);
        Assert.Equal(1050m, overview.PlannedAvailable);
        Assert.Equal(1030m, overview.ActualAvailable);

        var category = Assert.Single(overview.CategorySummaries);
        Assert.Equal("Alimentação", category.CategoryName);
        Assert.Equal(450m, category.PlannedExpense);
        Assert.Equal(420m, category.ActualExpense);
        Assert.Equal(150m, category.PlannedAvailable);
        Assert.Equal(180m, category.ActualAvailable);
    }

    [Fact]
    public async Task GetBudgetOverviewAsync_WithoutFamilyAccess_ThrowsUnauthorizedAccessException()
    {
        SetupFamilyAccess(userId: 3, familyId: 1, hasAccess: false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.GetBudgetOverviewAsync(3, 1));
    }

    [Fact]
    public async Task GetBudgetOverviewAsync_WithPeriodAndUserFilters_ReturnsFilteredDashboardsData()
    {
        SetupFamilyAccess(userId: 3, familyId: 1, hasAccess: true);
        SetupFamilyAccess(userId: 4, familyId: 1, hasAccess: true);

        _incomeRepositoryMock.Setup(r => r.GetByFamilyIdAsync(1))
            .ReturnsAsync(new[]
            {
                new Income { FamilyId = 1, UserId = 4, PlannedAmount = 800m, ActualAmount = 820m, Date = new DateTime(2026, 1, 10) },
                new Income { FamilyId = 1, UserId = 4, PlannedAmount = 900m, ActualAmount = 910m, Date = new DateTime(2026, 2, 10) },
                new Income { FamilyId = 1, UserId = 3, PlannedAmount = 2000m, ActualAmount = 2000m, Date = new DateTime(2026, 2, 15) }
            });

        _expenseRepositoryMock.Setup(r => r.GetByFamilyIdAsync(1))
            .ReturnsAsync(new[]
            {
                new Expense { FamilyId = 1, UserId = 4, PlannedAmount = 200m, ActualAmount = 210m, Date = new DateTime(2026, 1, 11), BudgetCategoryId = 7 },
                new Expense { FamilyId = 1, UserId = 4, PlannedAmount = 300m, ActualAmount = 280m, Date = new DateTime(2026, 2, 11), BudgetCategoryId = 7 },
                new Expense { FamilyId = 1, UserId = 3, PlannedAmount = 999m, ActualAmount = 999m, Date = new DateTime(2026, 2, 16), BudgetCategoryId = 7 }
            });

        _budgetCategoryRepositoryMock.Setup(r => r.GetByFamilyIdAsync(1))
            .ReturnsAsync(new[]
            {
                new BudgetCategory { Id = 7, FamilyId = 1, Name = "Casa", PlannedLimit = 1000m }
            });

        var overview = await _sut.GetBudgetOverviewAsync(
            userId: 3,
            familyId: 1,
            startDate: new DateTime(2026, 1, 1),
            endDate: new DateTime(2026, 2, 28),
            filteredUserId: 4,
            snapshotCount: 1);

        Assert.Equal(1700m, overview.PlannedIncome);
        Assert.Equal(1730m, overview.ActualIncome);
        Assert.Equal(500m, overview.PlannedExpense);
        Assert.Equal(490m, overview.ActualExpense);
        Assert.Equal(1200m, overview.PlannedAvailable);
        Assert.Equal(1240m, overview.ActualAvailable);

        Assert.Equal(2, overview.PeriodSummaries.Count);
        Assert.Single(overview.Snapshots);
        Assert.Equal("2026-02", overview.Snapshots.Single().Label);
    }

    [Fact]
    public async Task GetBudgetOverviewAsync_WithInvalidDateRange_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.GetBudgetOverviewAsync(3, 1, new DateTime(2026, 2, 1), new DateTime(2026, 1, 1)));
    }

    private void SetupFamilyAccess(int userId, int familyId, bool hasAccess)
    {
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId, Email = "ana@example.com", PasswordHash = "hash", Name = "Ana" });
        _familyRepositoryMock.Setup(r => r.GetByIdAsync(familyId)).ReturnsAsync(new Family { Id = familyId, Name = "Silva" });
        _familyMemberRepositoryMock.Setup(r => r.ExistsAsync(userId, familyId)).ReturnsAsync(hasAccess);
    }
}

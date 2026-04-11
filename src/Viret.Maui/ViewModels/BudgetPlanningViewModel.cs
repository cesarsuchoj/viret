using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;
using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Maui.ViewModels;

public partial class BudgetPlanningViewModel : BaseViewModel
{
    private readonly IFinancialPlanningService _financialPlanningService;

    [ObservableProperty]
    private string _familyIdText = string.Empty;

    [ObservableProperty]
    private string _userIdText = string.Empty;

    [ObservableProperty]
    private string _reportUserIdText = string.Empty;

    [ObservableProperty]
    private string _categoryName = string.Empty;

    [ObservableProperty]
    private string _categoryPlannedLimitText = string.Empty;

    [ObservableProperty]
    private decimal _plannedIncome;

    [ObservableProperty]
    private decimal _actualIncome;

    [ObservableProperty]
    private decimal _plannedExpense;

    [ObservableProperty]
    private decimal _actualExpense;

    [ObservableProperty]
    private decimal _plannedAvailable;

    [ObservableProperty]
    private decimal _actualAvailable;

    [ObservableProperty]
    private IEnumerable<BudgetCategorySummary> _categorySummaries = Enumerable.Empty<BudgetCategorySummary>();

    [ObservableProperty]
    private IEnumerable<BudgetPeriodSummary> _periodSummaries = Enumerable.Empty<BudgetPeriodSummary>();

    [ObservableProperty]
    private IEnumerable<string> _periodChartLines = Enumerable.Empty<string>();

    [ObservableProperty]
    private IEnumerable<BudgetSnapshot> _snapshots = Enumerable.Empty<BudgetSnapshot>();

    [ObservableProperty]
    private DateTime _startDate = DateTime.Today.AddMonths(-5);

    [ObservableProperty]
    private DateTime _endDate = DateTime.Today;

    [ObservableProperty]
    private string _snapshotCountText = "6";

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    public BudgetPlanningViewModel(IFinancialPlanningService financialPlanningService)
    {
        _financialPlanningService = financialPlanningService;
        Title = "Dashboard Financeiro";
    }

    [RelayCommand]
    private async Task CreateCategoryAsync()
    {
        if (IsBusy)
            return;

        if (!TryParseFamilyId(out var familyId) || !TryParseUserId(out var userId))
        {
            ErrorMessage = "Informe IDs válidos de família e usuário.";
            return;
        }

        if (!TryParseAmount(CategoryPlannedLimitText, out var plannedLimit))
        {
            ErrorMessage = "Informe um limite previsto válido para a categoria.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            await _financialPlanningService.AddBudgetCategoryAsync(new BudgetCategory
            {
                Name = CategoryName.Trim(),
                PlannedLimit = plannedLimit,
                FamilyId = familyId,
                UserId = userId
            });

            SuccessMessage = "Categoria de orçamento criada com sucesso.";
            await LoadOverviewAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadOverviewAsync()
    {
        if (IsBusy)
            return;

        if (!TryParseFamilyId(out var familyId))
        {
            ErrorMessage = "Informe um ID de família válido.";
            return;
        }

        if (!TryParseUserId(out var userId))
        {
            ErrorMessage = "Informe um ID de usuário válido.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            if (!TryParseSnapshotCount(out var snapshotCount))
            {
                ErrorMessage = "Informe uma quantidade válida de snapshots.";
                return;
            }

            if (StartDate.Date > EndDate.Date)
            {
                ErrorMessage = "A data inicial não pode ser maior que a data final.";
                return;
            }

            int? filteredUserId = null;
            if (!string.IsNullOrWhiteSpace(ReportUserIdText))
            {
                if (!TryParseReportUserId(out var parsedReportUserId))
                {
                    ErrorMessage = "Informe um filtro de usuário válido.";
                    return;
                }

                filteredUserId = parsedReportUserId;
            }

            var overview = await _financialPlanningService.GetBudgetOverviewAsync(
                userId,
                familyId,
                StartDate.Date,
                EndDate.Date,
                filteredUserId,
                snapshotCount);
            PlannedIncome = overview.PlannedIncome;
            ActualIncome = overview.ActualIncome;
            PlannedExpense = overview.PlannedExpense;
            ActualExpense = overview.ActualExpense;
            PlannedAvailable = overview.PlannedAvailable;
            ActualAvailable = overview.ActualAvailable;
            CategorySummaries = overview.CategorySummaries;
            PeriodSummaries = overview.PeriodSummaries;
            Snapshots = overview.Snapshots;
            PeriodChartLines = BuildChartLines(overview.PeriodSummaries);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool TryParseFamilyId(out int familyId) => int.TryParse(FamilyIdText, out familyId) && familyId > 0;
    private bool TryParseUserId(out int userId) => int.TryParse(UserIdText, out userId) && userId > 0;
    private bool TryParseReportUserId(out int userId) => int.TryParse(ReportUserIdText, out userId) && userId > 0;
    private bool TryParseSnapshotCount(out int snapshotCount) => int.TryParse(SnapshotCountText, out snapshotCount) && snapshotCount > 0;

    private static bool TryParseAmount(string text, out decimal value)
        => decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out value) ||
           decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value);

    private static IReadOnlyCollection<string> BuildChartLines(IEnumerable<BudgetPeriodSummary> summaries)
    {
        var periodItems = summaries.ToArray();
        if (periodItems.Length == 0)
        {
            return Array.Empty<string>();
        }

        var maxValue = periodItems
            .SelectMany(summary => new[] { summary.ActualIncome, summary.ActualExpense })
            .DefaultIfEmpty(0m)
            .Max();

        if (maxValue <= 0)
        {
            return periodItems.Select(summary => $"{summary.PeriodLabel} | ganhos: 0 | gastos: 0").ToArray();
        }

        return periodItems.Select(summary =>
        {
            var incomeBar = BuildBar(summary.ActualIncome, maxValue);
            var expenseBar = BuildBar(summary.ActualExpense, maxValue);
            return $"{summary.PeriodLabel} | G {incomeBar} {summary.ActualIncome:C} | E {expenseBar} {summary.ActualExpense:C}";
        }).ToArray();
    }

    private static string BuildBar(decimal value, decimal maxValue)
    {
        var blockCount = (int)Math.Round((double)(value / maxValue * 10m), MidpointRounding.AwayFromZero);
        if (blockCount <= 0)
        {
            return "-";
        }

        return new string('█', blockCount);
    }
}

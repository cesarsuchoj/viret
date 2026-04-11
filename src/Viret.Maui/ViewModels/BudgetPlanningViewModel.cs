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
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    public BudgetPlanningViewModel(IFinancialPlanningService financialPlanningService)
    {
        _financialPlanningService = financialPlanningService;
        Title = "Orçamento e Planejamento";
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
            var overview = await _financialPlanningService.GetBudgetOverviewAsync(userId, familyId);
            PlannedIncome = overview.PlannedIncome;
            ActualIncome = overview.ActualIncome;
            PlannedExpense = overview.PlannedExpense;
            ActualExpense = overview.ActualExpense;
            PlannedAvailable = overview.PlannedAvailable;
            ActualAvailable = overview.ActualAvailable;
            CategorySummaries = overview.CategorySummaries;
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

    private static bool TryParseAmount(string text, out decimal value)
        => decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out value) ||
           decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
}

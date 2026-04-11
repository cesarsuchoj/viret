using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;
using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Maui.ViewModels;

public partial class IncomeEntryViewModel : BaseViewModel
{
    private readonly IFinancialPlanningService _financialPlanningService;

    [ObservableProperty]
    private string _familyIdText = string.Empty;

    [ObservableProperty]
    private string _userIdText = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _plannedAmountText = string.Empty;

    [ObservableProperty]
    private string _actualAmountText = string.Empty;

    [ObservableProperty]
    private DateTime _date = DateTime.Today;

    [ObservableProperty]
    private IEnumerable<BudgetCategory> _categories = Enumerable.Empty<BudgetCategory>();

    [ObservableProperty]
    private BudgetCategory? _selectedCategory;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    public IncomeEntryViewModel(IFinancialPlanningService financialPlanningService)
    {
        _financialPlanningService = financialPlanningService;
        Title = "Cadastro de Ganho";
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        if (!TryParseFamilyId(out var familyId))
        {
            ErrorMessage = "Informe um ID de família válido.";
            return;
        }

        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            Categories = await _financialPlanningService.GetBudgetCategoriesByFamilyAsync(familyId);
        }
        catch (Exception)
        {
            ErrorMessage = "Não foi possível carregar as categorias de orçamento.";
        }
    }

    [RelayCommand]
    private async Task SaveIncomeAsync()
    {
        if (IsBusy)
            return;

        if (!TryParseFamilyId(out var familyId) || !TryParseUserId(out var userId))
        {
            ErrorMessage = "Informe IDs válidos de família e usuário.";
            return;
        }

        if (!TryParseAmount(PlannedAmountText, out var plannedAmount) || !TryParseAmount(ActualAmountText, out var actualAmount))
        {
            ErrorMessage = "Informe valores previstos e efetivos válidos.";
            return;
        }

        if (SelectedCategory is null)
        {
            ErrorMessage = "Selecione uma categoria de orçamento.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            await _financialPlanningService.AddIncomeAsync(new Income
            {
                FamilyId = familyId,
                UserId = userId,
                Description = Description.Trim(),
                PlannedAmount = plannedAmount,
                ActualAmount = actualAmount,
                Date = Date,
                BudgetCategoryId = SelectedCategory.Id
            });

            SuccessMessage = "Ganho registrado com sucesso.";
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

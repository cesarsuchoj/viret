using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Maui.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly ITransactionService _transactionService;

    [ObservableProperty]
    private decimal _balance;

    [ObservableProperty]
    private IEnumerable<Transaction> _transactions = Enumerable.Empty<Transaction>();

    [ObservableProperty]
    private int _familyId;

    [ObservableProperty]
    private string _familyIdText = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    public MainViewModel(ITransactionService transactionService)
    {
        _transactionService = transactionService;
        Title = "Viret – Gestão Financeira";
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (IsBusy)
            return;

        if (FamilyId <= 0)
        {
            SuccessMessage = string.Empty;
            ErrorMessage = "Informe um ID de família válido para atualizar os dados.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        try
        {
            Transactions = await _transactionService.GetTransactionsByFamilyAsync(FamilyId);
            Balance = await _transactionService.GetBalanceAsync(FamilyId);
            SuccessMessage = "Dados financeiros atualizados com sucesso.";
        }
        catch (Exception)
        {
            ErrorMessage = "Não foi possível atualizar os dados agora. Tente novamente em instantes.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnFamilyIdChanged(int value)
    {
        var valueText = value.ToString();
        if (!string.Equals(FamilyIdText, valueText, StringComparison.Ordinal))
            FamilyIdText = valueText;
    }

    partial void OnFamilyIdTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (FamilyId != 0)
                FamilyId = 0;
            return;
        }

        if (!int.TryParse(value, out var parsedFamilyId) || parsedFamilyId <= 0)
        {
            if (FamilyId != 0)
                FamilyId = 0;
            return;
        }

        if (FamilyId != parsedFamilyId)
            FamilyId = parsedFamilyId;
    }
}

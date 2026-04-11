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

        IsBusy = true;
        try
        {
            Transactions = await _transactionService.GetTransactionsByFamilyAsync(FamilyId);
            Balance = await _transactionService.GetBalanceAsync(FamilyId);
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
            return;

        if (FamilyId != parsedFamilyId)
            FamilyId = parsedFamilyId;
    }
}

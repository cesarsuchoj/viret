using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Maui.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly ITransactionService _transactionService;
    private readonly int _familyId;

    [ObservableProperty]
    private decimal _balance;

    [ObservableProperty]
    private IEnumerable<Transaction> _transactions = Enumerable.Empty<Transaction>();

    public MainViewModel(ITransactionService transactionService)
    {
        _transactionService = transactionService;
        _familyId = 1;
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
            Transactions = await _transactionService.GetTransactionsByFamilyAsync(_familyId);
            Balance = await _transactionService.GetBalanceAsync(_familyId);
        }
        finally
        {
            IsBusy = false;
        }
    }
}

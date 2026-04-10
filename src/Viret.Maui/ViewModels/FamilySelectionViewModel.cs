using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Maui.ViewModels;

public partial class FamilySelectionViewModel : BaseViewModel
{
    private readonly IUserService _userService;

    [ObservableProperty]
    private int _userId;

    [ObservableProperty]
    private IEnumerable<Family> _families = Enumerable.Empty<Family>();

    [ObservableProperty]
    private Family? _selectedFamily;

    [ObservableProperty]
    private bool _hasAccessToSelectedFamily;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public FamilySelectionViewModel(IUserService userService)
    {
        _userService = userService;
        Title = "Seleção de Família";
    }

    [RelayCommand]
    private async Task LoadFamiliesAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            Families = await _userService.GetFamiliesForUserAsync(UserId);
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
    private async Task SelectFamilyAsync()
    {
        if (SelectedFamily is null)
        {
            HasAccessToSelectedFamily = false;
            ErrorMessage = "Selecione uma família.";
            return;
        }

        ErrorMessage = string.Empty;
        HasAccessToSelectedFamily = await _userService.UserHasAccessToFamilyAsync(UserId, SelectedFamily.Id);
    }
}

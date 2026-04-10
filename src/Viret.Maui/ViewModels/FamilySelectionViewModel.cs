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
    private string _userIdText = string.Empty;

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

        if (!TryParseUserId(out var parsedUserId))
        {
            ErrorMessage = "Informe um ID de usuário válido.";
            Families = Enumerable.Empty<Family>();
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            UserId = parsedUserId;
            Families = await _userService.GetFamiliesForUserAsync(parsedUserId);
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
        if (!TryParseUserId(out var parsedUserId))
        {
            HasAccessToSelectedFamily = false;
            ErrorMessage = "Informe um ID de usuário válido.";
            return;
        }

        if (SelectedFamily is null)
        {
            HasAccessToSelectedFamily = false;
            ErrorMessage = "Selecione uma família.";
            return;
        }

        ErrorMessage = string.Empty;
        UserId = parsedUserId;
        HasAccessToSelectedFamily = await _userService.UserHasAccessToFamilyAsync(parsedUserId, SelectedFamily.Id);
    }

    private bool TryParseUserId(out int parsedUserId)
    {
        if (!int.TryParse(UserIdText, out parsedUserId))
        {
            parsedUserId = 0;
            return false;
        }

        return parsedUserId > 0;
    }
}

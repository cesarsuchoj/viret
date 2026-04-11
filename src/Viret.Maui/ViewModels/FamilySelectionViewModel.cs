using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Viret.Core.Interfaces;
using Viret.Core.Models;

namespace Viret.Maui.ViewModels;

public partial class FamilySelectionViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private readonly IFamilyService _familyService;

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

    [ObservableProperty]
    private string _newFamilyName = string.Empty;

    [ObservableProperty]
    private bool _showCreateFamilyOption;

    public FamilySelectionViewModel(IUserService userService, IFamilyService familyService)
    {
        _userService = userService;
        _familyService = familyService;
        Title = "Seleção de Família";
    }

    [RelayCommand]
    private async Task LoadFamiliesAsync()
    {
        if (IsBusy)
            return;

        ShowCreateFamilyOption = false;
        SelectedFamily = null;

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
            Families = (await _userService.GetFamiliesForUserAsync(parsedUserId)).ToList();
            SelectedFamily = Families.FirstOrDefault();
            ShowCreateFamilyOption = !Families.Any();

            if (ShowCreateFamilyOption)
                ErrorMessage = "Você ainda não participa de nenhuma família. Crie uma nova.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            ShowCreateFamilyOption = false;
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

        if (!HasAccessToSelectedFamily)
            ErrorMessage = "Sem acesso à família selecionada.";
    }

    [RelayCommand]
    private async Task CreateFamilyAsync()
    {
        if (IsBusy)
            return;

        if (!TryParseUserId(out var parsedUserId))
        {
            ErrorMessage = "Informe um ID de usuário válido.";
            return;
        }

        if (string.IsNullOrWhiteSpace(NewFamilyName))
        {
            ErrorMessage = "Informe o nome da nova família.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            UserId = parsedUserId;

            var family = await _familyService.CreateFamilyAsync(new Family
            {
                Name = NewFamilyName.Trim()
            });

            await _userService.AddUserToFamilyAsync(parsedUserId, family.Id, FamilyRole.Admin);

            Families = (await _userService.GetFamiliesForUserAsync(parsedUserId)).ToList();
            SelectedFamily = Families.FirstOrDefault(f => f.Id == family.Id) ?? Families.FirstOrDefault();
            ShowCreateFamilyOption = !Families.Any();
            NewFamilyName = string.Empty;
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

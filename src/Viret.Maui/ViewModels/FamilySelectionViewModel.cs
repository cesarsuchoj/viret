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
    private string _successMessage = string.Empty;

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
            SuccessMessage = string.Empty;
            ErrorMessage = "Informe um ID de usuário válido.";
            Families = Enumerable.Empty<Family>();
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            UserId = parsedUserId;
            Families = (await _userService.GetFamiliesForUserAsync(parsedUserId)).ToList();
            SelectedFamily = Families.FirstOrDefault();
            ShowCreateFamilyOption = !Families.Any();

            if (ShowCreateFamilyOption)
                ErrorMessage = "Você ainda não participa de nenhuma família. Crie uma nova.";
            else
                SuccessMessage = "Famílias carregadas com sucesso.";
        }
        catch (Exception)
        {
            ErrorMessage = "Não foi possível carregar as famílias agora. Tente novamente em instantes.";
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
        if (IsBusy)
            return;

        if (!TryParseUserId(out var parsedUserId))
        {
            HasAccessToSelectedFamily = false;
            SuccessMessage = string.Empty;
            ErrorMessage = "Informe um ID de usuário válido.";
            return;
        }

        if (SelectedFamily is null)
        {
            HasAccessToSelectedFamily = false;
            SuccessMessage = string.Empty;
            ErrorMessage = "Selecione uma família.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            UserId = parsedUserId;
            HasAccessToSelectedFamily = await _userService.UserHasAccessToFamilyAsync(parsedUserId, SelectedFamily.Id);

            if (!HasAccessToSelectedFamily)
                ErrorMessage = "Sem acesso à família selecionada.";
            else
                SuccessMessage = "Acesso à família confirmado.";
        }
        catch (Exception)
        {
            HasAccessToSelectedFamily = false;
            ErrorMessage = "Não foi possível validar o acesso à família. Tente novamente.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CreateFamilyAsync()
    {
        if (IsBusy)
            return;

        if (!TryParseUserId(out var parsedUserId))
        {
            SuccessMessage = string.Empty;
            ErrorMessage = "Informe um ID de usuário válido.";
            return;
        }

        if (string.IsNullOrWhiteSpace(NewFamilyName))
        {
            SuccessMessage = string.Empty;
            ErrorMessage = "Informe o nome da nova família.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

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
            SuccessMessage = "Família criada e vinculada com sucesso.";
        }
        catch (Exception)
        {
            ErrorMessage = "Não foi possível criar a família agora. Verifique os dados e tente novamente.";
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

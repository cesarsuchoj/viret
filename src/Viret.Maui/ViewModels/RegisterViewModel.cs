using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Viret.Core.Interfaces;

namespace Viret.Maui.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IUserService _userService;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    [ObservableProperty]
    private int? _createdUserId;

    public RegisterViewModel(IUserService userService)
    {
        _userService = userService;
        Title = "Cadastro";
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            CreatedUserId = null;
            SuccessMessage = string.Empty;
            ErrorMessage = "Preencha nome, e-mail e senha para criar sua conta.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        CreatedUserId = null;

        try
        {
            var user = await _userService.RegisterAsync(Name, Email, Password);
            CreatedUserId = user.Id;
            SuccessMessage = "Conta criada com sucesso.";
        }
        catch (Exception)
        {
            ErrorMessage = "Não foi possível criar sua conta agora. Revise os dados e tente novamente.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}

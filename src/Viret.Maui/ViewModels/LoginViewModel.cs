using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Viret.Core.Interfaces;

namespace Viret.Maui.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IUserService _userService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private int? _authenticatedUserId;

    public LoginViewModel(IUserService userService)
    {
        _userService = userService;
        Title = "Login";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Informe e-mail e senha.";
            AuthenticatedUserId = null;
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        AuthenticatedUserId = null;

        try
        {
            var user = await _userService.LoginAsync(Email, Password);
            AuthenticatedUserId = user.Id;
        }
        catch (UnauthorizedAccessException)
        {
            ErrorMessage = "E-mail ou senha invalidos.";
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
}

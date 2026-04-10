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

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var user = await _userService.RegisterAsync(Name, Email, Password);
            CreatedUserId = user.Id;
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

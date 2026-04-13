using Microsoft.Maui.Controls;
using Viret.Maui.Views;

namespace Viret.Maui;

public class App : Application
{
    public App(LoginPage loginPage)
    {
        MainPage = new NavigationPage(loginPage);
        AccessibilityUi.ApplyToApplication();
    }
}

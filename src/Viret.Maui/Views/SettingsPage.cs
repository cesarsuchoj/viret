using Microsoft.Maui.Controls;

namespace Viret.Maui.Views;

public class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        Title = "Configurações";

        Content = new VerticalStackLayout
        {
            Padding = 24,
            Spacing = 12,
            Children =
            {
                new Label
                {
                    Text = "Configurações",
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold
                },
                new Label
                {
                    Text = "Área para preferências gerais da aplicação."
                }
            }
        };
    }
}

using Microsoft.Maui.Controls;

namespace Viret.Maui.Views;

public class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        Title = "Configurações";

        var largeTextDescriptionLabel = new Label
        {
            Text = "Aumentar fonte para melhorar legibilidade em toda a aplicação.",
            LineBreakMode = LineBreakMode.WordWrap
        };

        var largeTextSwitch = new Switch
        {
            IsToggled = AccessibilityUi.IsLargeTextEnabled()
        };
        AutomationProperties.SetName(largeTextSwitch, "Alternar aumento de fonte");
        SemanticProperties.SetDescription(largeTextSwitch, "Habilita fonte ampliada para melhorar acessibilidade visual");
        largeTextSwitch.Toggled += (_, args) =>
        {
            AccessibilityUi.SetLargeTextEnabled(args.Value);
            AccessibilityUi.ApplyToApplication();
        };

        var content = new ScrollView
        {
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
                    },
                    new HorizontalStackLayout
                    {
                        Spacing = 12,
                        Children =
                        {
                            new Label
                            {
                                Text = "Aumentar fonte",
                                VerticalTextAlignment = TextAlignment.Center
                            },
                            largeTextSwitch
                        }
                    },
                    largeTextDescriptionLabel
                }
            }
        };

        AccessibilityUi.ApplyToView(content);
        Content = content;
    }
}

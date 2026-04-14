using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Viret.Maui.Views;

internal static class AccessibilityUi
{
    private const string LargeTextPreferenceKey = "accessibility.largeTextEnabled";
    private const double DefaultFontScale = 1.0;
    private const double LargeFontScale = 1.2;
    private const double MinimumTouchTargetHeight = 44.0;
    private static readonly BindableProperty BaseFontSizeProperty = BindableProperty.CreateAttached(
        "BaseFontSize",
        typeof(double),
        typeof(AccessibilityUi),
        -1.0);

    public static bool IsLargeTextEnabled()
        => Preferences.Default.Get(LargeTextPreferenceKey, false);

    public static void SetLargeTextEnabled(bool enabled)
        => Preferences.Default.Set(LargeTextPreferenceKey, enabled);

    public static void ApplyToView(IView? root)
    {
        if (root is not null)
        {
            ApplyRecursive(root, GetFontScale());
        }
    }

    public static void ApplyToApplication()
    {
        if (Application.Current?.MainPage is not null)
        {
            ApplyToPage(Application.Current.MainPage, GetFontScale());
        }
    }

    private static double GetFontScale()
        => IsLargeTextEnabled() ? LargeFontScale : DefaultFontScale;

    private static void ApplyToPage(Page page, double scale)
    {
        switch (page)
        {
            case Shell shell:
                foreach (var shellItem in shell.Items)
                {
                    foreach (var shellSection in shellItem.Items)
                    {
                        foreach (var shellContent in shellSection.Items)
                        {
                            if (shellContent.Content is not null)
                            {
                                ApplyToPage(shellContent.Content, scale);
                            }
                        }
                    }
                }

                if (shell.CurrentPage is not null)
                {
                    ApplyToPage(shell.CurrentPage, scale);
                }

                break;
            case NavigationPage navigationPage:
                foreach (var navigationStackPage in navigationPage.Navigation.NavigationStack)
                {
                    ApplyToPage(navigationStackPage, scale);
                }

                if (navigationPage.CurrentPage is not null)
                {
                    ApplyToPage(navigationPage.CurrentPage, scale);
                }

                break;
            case ContentPage contentPage when contentPage.Content is not null:
                ApplyRecursive(contentPage.Content, scale);
                break;
        }
    }

    private static void ApplyRecursive(IView view, double scale)
    {
        ApplyTouchTarget(view);
        ApplySemanticDefaults(view);
        ApplyFontScale(view, scale);

        if (view is ScrollView scrollView && scrollView.Content is IView scrollContent)
        {
            ApplyRecursive(scrollContent, scale);
        }

        if (view is ContentView contentView && contentView.Content is IView content)
        {
            ApplyRecursive(content, scale);
        }

        if (view is Border border && border.Content is IView borderContent)
        {
            ApplyRecursive(borderContent, scale);
        }

        if (view is Layout layout)
        {
            foreach (var child in layout.Children)
            {
                ApplyRecursive(child, scale);
            }
        }
    }

    private static void ApplyTouchTarget(IView view)
    {
        if (view is not VisualElement visualElement)
        {
            return;
        }

        if (view is Button or Entry or Picker or DatePicker or Switch)
        {
            if (visualElement.HeightRequest is < MinimumTouchTargetHeight and > 0)
            {
                visualElement.HeightRequest = MinimumTouchTargetHeight;
            }
            else if (visualElement.HeightRequest <= 0)
            {
                visualElement.HeightRequest = MinimumTouchTargetHeight;
            }
        }
    }

    private static void ApplySemanticDefaults(IView view)
    {
        switch (view)
        {
            case Button button when string.IsNullOrWhiteSpace(AutomationProperties.GetName(button)):
                AutomationProperties.SetName(button, button.Text);
                SemanticProperties.SetDescription(button, button.Text);
                break;
            case Entry entry when string.IsNullOrWhiteSpace(AutomationProperties.GetName(entry)):
                var entryName = !string.IsNullOrWhiteSpace(entry.Placeholder) ? entry.Placeholder : "Campo de texto";
                AutomationProperties.SetName(entry, entryName);
                SemanticProperties.SetDescription(entry, entryName);
                break;
            case Picker picker when string.IsNullOrWhiteSpace(AutomationProperties.GetName(picker)):
                var pickerName = !string.IsNullOrWhiteSpace(picker.Title) ? picker.Title : "Seletor";
                AutomationProperties.SetName(picker, pickerName);
                SemanticProperties.SetDescription(picker, pickerName);
                break;
            case DatePicker datePicker when string.IsNullOrWhiteSpace(AutomationProperties.GetName(datePicker)):
                const string datePickerName = "Selecionar data";
                AutomationProperties.SetName(datePicker, datePickerName);
                SemanticProperties.SetDescription(datePicker, datePickerName);
                break;
            case ActivityIndicator indicator when string.IsNullOrWhiteSpace(AutomationProperties.GetName(indicator)):
                const string loadingIndicatorName = "Indicador de carregamento";
                AutomationProperties.SetName(indicator, loadingIndicatorName);
                SemanticProperties.SetDescription(indicator, loadingIndicatorName);
                break;
        }
    }

    private static void ApplyFontScale(IView view, double scale)
    {
        switch (view)
        {
            case Label label:
                label.FontSize = GetScaledFontSize(label, label.FontSize, NamedSize.Body, scale);
                break;
            case Button button:
                button.FontSize = GetScaledFontSize(button, button.FontSize, NamedSize.Body, scale);
                break;
            case Entry entry:
                entry.FontSize = GetScaledFontSize(entry, entry.FontSize, NamedSize.Body, scale);
                break;
            case Picker picker:
                picker.FontSize = GetScaledFontSize(picker, picker.FontSize, NamedSize.Body, scale);
                break;
            case DatePicker datePicker:
                datePicker.FontSize = GetScaledFontSize(datePicker, datePicker.FontSize, NamedSize.Body, scale);
                break;
        }
    }

    private static double GetScaledFontSize(BindableObject control, double currentFontSize, NamedSize namedSize, double scale)
    {
        var baseFontSize = (double)control.GetValue(BaseFontSizeProperty);
        if (baseFontSize <= 0)
        {
            baseFontSize = currentFontSize > 0
                ? currentFontSize
                : Device.GetNamedSize(namedSize, control.GetType());
            control.SetValue(BaseFontSizeProperty, baseFontSize);
        }

        return Math.Round(baseFontSize * scale, 2, MidpointRounding.AwayFromZero);
    }
}

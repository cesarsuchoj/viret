using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Globalization;

namespace Viret.Maui.Views;

internal static class FeedbackUi
{
    private static readonly IValueConverter HasTextConverter = new HasTextValueConverter();

    public static readonly Color SuccessColor = Color.FromArgb("#1E8E3E");
    public static readonly Color ErrorColor = Color.FromArgb("#C62828");
    public static readonly Color LoadingColor = Color.FromArgb("#1565C0");

    public static Label CreateSuccessLabel(string bindingPath)
        => CreateMessageLabel(bindingPath, SuccessColor, "✅");

    public static Label CreateErrorLabel(string bindingPath)
        => CreateMessageLabel(bindingPath, ErrorColor, "⚠️");

    public static HorizontalStackLayout CreateLoadingFeedback(string busyBindingPath = "IsBusy")
    {
        var indicator = new ActivityIndicator { Color = LoadingColor };
        indicator.SetBinding(ActivityIndicator.IsRunningProperty, busyBindingPath);
        indicator.SetBinding(ActivityIndicator.IsVisibleProperty, busyBindingPath);

        var loadingLabel = new Label
        {
            Text = "⏳ Processando...",
            TextColor = LoadingColor,
            VerticalTextAlignment = TextAlignment.Center
        };
        loadingLabel.SetBinding(VisualElement.IsVisibleProperty, busyBindingPath);

        var loadingFeedback = new HorizontalStackLayout
        {
            Spacing = 8,
            Children = { indicator, loadingLabel }
        };
        loadingFeedback.SetBinding(VisualElement.IsVisibleProperty, busyBindingPath);
        return loadingFeedback;
    }

    private static Label CreateMessageLabel(string bindingPath, Color textColor, string icon)
    {
        var label = new Label { TextColor = textColor, LineBreakMode = LineBreakMode.WordWrap };
        label.SetBinding(Label.TextProperty, new Binding(bindingPath, stringFormat: $"{icon} {{0}}"));
        label.SetBinding(VisualElement.IsVisibleProperty, new Binding(bindingPath, converter: HasTextConverter));
        return label;
    }

    private sealed class HasTextValueConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => !string.IsNullOrWhiteSpace(value as string);

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}

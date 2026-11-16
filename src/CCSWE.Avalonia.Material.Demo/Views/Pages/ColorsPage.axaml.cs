using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace CCSWE.Avalonia.Material.Demo.Views.Pages;

public partial class ColorsPage : GalleryPage
{
    // (container, on-color) pairs, as the theme's control themes and consumers pair them.
    private static readonly (string Background, string Foreground)[] PairRoles =
    [
        ("Surface", "OnSurface"),
        ("SurfaceContainerLow", "OnSurface"),
        ("SurfaceContainerHighest", "OnSurfaceVariant"),
        ("Primary", "OnPrimary"),
        ("PrimaryFill", "OnPrimaryFill"),
        ("PrimaryContainer", "OnPrimaryContainer"),
        ("Secondary", "OnSecondary"),
        ("SecondaryContainer", "OnSecondaryContainer"),
        ("Tertiary", "OnTertiary"),
        ("TertiaryContainer", "OnTertiaryContainer"),
        ("Error", "OnError"),
        ("ErrorFill", "OnErrorFill"),
        ("ErrorContainer", "OnErrorContainer"),
        ("Success", "OnSuccess"),
        ("SuccessContainer", "OnSuccessContainer"),
        ("Warning", "OnWarning"),
        ("WarningContainer", "OnWarningContainer"),
        ("Info", "OnInfo"),
        ("InfoContainer", "OnInfoContainer"),
        ("InverseSurface", "InverseOnSurface"),
    ];

    // Text roles shown on the surfaces they're used on.
    private static readonly (string Background, string Foreground)[] TextRoles =
    [
        ("Surface", "OnSurfaceStrong"),
        ("Surface", "OnSurfaceVariant"),
        ("Surface", "OnSurfaceMuted"),
        ("Surface", "Accent"),
        ("Surface", "AccentStrong"),
        ("SurfaceContainerLow", "Accent"),
        ("InverseSurface", "InversePrimary"),
    ];

    public ColorsPage()
    {
        InitializeComponent();

        foreach (var (background, foreground) in PairRoles)
        {
            Pairs.Children.Add(Swatch(background, foreground, $"{foreground} on {background}"));
        }

        foreach (var (background, foreground) in TextRoles)
        {
            TextOnSurfaces.Children.Add(Swatch(background, foreground, $"{foreground} on {background}"));
        }
    }

    // A swatch whose brushes follow the theme variant (bound to the resource, not resolved once).
    private static Border Swatch(string background, string foreground, string label)
    {
        var text = new TextBlock
        {
            Text = label,
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Center,
        };
        text.Classes.Add("LabelLarge");

        var swatch = new Border
        {
            Width = 220,
            Height = 64,
            Margin = new Thickness(0, 0, 12, 12),
            Padding = new Thickness(12),
            BorderThickness = new Thickness(1),
            Child = text,
        };
        swatch.Bind(Border.BackgroundProperty, swatch.GetResourceObservable(background));
        swatch.Bind(Border.BorderBrushProperty, swatch.GetResourceObservable("OutlineVariant"));
        swatch.Bind(Border.CornerRadiusProperty, swatch.GetResourceObservable("CornerRadiusMedium"));
        text.Bind(TextBlock.ForegroundProperty, text.GetResourceObservable(foreground));

        return swatch;
    }
}

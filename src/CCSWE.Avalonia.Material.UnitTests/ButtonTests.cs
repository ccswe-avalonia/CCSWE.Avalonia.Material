using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Styling;
using CCSWE.Avalonia.Material;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ButtonTests
{
    protected static IEnumerable<ThemeVariant> Variants =>
    [
        ThemeVariant.Dark,
        ThemeVariant.Light,
        MaterialTheme.DarkHighContrast,
        MaterialTheme.LightHighContrast,
    ];

    // The brush the applied theme resolves for `key` in the window's variant.
    protected static object? Resource(Window window, string key) =>
        Application.Current!.TryGetResource(key, window.ActualThemeVariant, out var value) ? value : null;

    protected static (Window Window, Button Button) Show(Button button, ThemeVariant variant)
    {
        var window = new Window { RequestedThemeVariant = variant, Content = button };
        window.Show();
        return (window, button);
    }

    public class When_A_Filled_Button_Is_Shown : ButtonTests
    {
        [AvaloniaTest]
        public void It_paints_the_primary_fill_pair([ValueSource(nameof(Variants))] ThemeVariant variant)
        {
            var (window, sut) = Show(new Button { Content = "OK", Classes = { "Filled" } }, variant);

            Assert.Multiple(() =>
            {
                Assert.That(sut.Background, Is.SameAs(Resource(window, "PrimaryFill")));
                Assert.That(sut.Foreground, Is.SameAs(Resource(window, "OnPrimaryFill")));
                Assert.That(sut.Background, Is.Not.SameAs(Resource(window, "Primary")));
            });

            window.Close();
        }

        [AvaloniaTest]
        public void It_is_the_default_button_theme([ValueSource(nameof(Variants))] ThemeVariant variant)
        {
            var (window, sut) = Show(new Button { Content = "OK" }, variant);

            Assert.That(sut.Background, Is.SameAs(Resource(window, "PrimaryFill")));

            window.Close();
        }
    }

    public class When_A_Destructive_Button_Is_Shown : ButtonTests
    {
        [AvaloniaTest]
        public void It_paints_the_error_fill_pair([ValueSource(nameof(Variants))] ThemeVariant variant)
        {
            var (window, sut) = Show(new Button { Content = "Delete", Classes = { "Destructive" } }, variant);

            Assert.Multiple(() =>
            {
                Assert.That(sut.Background, Is.SameAs(Resource(window, "ErrorFill")));
                Assert.That(sut.Foreground, Is.SameAs(Resource(window, "OnErrorFill")));
            });

            window.Close();
        }
    }
}

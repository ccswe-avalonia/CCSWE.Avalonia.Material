using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.NUnit;
using Avalonia.Styling;
using Avalonia.VisualTree;
using CCSWE.Avalonia.Material;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class HyperlinkButtonTests
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

    public class When_A_Hyperlink_Button_Is_Shown : HyperlinkButtonTests
    {
        [AvaloniaTest]
        public void It_paints_its_label_in_accent([ValueSource(nameof(Variants))] ThemeVariant variant)
        {
            var sut = new HyperlinkButton { Content = "Docs" };
            var window = new Window { RequestedThemeVariant = variant, Content = sut };
            window.Show();

            var presenter = sut.GetVisualDescendants().OfType<ContentPresenter>().Single(p => p.Name == "PART_ContentPresenter");

            Assert.Multiple(() =>
            {
                Assert.That(presenter.Foreground, Is.SameAs(Resource(window, "Accent")));
                Assert.That(presenter.Foreground, Is.Not.SameAs(Resource(window, "Primary")));
            });

            window.Close();
        }
    }
}

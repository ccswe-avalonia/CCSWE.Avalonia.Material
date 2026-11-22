using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using NUnit.Framework;
using Path = Avalonia.Controls.Shapes.Path;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class MaterialSymbolIconTests
{
    // CalendarDayButton's blackout X stretches to the cell; the window caption glyphs stay cropped
    // and centered like desktop caption buttons.
    private static readonly string[] AllowedFiles =
    [
        "Base/WindowDrawnDecorations.axaml",
        "Controls/CalendarDayButton.axaml",
    ];

    private static readonly Regex IconElement =
        new(@"<(?<tag>[\w:.]+)\b[^>]*Data=""\{DynamicResource\s+Material_Icon\w+\s*\}""[^>]*>", RegexOptions.Compiled);

    private static (PathIcon Icon, Path Glyph) Show(string iconKey)
    {
        var app = Application.Current!;
        var icon = new PathIcon
        {
            Theme = (ControlTheme)app.FindResource("Material_SymbolTheme")!,
            Data = (Geometry)app.FindResource(iconKey)!,
        };

        var window = new Window { Content = new StackPanel { Children = { icon } } };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        return (icon, icon.GetVisualDescendants().OfType<Path>().Single());
    }

    public class When_The_Theme_Is_Applied : MaterialSymbolIconTests
    {
        [AvaloniaTest]
        public void It_defaults_to_24()
        {
            var (icon, _) = Show("Material_IconClose");

            Assert.That(icon.Bounds.Size, Is.EqualTo(new Size(24, 24)));
        }

        [AvaloniaTest]
        public void It_lays_the_glyph_out_in_the_full_960_frame()
        {
            var (_, glyph) = Show("Material_IconClose");

            Assert.That(glyph.Bounds.Size, Is.EqualTo(new Size(960, 960)));
        }

        // Close and Remove are centered in Material's frame and use only straight segments, so their
        // headless bounds are exact.
        [AvaloniaTest]
        [TestCase("Material_IconClose")]
        [TestCase("Material_IconRemove")]
        public void It_centers_a_centered_glyph_in_the_icon(string iconKey)
        {
            var (icon, glyph) = Show(iconKey);
            var transform = glyph.TransformToVisual(icon)!.Value;

            var center = glyph.Data!.Bounds.TransformToAABB(transform).Center;

            Assert.That(center.X, Is.EqualTo(12).Within(0.01));
            Assert.That(center.Y, Is.EqualTo(12).Within(0.01));
        }
    }

    public class When_The_Library_Xaml_Is_Scanned : MaterialSymbolIconTests
    {
        [Test]
        public void It_renders_every_material_icon_with_the_symbol_theme()
        {
            var violations = new List<string>();

            foreach (var (path, text) in EmbeddedXaml.ReadAll())
            {
                if (AllowedFiles.Contains(path, StringComparer.Ordinal))
                {
                    continue;
                }

                foreach (Match match in IconElement.Matches(text))
                {
                    if (match.Groups["tag"].Value != "PathIcon" || !match.Value.Contains("Material_SymbolTheme", StringComparison.Ordinal))
                    {
                        violations.Add($"{path}: {match.Value}");
                    }
                }
            }

            Assert.That(violations, Is.Empty, "Material_Icon* drawn without Material_SymbolTheme:" + Environment.NewLine + string.Join(Environment.NewLine, violations));
        }
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Headless.NUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ToggleSwitchTests
{
    private static MaterialTheme Theme => Application.Current!.Styles.OfType<MaterialTheme>().First();

    // Density, class, IsChecked, IsEnabled and pseudo-classes are set before Show(): changing
    // them afterwards starts the thumb's slide/Width/Height transitions, and the headless render
    // timer runs on a real clock.
    private static ThumbInfo Inspect(
        bool isChecked,
        DensityStyle density = DensityStyle.Normal,
        bool compactClass = false,
        bool disabled = false,
        params string[] pseudoClasses)
    {
        try
        {
            Theme.DensityStyle = density;

            // Left-aligned so the control is only as wide as its template, and a horizontal overhang shows.
            var toggle = new ToggleSwitch { IsChecked = isChecked, IsEnabled = !disabled, HorizontalAlignment = HorizontalAlignment.Left };
            if (compactClass)
            {
                toggle.Classes.Add("Compact");
            }

            foreach (var pseudoClass in pseudoClasses)
            {
                // Classes.Add rejects pseudo-classes from outside the control; IPseudoClasses is
                // the interface the control itself uses, so tests can drive :pressed/:pointerover.
                ((IPseudoClasses)toggle.Classes).Add(pseudoClass);
            }

            var window = new Window { Content = new StackPanel { Children = { toggle } } };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var track = toggle.GetVisualDescendants().OfType<Control>().Single(c => c.Name == "SwitchOuter");
            var thumb = toggle.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "Thumb");
            var stateLayer = toggle.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "StateLayer");

            return new ThumbInfo(
                thumb.TranslatePoint(new Point(thumb.Bounds.Width / 2, thumb.Bounds.Height / 2), track)!.Value,
                thumb.Bounds.Width,
                stateLayer.Opacity,
                ((ISolidColorBrush)thumb.Background!).Color,
                ((ISolidColorBrush)stateLayer.Background!).Color,
                toggle.ActualThemeVariant,
                new Rect(stateLayer.TranslatePoint(default, toggle)!.Value, stateLayer.Bounds.Size),
                toggle.Bounds.Size);
        }
        finally
        {
            Theme.DensityStyle = DensityStyle.Normal;
        }
    }

    private static Color ThemeColor(string key, ThemeVariant? variant)
    {
        Assert.That(Application.Current!.TryGetResource(key, variant, out var value), Is.True, $"Theme resource '{key}' did not resolve.");

        return ((ISolidColorBrush)value!).Color;
    }

    public class When_DensityStyle_Is_Normal : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_centers_the_unchecked_thumb_16_from_the_start() =>
            Assert.That(Inspect(isChecked: false).Center, Is.EqualTo(new Point(16, 16)));

        [AvaloniaTest]
        public void It_centers_the_checked_thumb_16_from_the_end() =>
            Assert.That(Inspect(isChecked: true).Center, Is.EqualTo(new Point(52 - 16, 16)));
    }

    public class When_DensityStyle_Is_Compact : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_centers_the_unchecked_thumb_12_from_the_start() =>
            Assert.That(Inspect(isChecked: false, DensityStyle.Compact).Center, Is.EqualTo(new Point(12, 12)));

        [AvaloniaTest]
        public void It_centers_the_checked_thumb_12_from_the_end() =>
            Assert.That(Inspect(isChecked: true, DensityStyle.Compact).Center, Is.EqualTo(new Point(40 - 12, 12)));
    }

    public class When_Compact_Class_Is_Applied : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_centers_the_unchecked_thumb_12_from_the_start() =>
            Assert.That(Inspect(isChecked: false, compactClass: true).Center, Is.EqualTo(new Point(12, 12)));

        [AvaloniaTest]
        public void It_centers_the_checked_thumb_12_from_the_end() =>
            Assert.That(Inspect(isChecked: true, compactClass: true).Center, Is.EqualTo(new Point(40 - 12, 12)));
    }

    public class When_Idle : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_hides_the_state_layer() =>
            Assert.That(Inspect(isChecked: false).StateLayerOpacity, Is.EqualTo(0));

        [AvaloniaTest]
        public void It_tints_the_unchecked_state_layer_on_surface()
        {
            var info = Inspect(isChecked: false);

            Assert.That(info.StateLayerColor, Is.EqualTo(ThemeColor("OnSurface", info.Variant)));
        }

        [AvaloniaTest]
        public void It_tints_the_checked_state_layer_primary()
        {
            var info = Inspect(isChecked: true);

            Assert.That(info.StateLayerColor, Is.EqualTo(ThemeColor("Primary", info.Variant)));
        }
    }

    public class When_PointerOver : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_shows_the_state_layer_at_8_percent() =>
            Assert.That(Inspect(isChecked: false, pseudoClasses: ":pointerover").StateLayerOpacity, Is.EqualTo(0.08));
    }

    public class When_Pressed : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_grows_the_unchecked_thumb_to_28() =>
            Assert.That(Inspect(isChecked: false, pseudoClasses: ":pressed").ThumbWidth, Is.EqualTo(28));

        [AvaloniaTest]
        public void It_grows_the_checked_thumb_to_28() =>
            Assert.That(Inspect(isChecked: true, pseudoClasses: ":pressed").ThumbWidth, Is.EqualTo(28));

        [AvaloniaTest]
        public void It_grows_the_compact_density_thumb_to_21() =>
            Assert.That(Inspect(isChecked: false, DensityStyle.Compact, pseudoClasses: ":pressed").ThumbWidth, Is.EqualTo(21));

        [AvaloniaTest]
        public void It_grows_the_compact_class_thumb_to_21() =>
            Assert.That(Inspect(isChecked: false, compactClass: true, pseudoClasses: ":pressed").ThumbWidth, Is.EqualTo(21));

        [AvaloniaTest]
        public void It_keeps_the_unchecked_thumb_centered() =>
            Assert.That(Inspect(isChecked: false, pseudoClasses: ":pressed").Center, Is.EqualTo(new Point(16, 16)));

        [AvaloniaTest]
        public void It_keeps_the_checked_thumb_centered() =>
            Assert.That(Inspect(isChecked: true, pseudoClasses: ":pressed").Center, Is.EqualTo(new Point(52 - 16, 16)));

        [AvaloniaTest]
        public void It_shows_the_state_layer_at_10_percent() =>
            Assert.That(Inspect(isChecked: false, pseudoClasses: ":pressed").StateLayerOpacity, Is.EqualTo(0.10));
    }

    // The state layer overhangs the track; the control reserves the overhang so a clipping parent can't cut it off (#78).
    public class When_The_State_Layer_Is_Laid_Out : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_fits_the_unchecked_state_layer_in_the_control() =>
            Assert.That(Inspect(isChecked: false).StateLayer, Is.EqualTo(new Rect(0, 0, 40, 40)));

        [AvaloniaTest]
        public void It_fits_the_checked_state_layer_in_the_control() =>
            Assert.That(Inspect(isChecked: true).StateLayer, Is.EqualTo(new Rect(20, 0, 40, 40)));

        [AvaloniaTest]
        public void It_fits_the_pressed_state_layer_in_the_control() =>
            Assert.That(Inspect(isChecked: false, pseudoClasses: ":pressed").StateLayer, Is.EqualTo(new Rect(0, 0, 40, 40)));

        [AvaloniaTest]
        public void It_makes_the_control_40_high() =>
            Assert.That(Inspect(isChecked: false).ToggleSize.Height, Is.EqualTo(40));

        [AvaloniaTest]
        public void It_fits_the_compact_density_state_layer_in_the_control()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Inspect(isChecked: false, DensityStyle.Compact).StateLayer, Is.EqualTo(new Rect(0, 0, 30, 30)));
                Assert.That(Inspect(isChecked: true, DensityStyle.Compact).StateLayer, Is.EqualTo(new Rect(16, 0, 30, 30)));
                Assert.That(Inspect(isChecked: false, DensityStyle.Compact).ToggleSize.Height, Is.EqualTo(30));
            }
        }

        [AvaloniaTest]
        public void It_fits_the_compact_class_state_layer_in_the_control()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Inspect(isChecked: false, compactClass: true).StateLayer, Is.EqualTo(new Rect(0, 0, 30, 30)));
                Assert.That(Inspect(isChecked: true, compactClass: true).StateLayer, Is.EqualTo(new Rect(16, 0, 30, 30)));
                Assert.That(Inspect(isChecked: false, compactClass: true).ToggleSize.Height, Is.EqualTo(30));
            }
        }
    }

    public class When_Disabled : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_colors_the_unchecked_thumb_on_surface_38()
        {
            var info = Inspect(isChecked: false, disabled: true);

            Assert.That(info.ThumbColor, Is.EqualTo(ThemeColor("OnSurface38", info.Variant)));
        }

        [AvaloniaTest]
        public void It_colors_the_checked_thumb_surface()
        {
            var info = Inspect(isChecked: true, disabled: true);

            Assert.That(info.ThumbColor, Is.EqualTo(ThemeColor("Surface", info.Variant)));
        }

        [AvaloniaTest]
        public void It_hides_the_state_layer_on_pointer_over() =>
            Assert.That(Inspect(isChecked: false, disabled: true, pseudoClasses: ":pointerover").StateLayerOpacity, Is.EqualTo(0));
    }

    private sealed record ThumbInfo(
        Point Center,
        double ThumbWidth,
        double StateLayerOpacity,
        Color ThumbColor,
        Color StateLayerColor,
        ThemeVariant? Variant,
        Rect StateLayer,
        Size ToggleSize);
}

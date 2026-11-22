using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
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

    private sealed record ThumbInfo(
        double CenterX,
        double ThumbWidth,
        double StateLayerOpacity,
        Color ThumbColor,
        Color StateLayerColor,
        ThemeVariant? Variant);

    // Density, class, IsChecked, IsEnabled and pseudo-classes are set before Show(): changing
    // them afterwards starts the thumb's translateX/Width/Height transitions, and the headless
    // render timer runs on a real clock.
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

            var toggle = new ToggleSwitch { IsChecked = isChecked, IsEnabled = !disabled };
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

            var assembly = toggle.GetVisualDescendants().OfType<Panel>().Single(p => p.Name == "ThumbAssembly");
            var thumb = toggle.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "Thumb");
            var stateLayer = toggle.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "StateLayer");
            // The thumb slides via the assembly's Canvas.Left, which is part of its Bounds:
            // the visual thumb center is the assembly center (assembly is track-sized).

            return new ThumbInfo(
                assembly.Bounds.Center.X,
                thumb.Bounds.Width,
                stateLayer.Opacity,
                ((ISolidColorBrush)thumb.Background!).Color,
                ((ISolidColorBrush)stateLayer.Background!).Color,
                toggle.ActualThemeVariant);
        }
        finally
        {
            Theme.DensityStyle = DensityStyle.Normal;
        }
    }

    private static Color ThemeColor(string key, ThemeVariant? variant)
    {
        Assert.That(
            Application.Current!.TryGetResource(key, variant, out var value),
            Is.True,
            $"Theme resource '{key}' did not resolve.");
        return ((ISolidColorBrush)value!).Color;
    }

    public class When_DensityStyle_Is_Normal : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_centers_the_unchecked_thumb_16_from_the_start() =>
            Assert.That(Inspect(isChecked: false).CenterX, Is.EqualTo(16));

        [AvaloniaTest]
        public void It_centers_the_checked_thumb_16_from_the_end() =>
            Assert.That(Inspect(isChecked: true).CenterX, Is.EqualTo(52 - 16));
    }

    public class When_DensityStyle_Is_Compact : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_centers_the_unchecked_thumb_12_from_the_start() =>
            Assert.That(Inspect(isChecked: false, DensityStyle.Compact).CenterX, Is.EqualTo(12));

        [AvaloniaTest]
        public void It_centers_the_checked_thumb_12_from_the_end() =>
            Assert.That(Inspect(isChecked: true, DensityStyle.Compact).CenterX, Is.EqualTo(40 - 12));
    }

    public class When_Compact_Class_Is_Applied : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_centers_the_unchecked_thumb_12_from_the_start() =>
            Assert.That(Inspect(isChecked: false, compactClass: true).CenterX, Is.EqualTo(12));

        [AvaloniaTest]
        public void It_centers_the_checked_thumb_12_from_the_end() =>
            Assert.That(Inspect(isChecked: true, compactClass: true).CenterX, Is.EqualTo(40 - 12));
    }

    public class When_Pressed : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_grows_the_unchecked_thumb_to_28dp() =>
            Assert.That(Inspect(isChecked: false, pseudoClasses: new[] { ":pressed" }).ThumbWidth, Is.EqualTo(28));

        [AvaloniaTest]
        public void It_grows_the_checked_thumb_to_28dp() =>
            Assert.That(Inspect(isChecked: true, pseudoClasses: new[] { ":pressed" }).ThumbWidth, Is.EqualTo(28));

        [AvaloniaTest]
        public void It_keeps_the_pressed_thumb_centered_at_its_rest_position() =>
            Assert.That(Inspect(isChecked: false, pseudoClasses: new[] { ":pressed" }).CenterX, Is.EqualTo(16));

        [AvaloniaTest]
        public void It_grows_the_compact_density_thumb_to_21dp() =>
            Assert.That(
                Inspect(isChecked: false, density: DensityStyle.Compact, pseudoClasses: new[] { ":pressed" }).ThumbWidth,
                Is.EqualTo(21));

        [AvaloniaTest]
        public void It_grows_the_compact_class_thumb_to_21dp() =>
            Assert.That(
                Inspect(isChecked: false, compactClass: true, pseudoClasses: new[] { ":pressed" }).ThumbWidth,
                Is.EqualTo(21));
    }

    public class When_PointerOver : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void The_state_layer_shows_at_8_percent() =>
            Assert.That(Inspect(isChecked: false, pseudoClasses: new[] { ":pointerover" }).StateLayerOpacity, Is.EqualTo(0.08));
    }

    public class When_Pressed_State_Layer : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void The_state_layer_shows_at_12_percent() =>
            Assert.That(Inspect(isChecked: false, pseudoClasses: new[] { ":pressed" }).StateLayerOpacity, Is.EqualTo(0.12));

        [AvaloniaTest]
        public void The_unchecked_state_layer_uses_on_surface_variant()
        {
            var info = Inspect(isChecked: false);
            Assert.That(info.StateLayerColor, Is.EqualTo(ThemeColor("OnSurfaceVariant", info.Variant)));
        }

        [AvaloniaTest]
        public void The_checked_state_layer_uses_primary()
        {
            var info = Inspect(isChecked: true);
            Assert.That(info.StateLayerColor, Is.EqualTo(ThemeColor("Primary", info.Variant)));
        }
    }

    public class When_Disabled : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void The_unchecked_thumb_uses_on_surface_38()
        {
            var info = Inspect(isChecked: false, disabled: true);
            Assert.That(info.ThumbColor, Is.EqualTo(ThemeColor("OnSurface38", info.Variant)));
        }

        [AvaloniaTest]
        public void The_checked_thumb_uses_surface()
        {
            var info = Inspect(isChecked: true, disabled: true);
            Assert.That(info.ThumbColor, Is.EqualTo(ThemeColor("Surface", info.Variant)));
        }

        [AvaloniaTest]
        public void The_state_layer_stays_hidden() =>
            Assert.That(Inspect(isChecked: false, disabled: true).StateLayerOpacity, Is.EqualTo(0));
    }
}

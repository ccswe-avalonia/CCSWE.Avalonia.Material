using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ToggleSwitchTests
{
    private static MaterialTheme Theme => Application.Current!.Styles.OfType<MaterialTheme>().First();

    // Density, class and IsChecked are set before Show(): changing them afterwards starts the thumb's
    // Margin/Width/Height transitions, and the headless render timer runs on a real clock.
    private static Rect ThumbBounds(bool isChecked, DensityStyle density = DensityStyle.Normal, bool compactClass = false)
    {
        try
        {
            Theme.DensityStyle = density;

            var toggle = new ToggleSwitch { IsChecked = isChecked };
            if (compactClass)
            {
                toggle.Classes.Add("Compact");
            }

            var window = new Window { Content = new StackPanel { Children = { toggle } } };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            return toggle.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "Thumb").Bounds;
        }
        finally
        {
            Theme.DensityStyle = DensityStyle.Normal;
        }
    }

    public class When_DensityStyle_Is_Normal : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_centers_the_unchecked_thumb_16_from_the_start() =>
            Assert.That(ThumbBounds(isChecked: false).Center.X, Is.EqualTo(16));

        [AvaloniaTest]
        public void It_centers_the_checked_thumb_16_from_the_end() =>
            Assert.That(ThumbBounds(isChecked: true).Center.X, Is.EqualTo(52 - 16));
    }

    public class When_DensityStyle_Is_Compact : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_centers_the_unchecked_thumb_12_from_the_start() =>
            Assert.That(ThumbBounds(isChecked: false, DensityStyle.Compact).Center.X, Is.EqualTo(12));

        [AvaloniaTest]
        public void It_centers_the_checked_thumb_12_from_the_end() =>
            Assert.That(ThumbBounds(isChecked: true, DensityStyle.Compact).Center.X, Is.EqualTo(40 - 12));
    }

    public class When_Compact_Class_Is_Applied : ToggleSwitchTests
    {
        [AvaloniaTest]
        public void It_centers_the_unchecked_thumb_12_from_the_start() =>
            Assert.That(ThumbBounds(isChecked: false, compactClass: true).Center.X, Is.EqualTo(12));

        [AvaloniaTest]
        public void It_centers_the_checked_thumb_12_from_the_end() =>
            Assert.That(ThumbBounds(isChecked: true, compactClass: true).Center.X, Is.EqualTo(40 - 12));
    }
}

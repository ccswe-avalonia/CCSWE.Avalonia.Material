using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

// The thumb's 40dp state layer overhangs its 20dp thumb; the slider reserves the overhang so a clipping
// parent can't cut it off at either end of the track (#78).
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class SliderTests
{
    private static (Rect StateLayer, Size SliderSize) Inspect(Orientation orientation, double value)
    {
        var slider = orientation == Orientation.Horizontal
            ? new Slider { Orientation = orientation, Width = 200, VerticalAlignment = VerticalAlignment.Top }
            : new Slider { Orientation = orientation, Height = 200, HorizontalAlignment = HorizontalAlignment.Left };
        slider.Value = value;

        var window = new Window { Content = new StackPanel { Children = { slider } } };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        var stateLayer = slider.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "StateLayer");

        return (new Rect(stateLayer.TranslatePoint(default, slider)!.Value, stateLayer.Bounds.Size), slider.Bounds.Size);
    }

    private static void AssertInside((Rect StateLayer, Size SliderSize) info) =>
        Assert.That(new Rect(info.SliderSize).Contains(info.StateLayer), Is.True, $"State layer {info.StateLayer} outside the slider {info.SliderSize}.");

    public class When_Horizontal : SliderTests
    {
        [AvaloniaTest]
        public void It_fits_the_state_layer_at_the_minimum() => AssertInside(Inspect(Orientation.Horizontal, 0));

        [AvaloniaTest]
        public void It_fits_the_state_layer_at_the_maximum() => AssertInside(Inspect(Orientation.Horizontal, 100));
    }

    public class When_Vertical : SliderTests
    {
        [AvaloniaTest]
        public void It_fits_the_state_layer_at_the_minimum() => AssertInside(Inspect(Orientation.Vertical, 0));

        [AvaloniaTest]
        public void It_fits_the_state_layer_at_the_maximum() => AssertInside(Inspect(Orientation.Vertical, 100));

        [AvaloniaTest]
        public void It_makes_the_control_40_wide() => Assert.That(Inspect(Orientation.Vertical, 0).SliderSize.Width, Is.EqualTo(40));
    }
}

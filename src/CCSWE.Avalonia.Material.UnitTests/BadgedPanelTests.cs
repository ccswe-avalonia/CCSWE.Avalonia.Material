using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class BadgedPanelTests
{
    // A 64x64 panel holding a Border and a badge of the given minimum size, measured and arranged.
    private static (Control Child, Control Badge) Construct(Size badgeSize)
    {
        var panel = new BadgedPanel();
        var child = new Border();
        panel.Children.Add(child);
        var badge = new Badge
        {
            MinWidth = badgeSize.Width,
            MinHeight = badgeSize.Height
        };
        panel.Children.Add(badge);

        panel.Measure(new Size(64, 64));
        panel.Arrange(new Rect(0, 0, 64, 64));

        return (child, badge);
    }

    public class When_It_Has_Only_One_Child : BadgedPanelTests
    {
        [AvaloniaTest]
        public void It_arranges_the_child_normally()
        {
            var panel = new BadgedPanel();
            var child = new Border();
            panel.Children.Add(child);

            panel.Measure(new Size(64, 64));
            panel.Arrange(new Rect(0, 0, 64, 64));

            Assert.That(child.Bounds, Is.EqualTo(new Rect(0, 0, 64, 64)));
        }
    }

    public class When_It_Has_Two_Children : BadgedPanelTests
    {
        [AvaloniaTest]
        public void It_arranges_the_first_child_normally()
        {
            var (child, _) = Construct(default);

            Assert.That(child.Bounds, Is.EqualTo(new Rect(0, 0, 64, 64)));
        }

        [AvaloniaTest]
        public void It_arranges_the_small_badge_child_inset()
        {
            var (_, badge) = Construct(new Size(6, 6));

            Assert.That(badge.Bounds, Is.EqualTo(new Rect(58, 0, 6, 6)));
        }

        [AvaloniaTest]
        public void It_arranges_the_large_badge_child_overflowed()
        {
            var (_, badge) = Construct(new Size(16, 16));

            Assert.That(badge.Bounds, Is.EqualTo(new Rect(52, -2, 16, 16)));
        }
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class BadgedPanelTests
{
    public class When_It_Has_Only_One_Child : CardTests
    {
        [AvaloniaTest]
        public void It_arranges_the_child_normally()
        {
            var panel = new BadgedPanel();
            var child = new Border();
            panel.Children.Add(child);

            panel.Measure(new(64, 64));
            panel.Arrange(new(0, 0, 64, 64));

            Assert.That(child.Bounds, Is.EqualTo(new Rect(0, 0, 64, 64)));
        }
    }

    public class When_It_Has_Two_Child : CardTests
    {
        protected static (Control Child, Control Badge) Construct(Size badgeSize)
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

            panel.Measure(new(64, 64));
            panel.Arrange(new(0, 0, 64, 64));

            return (child, badge);
        }

        [AvaloniaTest]
        public void It_arranges_the_first_child_normally()
        {
            var (child, _) = Construct(default);

            Assert.That(child.Bounds, Is.EqualTo(new Rect(0, 0, 64, 64)));
        }

        [AvaloniaTest]
        public void It_arranges_the_small_badge_child_inset()
        {
            var (_, badge) = Construct(new(6, 6));

            Assert.That(badge.Bounds, Is.EqualTo(new Rect(58, 0, 6, 6)));
        }

        [AvaloniaTest]
        public void It_arranges_the_large_badge_child_overflowed()
        {
            var (_, badge) = Construct(new(16, 16));

            Assert.That(badge.Bounds, Is.EqualTo(new Rect(52, -2, 16, 16)));
        }
    }
}

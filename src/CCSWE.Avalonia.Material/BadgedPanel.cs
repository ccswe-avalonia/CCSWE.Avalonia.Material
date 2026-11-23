using Avalonia;
using Avalonia.Controls;

namespace CCSWE.Avalonia.Material;

/// <summary>
/// A panel that arranges all children like <see cref="Panel" /> but the last child (the "badge", usually a
/// <see cref="Badge" />), which is positioned at the top-right corner instead.
/// </summary>
/// <remarks>
/// This panel only works with two or more children. It is equivalent to <see cref="Panel" /> when only one
/// child exists.
/// </remarks>
public sealed class BadgedPanel : Panel
{
    public static readonly StyledProperty<Size> MaxBadgeInsetProperty =
        AvaloniaProperty.Register<BadgedPanel, Size>(nameof(MaxBadgeInset), new Size(12, 14));
    /// <summary>
    /// The maximum distance from the panel's top-right corner to the badge's bottom-left corner.
    /// </summary>
    /// <remarks>
    /// By default, the badge is positioned so that its top-right corner aligns with the panel's top-right
    /// corner. However, if the badge is larger than <see cref="MaxBadgeInset" />, it will stretch out of the
    /// panel's bounds to avoid covering too much content of the panel.
    /// </remarks>
    public Size MaxBadgeInset
    {
        get => GetValue(MaxBadgeInsetProperty);
        set => SetValue(MaxBadgeInsetProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double width = 0;
        double height = 0;

        int badgedCount = GetBadgedChildrenCount();
        for (int i = 0; i < badgedCount; i++)
        {
            var child = Children[i];
            child.Measure(availableSize);
            var childSize = child.DesiredSize;

            if (childSize.Width > width)
                width = childSize.Width;

            if (childSize.Height > height)
                height = childSize.Height;
        }

        if (Children.Count > 1)
        {
            var badge = Children[^1];
            badge.Measure(Size.Infinity);
        }

        return new Size(width, height);
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
        var arrangeRect = new Rect(finalSize);

        int badgedCount = GetBadgedChildrenCount();
        for (int i = 0; i < badgedCount; i++)
        {
            var child = Children[i];
            child.Arrange(arrangeRect);
        }

        if (Children.Count > 1)
        {
            var badge = Children[^1];
            var desiredSize = badge.DesiredSize;
            var pos = arrangeRect.TopRight;
            double x = pos.X - Math.Min(MaxBadgeInset.Width, desiredSize.Width);
            double y = pos.Y + Math.Min(0, MaxBadgeInset.Height - desiredSize.Height);
            badge.Arrange(new(new(x, y), desiredSize));
        }

        return finalSize;
    }
    int GetBadgedChildrenCount()
    {
        int count = Children.Count;
        return count switch
        {
            0 => 0,
            1 => 1,
            _ => count - 1,
        };
    }
}

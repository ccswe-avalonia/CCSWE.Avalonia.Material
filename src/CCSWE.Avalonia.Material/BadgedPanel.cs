using Avalonia;
using Avalonia.Controls;
using JetBrains.Annotations;

namespace CCSWE.Avalonia.Material;

/// <summary>
/// A panel that arranges its children like <see cref="Panel" />, except the last child (the "badge", usually a
/// <see cref="Badge" />), which is positioned at the top-right corner instead.
/// </summary>
/// <remarks>
/// <para>With a single child, it behaves like <see cref="Panel" />.</para>
/// <para>The badge doesn't count toward the panel's size and can extend past the panel's edges (see
/// <see cref="MaxBadgeInset" />), so an ancestor that clips its content (<c>ClipToBounds</c>) can cut it off.</para>
/// </remarks>
[PublicAPI]
public sealed class BadgedPanel : Panel
{
    /// <summary>Defines the <see cref="MaxBadgeInset" /> property.</summary>
    public static readonly StyledProperty<Size> MaxBadgeInsetProperty =
        AvaloniaProperty.Register<BadgedPanel, Size>(nameof(MaxBadgeInset), new Size(12, 14));

    /// <summary>
    /// The maximum distance from the panel's top-right corner to the badge's bottom-left corner. Default 12×14.
    /// </summary>
    /// <remarks>
    /// The badge's top-right corner aligns with the panel's top-right corner. A badge larger than
    /// <see cref="MaxBadgeInset" /> extends out of the panel's bounds instead, so it doesn't cover too much of the
    /// panel's content.
    /// </remarks>
    public Size MaxBadgeInset
    {
        get => GetValue(MaxBadgeInsetProperty);
        set => SetValue(MaxBadgeInsetProperty, value);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var arrangeRect = new Rect(finalSize);

        var badgedCount = GetBadgedChildrenCount();
        for (var i = 0; i < badgedCount; i++)
        {
            Children[i].Arrange(arrangeRect);
        }

        if (Children.Count > 1)
        {
            var badge = Children[^1];
            var desiredSize = badge.DesiredSize;
            var position = arrangeRect.TopRight;
            var x = position.X - Math.Min(MaxBadgeInset.Width, desiredSize.Width);
            var y = position.Y + Math.Min(0, MaxBadgeInset.Height - desiredSize.Height);
            badge.Arrange(new Rect(new Point(x, y), desiredSize));
        }

        return finalSize;
    }

    // Every child but the last is content; a single child is content.
    private int GetBadgedChildrenCount() => Children.Count > 1 ? Children.Count - 1 : Children.Count;

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = 0d;
        var height = 0d;

        var badgedCount = GetBadgedChildrenCount();
        for (var i = 0; i < badgedCount; i++)
        {
            var child = Children[i];
            child.Measure(availableSize);
            width = Math.Max(width, child.DesiredSize.Width);
            height = Math.Max(height, child.DesiredSize.Height);
        }

        if (Children.Count > 1)
        {
            Children[^1].Measure(Size.Infinity);
        }

        return new Size(width, height);
    }
}

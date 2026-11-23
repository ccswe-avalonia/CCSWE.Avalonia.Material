using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;

namespace CCSWE.Avalonia.Material;

/// <summary>
/// A badge that shows notifications, counts, or status information on navigation items and icons.
/// </summary>
[PseudoClasses([":empty"])]
public sealed class Badge : ContentControl
{
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        UpdatePseudoClasses();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ContentProperty)
        {
            UpdatePseudoClasses();
        }
    }

    void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":empty", Content == null || (Content is string s && string.IsNullOrEmpty(s)));
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using JetBrains.Annotations;

namespace CCSWE.Avalonia.Material;

/// <summary>
/// A Material 3 badge: shows notifications, counts, or status information on navigation items and icons. With no
/// content (<see langword="null"/> or an empty string) it is a small dot; with content it is a pill that sizes to
/// its label.
/// </summary>
/// <remarks>
/// The badge uses <c>ErrorFill</c> / <c>OnErrorFill</c> by default. For another color set
/// <see cref="Avalonia.Controls.Primitives.TemplatedControl.Background"/> and
/// <see cref="Avalonia.Controls.Primitives.TemplatedControl.Foreground"/> to a fill pair such as <c>PrimaryFill</c> /
/// <c>OnPrimaryFill</c>. To place a badge on an icon, make it the last child of a <see cref="BadgedPanel"/>.
/// </remarks>
[PublicAPI]
[PseudoClasses(":empty")]
public sealed class Badge : ContentControl
{
    public Badge()
    {
        // Avalonia's ContentControl only sets :empty (for null) from 12.1.2; this library supports 12.0.0+.
        UpdatePseudoClasses();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        // The base updates :empty for null only; update after it so an empty string also counts as empty.
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
        {
            UpdatePseudoClasses();
        }
    }

    private void UpdatePseudoClasses() => PseudoClasses.Set(":empty", Content is null or "");
}

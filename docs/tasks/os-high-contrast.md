# Follow the OS high-contrast setting

`MaterialTheme.DarkHighContrast` / `LightHighContrast` are selected only by the app (`RequestedThemeVariant`). Avalonia maps the platform theme to Light/Dark only, so an OS high-contrast setting isn't followed.

1. Find what Avalonia 12 exposes for the platform's contrast preference (`IPlatformSettings` / `PlatformColorValues.ContrastPreference`) on Windows, macOS and Linux.
2. If it is available, add an opt-in (e.g. a `MaterialTheme` property) that picks the high-contrast variant matching the platform's Dark/Light choice, and tracks changes.
3. Document it in the README and package README; add a Demo toggle.

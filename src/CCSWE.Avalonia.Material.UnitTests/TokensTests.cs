using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Media;
using Avalonia.Styling;
using CCSWE.Avalonia.Material;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

/// <summary>
/// Checks the generated <c>Tokens.axaml</c> as the applied theme resolves it: each high-contrast
/// dictionary is complete and reached (a missing or mis-keyed one would silently fall back to
/// Dark/Light through <see cref="ThemeVariant.InheritVariant"/>), and every role's
/// <c>&lt;Role&gt;Color</c> and <c>&lt;Role&gt;</c> brush agree (nothing merged later shadows a token key).
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class TokensTests
{
    protected static IEnumerable<ThemeVariant> Variants =>
    [
        ThemeVariant.Dark,
        ThemeVariant.Light,
        MaterialTheme.DarkHighContrast,
        MaterialTheme.LightHighContrast,
    ];

    protected static IEnumerable<ThemeVariant> HighContrastVariants =>
    [
        MaterialTheme.DarkHighContrast,
        MaterialTheme.LightHighContrast,
    ];

    protected static IResourceDictionary LoadTokens()
    {
        var theme = Application.Current!.Styles.OfType<MaterialTheme>().Single();
        // Compiled XAML merges Tokens.axaml as a plain ResourceDictionary: find it by a key only it defines.
        return ((IResourceDictionary)theme.Resources).MergedDictionaries
            .OfType<IResourceDictionary>()
            .Single(d => d.ThemeDictionaries.TryGetValue(ThemeVariant.Dark, out var dark)
                         && ((IResourceDictionary)dark).ContainsKey("SurfaceContainerHighestColor"));
    }

    protected static IResourceDictionary ThemeDictionary(ThemeVariant variant) =>
        (IResourceDictionary)LoadTokens().ThemeDictionaries[variant];

    public class When_The_Theme_Is_Applied : TokensTests
    {
        [AvaloniaTest]
        public void It_gives_each_high_contrast_variant_the_keys_of_the_variant_it_inherits()
        {
            foreach (var variant in HighContrastVariants)
            {
                var keys = ThemeDictionary(variant).Keys.OfType<string>().Order(StringComparer.Ordinal);
                var inherited = ThemeDictionary(variant.InheritVariant!).Keys.OfType<string>().Order(StringComparer.Ordinal);

                Assert.That(keys, Is.EqualTo(inherited), $"{variant} keys differ from {variant.InheritVariant}");
            }
        }

        [AvaloniaTest]
        public void It_resolves_each_high_contrast_key_from_its_own_dictionary()
        {
            var app = Application.Current!;
            var misses = new List<string>();

            foreach (var variant in HighContrastVariants)
            {
                var dictionary = ThemeDictionary(variant);

                foreach (var key in dictionary.Keys.OfType<string>())
                {
                    dictionary.TryGetResource(key, null, out var expected);
                    app.TryGetResource(key, variant, out var resolved);

                    if (!Equals(resolved, expected))
                    {
                        misses.Add($"{key} ({variant})");
                    }
                }
            }

            Assert.That(misses, Is.Empty, "Keys not resolved from their high-contrast dictionary:" + Environment.NewLine + string.Join(Environment.NewLine, misses));
        }

        [AvaloniaTest]
        public void It_resolves_each_role_color_to_its_brush_color()
        {
            var app = Application.Current!;
            var mismatches = new List<string>();

            foreach (var variant in Variants)
            {
                var dictionary = ThemeDictionary(variant);
                var roles = dictionary.Keys.OfType<string>()
                    .Where(key => dictionary.TryGetResource($"{key}Color", null, out var color) && color is Color);

                foreach (var role in roles)
                {
                    app.TryGetResource($"{role}Color", variant, out var color);
                    app.TryGetResource(role, variant, out var brush);

                    if (brush is not ISolidColorBrush solid || color is not Color resolved || solid.Color != resolved)
                    {
                        mismatches.Add($"{role} ({variant}): {role}Color = {color}, {role} brush = {(brush as ISolidColorBrush)?.Color}");
                    }
                }
            }

            Assert.That(mismatches, Is.Empty, "Role colors that don't match their brush:" + Environment.NewLine + string.Join(Environment.NewLine, mismatches));
        }
    }
}

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
/// Checks the generated <c>Tokens.axaml</c> as the applied theme resolves it: every role's
/// <c>&lt;Role&gt;Color</c> and <c>&lt;Role&gt;</c> brush agree in every variant (nothing merged later
/// shadows a token key).
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class TokensTests
{
    protected static IEnumerable<ThemeVariant> Variants =>
    [
        ThemeVariant.Dark,
        ThemeVariant.Light,
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

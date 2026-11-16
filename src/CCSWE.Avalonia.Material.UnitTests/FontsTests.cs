using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Headless.NUnit;
using Avalonia.Platform;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class FontsTests
{
    public class When_The_Library_Is_Loaded : FontsTests
    {
        [AvaloniaTest]
        public void It_embeds_a_font_file_for_every_family()
        {
            var files = AssetLoader.GetAssets(new Uri("avares://CCSWE.Avalonia.Material/Assets/Fonts/"), null)
                .Select(uri => Uri.UnescapeDataString(uri.AbsolutePath.Split('/').Last()))
                .ToArray();

            Assert.That(files, Is.SupersetOf(new[]
            {
                "DMMono-Medium.ttf",
                "DMMono-Regular.ttf",
                "DMSans[opsz,wght].ttf",
                "JetBrainsMono[wght].ttf",
                "PlusJakartaSans[wght].ttf",
            }));
        }
    }
}

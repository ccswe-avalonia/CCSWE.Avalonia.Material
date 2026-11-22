using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CCSWE.Avalonia.Material.UnitTests;

/// <summary>
/// The library's raw .axaml, embedded into this assembly under <c>ThemeXaml/</c> (see the csproj).
/// </summary>
internal static class EmbeddedXaml
{
    /// <summary>
    /// Returns each embedded .axaml as (path relative to the library root, using <c>/</c>, file text).
    /// </summary>
    public static IEnumerable<(string Path, string Text)> ReadAll()
    {
        var assembly = Assembly.GetExecutingAssembly();

        foreach (var name in assembly.GetManifestResourceNames().Where(n => n.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase)))
        {
            using var stream = assembly.GetManifestResourceStream(name);
            if (stream is null)
            {
                continue;
            }

            using var reader = new StreamReader(stream);
            var path = name.Replace('\\', '/');
            const string prefix = "ThemeXaml/";

            yield return (path.StartsWith(prefix, StringComparison.Ordinal) ? path[prefix.Length..] : path, reader.ReadToEnd());
        }
    }

    /// <summary>
    /// Returns the text of the embedded .axaml at <paramref name="path"/> (relative to the library root).
    /// </summary>
    public static string Read(string path) =>
        ReadAll().Single(file => string.Equals(file.Path, path, StringComparison.Ordinal)).Text;
}

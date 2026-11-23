using System.Diagnostics.CodeAnalysis;
using Avalonia.Headless.NUnit;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class BadgeTests
{
    public class When_Its_Content_Is_Null : BadgeTests
    {
        [AvaloniaTest]
        public void It_has_the_empty_pseudo_class()
        {
            var sut = new Badge();

            Assert.That(sut.Classes, Does.Contain(":empty"));
        }
    }

    public class When_Its_Content_Is_Empty_String : BadgeTests
    {
        [AvaloniaTest]
        public void It_has_the_empty_pseudo_class()
        {
            var sut = new Badge { Content = "" };

            Assert.That(sut.Classes, Does.Contain(":empty"));
        }
    }

    public class When_Its_Content_Is_Not_Null_Or_Empty : BadgeTests
    {
        [AvaloniaTest]
        public void It_does_not_have_the_empty_pseudo_class()
        {
            var sut = new Badge { Content = "1" };

            Assert.That(sut.Classes, Does.Not.Contain(":empty"));
        }
    }

    public class When_Its_Content_Changes_To_Empty_String : BadgeTests
    {
        // Avalonia's ContentControl clears :empty for any non-null content; Badge must update after it.
        [AvaloniaTest]
        public void It_has_the_empty_pseudo_class()
        {
            var sut = new Badge { Content = "1" };

            sut.Content = "";

            Assert.That(sut.Classes, Does.Contain(":empty"));
        }
    }
}

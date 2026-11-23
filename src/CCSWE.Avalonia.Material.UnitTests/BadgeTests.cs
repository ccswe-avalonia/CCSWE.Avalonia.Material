using Avalonia.Headless.NUnit;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class BadgeTests
{
    public class When_Its_Content_Is_Null : CardTests
    {
        [AvaloniaTest]
        public void It_has_the_empty_pseudo_class()
        {
            var sut = new Badge();

            Assert.That(sut.Classes, Does.Contain(":empty"));
        }
    }

    public class When_Its_Content_Is_Empty_String : CardTests
    {
        [AvaloniaTest]
        public void It_has_the_empty_pseudo_class()
        {
            var sut = new Badge() { Content = "" };

            Assert.That(sut.Classes, Does.Contain(":empty"));
        }
    }

    public class When_Its_Content_Is_Not_Null_Or_Empty : CardTests
    {
        [AvaloniaTest]
        public void It_does_not_have_the_empty_pseudo_class()
        {
            var sut = new Badge() { Content = "1" };

            Assert.That(sut.Classes, Does.Not.Contain(":empty"));
        }
    }
}

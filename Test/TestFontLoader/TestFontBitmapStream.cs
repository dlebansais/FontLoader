namespace TestFontLoader;

using FontLoader;
using NUnit.Framework;
using System;
using System.IO;

[TestFixture]
public class TestFontBitmapStream
{
    [Test, Order(1)]
    public void BasicTest()
    {
        using FontBitmapStream TestObject = new FontBitmapStream(typeof(Dummy).Assembly, $"{typeof(Dummy).Namespace}.FullFontResources.Test._75.black.normal.png", 75);
        using Stream TestStream = TestObject.LoadStream();

        Assert.NotNull(TestStream);
    }

    [Test]
    public void BasicPropertiesTest()
    {
        var FontAssembly = typeof(Dummy).Assembly;
        var FontSize = 75;
        var ResourceName = $"{typeof(Dummy).Namespace}.FullFontResources.Test._{FontSize}.black.normal.png";
        using FontBitmapStream TestObject = new FontBitmapStream(FontAssembly, ResourceName, FontSize);

        Assert.AreEqual(FontAssembly, TestObject.FontAssembly);
        Assert.AreEqual(ResourceName, TestObject.ResourceName);
        Assert.AreEqual(FontSize, TestObject.FontSize);
    }

    [Test]
    public void IsLoadedTest()
    {
        using FontBitmapStream TestObject = new FontBitmapStream(typeof(Dummy).Assembly, $"{typeof(Dummy).Namespace}.FullFontResources.Test._75.black.normal.png", 75);
        using Stream TestStream = TestObject.LoadStream();

        Assert.IsTrue(TestObject.IsLoaded);
    }

    [Test]
    public void SingleUsingTest()
    {
        using (FontBitmapStream TestObject = new FontBitmapStream(typeof(Dummy).Assembly, $"{typeof(Dummy).Namespace}.FullFontResources.Test._75.black.normal.png", 75))
        {
            using (Stream TestStream = TestObject.LoadStream())
            {
            }

            Stream OtherStream = TestObject.LoadStream();

            Assert.Throws<ObjectDisposedException>(() =>
            {
                OtherStream.Seek(0, SeekOrigin.Begin);
            });
        }
    }

}

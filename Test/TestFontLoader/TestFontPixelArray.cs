namespace TestFontScanner;

using FontLoader;
using NUnit.Framework;
using System.Drawing;
using System.IO;
using System.Reflection;

[TestFixture]
public class TestFontPixelArray
{
    [Test]
    public void BasicTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();
        Stream TestBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestFontPixelArray).Namespace}.FakeResource.png");
        Bitmap TestBitmap = new Bitmap(TestBitmapStream);

        FontPixelArray TestPixelArray = FontPixelArray.FromBitmap(TestBitmap);
        TestPixelArray.DebugPrint();

        FontPixelArray TestClipped = TestPixelArray.Clipped();
        Assert.IsTrue(TestClipped.IsClipped);
    }
}

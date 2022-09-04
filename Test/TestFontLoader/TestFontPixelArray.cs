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
        Stream TestBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestFontPixelArray).Namespace}.Black.png");
        Bitmap TestBitmap = new Bitmap(TestBitmapStream);
        FontPixelArray TestPixelArray = FontPixelArray.FromBitmap(TestBitmap);
    }

    [Test]
    public void ClippedTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();

        Stream BlackBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestFontPixelArray).Namespace}.Black.png");
        Bitmap BlackBitmap = new Bitmap(BlackBitmapStream);
        FontPixelArray BlackPixelArray = FontPixelArray.FromBitmap(BlackBitmap);
        BlackPixelArray.DebugPrint();
        FontPixelArray BlackClipped = BlackPixelArray.Clipped();
        Assert.IsTrue(BlackClipped.IsClipped);

        Stream WhiteBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestFontPixelArray).Namespace}.White.png");
        Bitmap WhiteBitmap = new Bitmap(WhiteBitmapStream);
        FontPixelArray WhitePixelArray = FontPixelArray.FromBitmap(WhiteBitmap);
        WhitePixelArray.DebugPrint();
        FontPixelArray WhiteClipped = WhitePixelArray.Clipped();
        Assert.AreEqual(FontPixelArray.Empty, WhiteClipped);
    }
}

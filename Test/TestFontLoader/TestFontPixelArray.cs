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
    public void GetSetTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();
        Stream TestBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestFontPixelArray).Namespace}.Black.png");
        Bitmap TestBitmap = new Bitmap(TestBitmapStream);
        FontPixelArray TestPixelArray = FontPixelArray.FromBitmap(TestBitmap);

        byte Pixel = TestPixelArray.GetPixel(0, 0);
        Assert.AreEqual(0, Pixel);
        Assert.IsFalse(TestPixelArray.IsColored(0, 0, out byte Color));
        Assert.AreEqual(0, Color);

        TestPixelArray.SetPixel(0, 0, 0x80);
        Pixel = TestPixelArray.GetPixel(0, 0);
        Assert.AreEqual(0x80, Pixel);

        TestPixelArray.ClearPixel(0, 0);
        Pixel = TestPixelArray.GetPixel(0, 0);
        Assert.AreEqual(0xFF, Pixel);
        Assert.IsTrue(TestPixelArray.IsWhite(0, 0));

        Assert.IsFalse(TestPixelArray.IsWhiteColumn(0));
        TestPixelArray.SetWhiteColumn(0, true);
        Assert.IsTrue(TestPixelArray.IsWhiteColumn(0));

        int ColoredCount = TestPixelArray.GetColoredCountColumn(0);
        Assert.AreEqual(0, ColoredCount);
        TestPixelArray.SetColoredCountColumn(0, 1);
        ColoredCount = TestPixelArray.GetColoredCountColumn(0);
        Assert.AreEqual(1, ColoredCount);
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

        Stream MixedBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestFontPixelArray).Namespace}.Mixed.png");
        Bitmap MixedBitmap = new Bitmap(MixedBitmapStream);
        FontPixelArray MixedPixelArray = FontPixelArray.FromBitmap(MixedBitmap);
        MixedPixelArray.DebugPrint();
        Assert.IsFalse(MixedPixelArray.IsClipped);
        FontPixelArray MixedClipped = MixedPixelArray.Clipped();
        Assert.IsTrue(MixedClipped.IsClipped);

        Stream WhiteBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestFontPixelArray).Namespace}.White.png");
        Bitmap WhiteBitmap = new Bitmap(WhiteBitmapStream);
        FontPixelArray WhitePixelArray = FontPixelArray.FromBitmap(WhiteBitmap);
        WhitePixelArray.DebugPrint();
        FontPixelArray WhiteClipped = WhitePixelArray.Clipped();
        Assert.AreEqual(FontPixelArray.Empty, WhiteClipped);
    }

    [Test]
    public void GetLeftSideTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();

        Stream BlackBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestFontPixelArray).Namespace}.Mixed.png");
        Bitmap BlackBitmap = new Bitmap(BlackBitmapStream);
        FontPixelArray BlackPixelArray = FontPixelArray.FromBitmap(BlackBitmap);
        FontPixelArray LeftSide = BlackPixelArray.GetLeftSide(1);
        Assert.IsTrue(LeftSide.IsWhiteColumn(0));
    }

    [Test]
    public void GetRightSideTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();

        Stream BlackBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestFontPixelArray).Namespace}.Mixed.png");
        Bitmap BlackBitmap = new Bitmap(BlackBitmapStream);
        FontPixelArray BlackPixelArray = FontPixelArray.FromBitmap(BlackBitmap);
        FontPixelArray RightSide = BlackPixelArray.GetRightSide(1);
        Assert.IsTrue(RightSide.IsWhiteColumn(0));
    }
}

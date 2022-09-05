namespace TestFontLoader;

using FontLoader;
using NUnit.Framework;
using System.Drawing;
using System.IO;
using System.Reflection;

[TestFixture]
public class TestPixelArray
{
    [Test]
    public void GetSetTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();
        Stream TestBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Black.png");
        Bitmap TestBitmap = new Bitmap(TestBitmapStream);
        PixelArray TestPixelArray = PixelArray.FromBitmap(TestBitmap);

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

        Rectangle Rect = new(0, 0, TestBitmap.Width, TestBitmap.Height);

        PixelArray TestPixelArray2 = PixelArray.FromBitmap(TestBitmap, Rect);
        Assert.AreEqual(TestPixelArray2.Width, TestPixelArray.Width);
        Assert.AreEqual(TestPixelArray2.Height, TestPixelArray.Height);
        Assert.AreEqual(TestPixelArray2.Baseline, TestPixelArray.Baseline);

        PixelArray TestPixelArray3 = PixelArray.FromBitmap(TestBitmap, 1);
        Assert.AreEqual(TestPixelArray3.Width, TestPixelArray.Width);
        Assert.AreEqual(TestPixelArray3.Height, TestPixelArray.Height);
        Assert.AreEqual(TestPixelArray3.Baseline, TestPixelArray.Baseline);

        PixelArray TestPixelArray4 = PixelArray.FromBitmap(TestBitmap, Rect, 1);
        Assert.AreEqual(TestPixelArray4.Width, TestPixelArray.Width);
        Assert.AreEqual(TestPixelArray4.Height, TestPixelArray.Height);
        Assert.AreEqual(TestPixelArray4.Baseline, TestPixelArray.Baseline);
    }

    [Test]
    public void ClippedTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();

        Stream BlackBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Black.png");
        Bitmap BlackBitmap = new Bitmap(BlackBitmapStream);
        PixelArray BlackPixelArray = PixelArray.FromBitmap(BlackBitmap);
        BlackPixelArray.DebugPrint();
        PixelArray BlackClipped = BlackPixelArray.Clipped();
        Assert.IsTrue(BlackClipped.IsClipped);

        Stream MixedBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Mixed.png");
        Bitmap MixedBitmap = new Bitmap(MixedBitmapStream);
        PixelArray MixedPixelArray = PixelArray.FromBitmap(MixedBitmap);
        MixedPixelArray.DebugPrint();
        Assert.IsFalse(MixedPixelArray.IsClipped);
        PixelArray MixedClipped = MixedPixelArray.Clipped();
        Assert.IsTrue(MixedClipped.IsClipped);

        Stream WhiteBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.White.png");
        Bitmap WhiteBitmap = new Bitmap(WhiteBitmapStream);
        PixelArray WhitePixelArray = PixelArray.FromBitmap(WhiteBitmap);
        WhitePixelArray.DebugPrint();
        PixelArray WhiteClipped = WhitePixelArray.Clipped();
        Assert.AreEqual(PixelArray.Empty, WhiteClipped);
    }

    [Test]
    public void GetLeftSideTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();

        Stream BlackBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Mixed.png");
        Bitmap BlackBitmap = new Bitmap(BlackBitmapStream);
        PixelArray BlackPixelArray = PixelArray.FromBitmap(BlackBitmap);
        PixelArray LeftSide = BlackPixelArray.GetLeftSide(1);
        Assert.IsTrue(LeftSide.IsWhiteColumn(0));
    }

    [Test]
    public void GetRightSideTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();

        Stream BlackBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Mixed.png");
        Bitmap BlackBitmap = new Bitmap(BlackBitmapStream);
        PixelArray BlackPixelArray = PixelArray.FromBitmap(BlackBitmap);
        PixelArray RightSide = BlackPixelArray.GetRightSide(1);
        Assert.IsTrue(RightSide.IsWhiteColumn(0));
    }
}

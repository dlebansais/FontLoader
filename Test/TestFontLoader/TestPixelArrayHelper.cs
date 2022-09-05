namespace TestFontLoader;

using FontLoader;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

[TestFixture]
public class TestPixelArrayHelper
{
    [Test]
    public void IsMatchTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();
        Stream TestBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Mixed.png");
        Bitmap TestBitmap = new Bitmap(TestBitmapStream);
        PixelArray TestPixelArray = PixelArray.FromBitmap(TestBitmap);
        TestPixelArray = TestPixelArray.Clipped();

        bool IsMatch;

        IsMatch = PixelArrayHelper.IsMatch(TestPixelArray, TestPixelArray, 0);
        Assert.IsTrue(IsMatch);

        IsMatch = PixelArrayHelper.IsMatch(TestPixelArray, TestPixelArray, 1);
        Assert.IsFalse(IsMatch);
    }

    [Test]
    public void IsCompatibleTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();

        Stream BlackBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Black.png");
        Bitmap BlackBitmap = new Bitmap(BlackBitmapStream);
        PixelArray BlackPixelArray = PixelArray.FromBitmap(BlackBitmap);

        Stream MixedBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Mixed.png");
        Bitmap MixedBitmap = new Bitmap(MixedBitmapStream);
        PixelArray MixedPixelArray = PixelArray.FromBitmap(MixedBitmap);

        bool IsCompatible;

        IsCompatible = PixelArrayHelper.IsCompatible(BlackPixelArray, BlackPixelArray, BlackPixelArray.Width);
        Assert.IsTrue(IsCompatible);

        IsCompatible = PixelArrayHelper.IsCompatible(BlackPixelArray, MixedPixelArray, Math.Max(BlackPixelArray.Width, MixedPixelArray.Width));
        Assert.IsFalse(IsCompatible);
    }

    [Test]
    public void MergeTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();
        Stream TestBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Mixed.png");
        Bitmap TestBitmap = new Bitmap(TestBitmapStream);
        PixelArray TestPixelArray = PixelArray.FromBitmap(TestBitmap);
        TestPixelArray = TestPixelArray.Clipped();

        PixelArray MergedPixelArray;

        MergedPixelArray = PixelArrayHelper.Merge(TestPixelArray, TestPixelArray, 0);
        Assert.AreEqual(TestPixelArray.Width * 2, MergedPixelArray.Width);
        Assert.AreEqual(TestPixelArray.Height, MergedPixelArray.Height);

        MergedPixelArray = PixelArrayHelper.Merge(TestPixelArray, TestPixelArray, 0, TestPixelArray.Width * 2);
        Assert.AreEqual(TestPixelArray.Width * 2, MergedPixelArray.Width);
        Assert.AreEqual(TestPixelArray.Height, MergedPixelArray.Height);

        MergedPixelArray = PixelArrayHelper.Merge(TestPixelArray, TestPixelArray, 1);
        Assert.AreEqual(TestPixelArray.Width * 2 - 1, MergedPixelArray.Width);
        Assert.AreEqual(TestPixelArray.Height, MergedPixelArray.Height);

        MergedPixelArray = PixelArrayHelper.Merge(TestPixelArray, TestPixelArray, 0, TestPixelArray.Width  + 1);
        Assert.AreEqual(TestPixelArray.Width + 1, MergedPixelArray.Width);
        Assert.AreEqual(TestPixelArray.Height, MergedPixelArray.Height);
    }

    [Test]
    public void CutTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();
        Stream TestBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Mixed.png");
        Bitmap TestBitmap = new Bitmap(TestBitmapStream);
        PixelArray TestPixelArray = PixelArray.FromBitmap(TestBitmap);

        PixelArray CutPixelArray;

        CutPixelArray = PixelArrayHelper.Cut(TestPixelArray, 0);
        Assert.AreEqual(CutPixelArray.Width, TestPixelArray.Width);
        Assert.AreEqual(CutPixelArray.Height, TestPixelArray.Height);

        CutPixelArray = PixelArrayHelper.Cut(TestPixelArray, 1);
        Assert.AreEqual(CutPixelArray.Width, TestPixelArray.Width - 1);
        Assert.AreEqual(CutPixelArray.Height, TestPixelArray.Height);
    }

    [Test]
    public void EnlargeTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();

        Stream BlackBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Black.png");
        Bitmap BlackBitmap = new Bitmap(BlackBitmapStream);
        PixelArray BlackPixelArray = PixelArray.FromBitmap(BlackBitmap);

        Stream LargeBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Large.png");
        Bitmap LargeBitmap = new Bitmap(LargeBitmapStream);
        PixelArray LargePixelArray = PixelArray.FromBitmap(LargeBitmap);

        PixelArray EnlargedPixelArray;

        EnlargedPixelArray = PixelArrayHelper.Enlarge(BlackPixelArray, LargePixelArray);
        Assert.AreEqual(EnlargedPixelArray.Width, LargePixelArray.Width);
        Assert.AreEqual(EnlargedPixelArray.Height, LargePixelArray.Height);

        EnlargedPixelArray = PixelArrayHelper.Enlarge(LargePixelArray, BlackPixelArray);
        Assert.AreEqual(EnlargedPixelArray.Width, LargePixelArray.Width);
        Assert.AreEqual(EnlargedPixelArray.Height, LargePixelArray.Height);
    }
}

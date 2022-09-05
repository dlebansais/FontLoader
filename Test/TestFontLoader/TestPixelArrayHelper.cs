﻿namespace TestFontLoader;

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

        Stream MixedBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Mixed.png");
        Bitmap MixedBitmap = new Bitmap(MixedBitmapStream);
        PixelArray MixedPixelArray = PixelArray.FromBitmap(MixedBitmap);
        MixedPixelArray = MixedPixelArray.Clipped();

        bool IsMatch;

        IsMatch = PixelArrayHelper.IsMatch(MixedPixelArray, MixedPixelArray, 0);
        Assert.IsTrue(IsMatch);

        IsMatch = PixelArrayHelper.IsMatch(MixedPixelArray, MixedPixelArray, 1);
        Assert.IsFalse(IsMatch);

        Stream LongBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Long.png");
        Bitmap LongBitmap = new Bitmap(LongBitmapStream);
        PixelArray LongPixelArray = PixelArray.FromBitmap(LongBitmap);
        LongPixelArray = LongPixelArray.Clipped();

        Stream LargeBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Large.png");
        Bitmap LargeBitmap = new Bitmap(LargeBitmapStream);
        PixelArray LargePixelArray = PixelArray.FromBitmap(LargeBitmap);
        LargePixelArray = LargePixelArray.Clipped();

        Stream BlackBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Black.png");
        Bitmap BlackBitmap = new Bitmap(BlackBitmapStream);
        PixelArray BlackPixelArray = PixelArray.FromBitmap(BlackBitmap);
        BlackPixelArray = BlackPixelArray.Clipped();

        IsMatch = PixelArrayHelper.IsMatch(MixedPixelArray, LongPixelArray, 0);
        Assert.IsFalse(IsMatch);

        IsMatch = PixelArrayHelper.IsMatch(MixedPixelArray, LargePixelArray, 0);
        Assert.IsFalse(IsMatch);

        IsMatch = PixelArrayHelper.IsMatch(MixedPixelArray, BlackPixelArray, 0);
        Assert.IsFalse(IsMatch);

        PixelArray MixedPixelArray2 = PixelArray.FromBitmap(MixedBitmap);
        MixedPixelArray2 = MixedPixelArray2.Clipped();
        for (int x = 0; x < MixedPixelArray2.Width; x++)
            for (int y = 0; y < MixedPixelArray2.Height; y++)
            {
                byte Pixel = MixedPixelArray.GetPixel(x, y);
                if (Pixel == 0)
                    MixedPixelArray2.SetPixel(x, y, 1);
                else
                    MixedPixelArray2.SetPixel(x, y, (byte)(Pixel - 1));
            }

        IsMatch = PixelArrayHelper.IsMatch(MixedPixelArray, MixedPixelArray2, 0);
        Assert.IsFalse(IsMatch);

        PixelArray MixedPixelArray3 = PixelArray.FromBitmap(MixedBitmap);
        MixedPixelArray3 = MixedPixelArray3.Clipped();
        for (int x = 0; x < MixedPixelArray3.Width; x++)
            for (int y = 0; y < MixedPixelArray3.Height; y++)
            {
                byte Pixel = MixedPixelArray.GetPixel(x, y);
                if (Pixel == 0)
                    MixedPixelArray3.SetPixel(x, y, 6);
                else
                    MixedPixelArray3.SetPixel(x, y, (byte)(Pixel - 6));
            }

        IsMatch = PixelArrayHelper.IsMatch(MixedPixelArray, MixedPixelArray3, 0);
        Assert.IsFalse(IsMatch);
    }

    [Test]
    public void IsPixelToPixelMatchTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();

        Stream LongBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Long.png");
        Bitmap LongBitmap = new Bitmap(LongBitmapStream);
        PixelArray LongPixelArray = PixelArray.FromBitmap(LongBitmap);

        Stream LargeBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Large.png");
        Bitmap LargeBitmap = new Bitmap(LargeBitmapStream);
        PixelArray LargePixelArray = PixelArray.FromBitmap(LargeBitmap);

        Stream BlackBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Black.png");
        Bitmap BlackBitmap = new Bitmap(BlackBitmapStream);
        PixelArray BlackPixelArray = PixelArray.FromBitmap(BlackBitmap);

        Stream MixedBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Mixed.png");
        Bitmap MixedBitmap = new Bitmap(MixedBitmapStream);
        PixelArray MixedPixelArray = PixelArray.FromBitmap(MixedBitmap);

        bool IsMatch;

        IsMatch = PixelArrayHelper.IsPixelToPixelMatch(LongPixelArray, MixedPixelArray);
        Assert.IsFalse(IsMatch);

        IsMatch = PixelArrayHelper.IsPixelToPixelMatch(LargePixelArray, MixedPixelArray);
        Assert.IsFalse(IsMatch);

        IsMatch = PixelArrayHelper.IsPixelToPixelMatch(BlackPixelArray, MixedPixelArray);
        Assert.IsFalse(IsMatch);

        IsMatch = PixelArrayHelper.IsPixelToPixelMatch(BlackPixelArray, BlackPixelArray);
        Assert.IsTrue(IsMatch);

        PixelArray BlackPixelArray2 = PixelArray.FromBitmap(BlackBitmap);
        BlackPixelArray2.SetPixel(0, 0, (byte)(BlackPixelArray.GetPixel(0, 0) + 1));

        IsMatch = PixelArrayHelper.IsPixelToPixelMatch(BlackPixelArray, BlackPixelArray2);
        Assert.IsFalse(IsMatch);
    }

    [Test]
    public void IsLeftMatchTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();

        Stream BlackBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Black.png");
        Bitmap BlackBitmap = new Bitmap(BlackBitmapStream);
        PixelArray BlackPixelArray = PixelArray.FromBitmap(BlackBitmap);

        Stream LargeBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Large.png");
        Bitmap LargeBitmap = new Bitmap(LargeBitmapStream);
        PixelArray LargePixelArray = PixelArray.FromBitmap(LargeBitmap);

        Stream LongBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Long.png");
        Bitmap LongBitmap = new Bitmap(LongBitmapStream);
        PixelArray LongPixelArray = PixelArray.FromBitmap(LongBitmap);

        Stream MixedBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Mixed.png");
        Bitmap MixedBitmap = new Bitmap(MixedBitmapStream);
        PixelArray MixedPixelArray = PixelArray.FromBitmap(MixedBitmap);

        bool IsMatch;
        int FirstDiffX;

        IsMatch = PixelArrayHelper.IsLeftMatch(BlackPixelArray, BlackPixelArray, 0, out FirstDiffX);
        Assert.IsTrue(IsMatch);
        Assert.AreEqual(-1, FirstDiffX);

        IsMatch = PixelArrayHelper.IsLeftMatch(BlackPixelArray, MixedPixelArray, 0, out _);
        Assert.IsFalse(IsMatch);

        IsMatch = PixelArrayHelper.IsLeftMatch(LargePixelArray, MixedPixelArray, 0, out _);
        Assert.IsFalse(IsMatch);

        IsMatch = PixelArrayHelper.IsLeftMatch(MixedPixelArray, LargePixelArray, 0, out _);
        Assert.IsFalse(IsMatch);

        IsMatch = PixelArrayHelper.IsLeftMatch(LongPixelArray, MixedPixelArray, 0, out _);
        Assert.IsFalse(IsMatch);

        IsMatch = PixelArrayHelper.IsLeftMatch(MixedPixelArray, LongPixelArray, 0, out _);
        Assert.IsFalse(IsMatch);

        PixelArray MixedPixelArray2 = PixelArray.FromBitmap(MixedBitmap);
        for (int x = 0; x < MixedPixelArray2.Width; x++)
            for (int y = 0; y < MixedPixelArray2.Height; y++)
            {
                byte Pixel = MixedPixelArray.GetPixel(x, y);
                if (Pixel == 0)
                    MixedPixelArray2.SetPixel(x, y, 1);
                else
                    MixedPixelArray2.SetPixel(x, y, (byte)(Pixel - 1));
            }

        IsMatch = PixelArrayHelper.IsLeftMatch(MixedPixelArray, MixedPixelArray2, 0, out _);
        Assert.IsFalse(IsMatch);

        PixelArray MixedPixelArray3 = PixelArray.FromBitmap(MixedBitmap);
        for (int x = 0; x < MixedPixelArray3.Width; x++)
            for (int y = 0; y < MixedPixelArray3.Height; y++)
            {
                byte Pixel = MixedPixelArray.GetPixel(x, y);
                if (Pixel == 0)
                    MixedPixelArray3.SetPixel(x, y, 6);
                else
                    MixedPixelArray3.SetPixel(x, y, (byte)(Pixel - 6));
            }

        IsMatch = PixelArrayHelper.IsLeftMatch(MixedPixelArray, MixedPixelArray3, 0, out _);
        Assert.IsFalse(IsMatch);

        PixelArray LongPixelArray2 = PixelArray.FromBitmap(LongBitmap);
        for (int x = 0; x < MixedPixelArray.Width; x++)
            for (int y = 0; y < MixedPixelArray.Height; y++)
            {
                byte Pixel = MixedPixelArray.GetPixel(x, y);
                LongPixelArray2.SetPixel(x, y, Pixel);
            }

        IsMatch = PixelArrayHelper.IsLeftMatch(LongPixelArray2, MixedPixelArray, 0, out _);
        Assert.IsFalse(IsMatch);
    }

    [Test]
    public void IsLeftDiagonalMatchTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();

        Stream BlackBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Black.png");
        Bitmap BlackBitmap = new Bitmap(BlackBitmapStream);
        PixelArray BlackPixelArray = PixelArray.FromBitmap(BlackBitmap);

        Stream LargeBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Large.png");
        Bitmap LargeBitmap = new Bitmap(LargeBitmapStream);
        PixelArray LargePixelArray = PixelArray.FromBitmap(LargeBitmap);

        Stream LongBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Long.png");
        Bitmap LongBitmap = new Bitmap(LongBitmapStream);
        PixelArray LongPixelArray = PixelArray.FromBitmap(LongBitmap);

        Stream MixedBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Mixed.png");
        Bitmap MixedBitmap = new Bitmap(MixedBitmapStream);
        PixelArray MixedPixelArray = PixelArray.FromBitmap(MixedBitmap);

        bool IsMatch;

        IsMatch = PixelArrayHelper.IsLeftDiagonalMatch(BlackPixelArray, 0, 0, BlackPixelArray, 0);
        Assert.IsTrue(IsMatch);

        IsMatch = PixelArrayHelper.IsLeftDiagonalMatch(MixedPixelArray, 0, 0, MixedPixelArray, 0);
        Assert.IsTrue(IsMatch);

        IsMatch = PixelArrayHelper.IsLeftDiagonalMatch(MixedPixelArray, 0, 0, LargePixelArray, 0);
        Assert.IsTrue(IsMatch);

        IsMatch = PixelArrayHelper.IsLeftDiagonalMatch(MixedPixelArray, 0, 0, LongPixelArray, 0);
        Assert.IsTrue(IsMatch);

        PixelArray LongPixelArray2 = PixelArray.FromBitmap(LongBitmap);
        for (int x = 0; x < MixedPixelArray.Width; x++)
            for (int y = 0; y < MixedPixelArray.Height; y++)
            {
                byte Pixel = MixedPixelArray.GetPixel(x, y);
                LongPixelArray2.SetPixel(x, y, Pixel);
            }

        IsMatch = PixelArrayHelper.IsLeftDiagonalMatch(LongPixelArray2, 0, 0, MixedPixelArray, 0);
        Assert.IsFalse(IsMatch);
    }

    [Test]
    public void IsRightMatchTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();

        Stream BlackBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Black.png");
        Bitmap BlackBitmap = new Bitmap(BlackBitmapStream);
        PixelArray BlackPixelArray = PixelArray.FromBitmap(BlackBitmap);

        Stream LargeBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Large.png");
        Bitmap LargeBitmap = new Bitmap(LargeBitmapStream);
        PixelArray LargePixelArray = PixelArray.FromBitmap(LargeBitmap);

        Stream LongBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Long.png");
        Bitmap LongBitmap = new Bitmap(LongBitmapStream);
        PixelArray LongPixelArray = PixelArray.FromBitmap(LongBitmap);

        Stream MixedBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Mixed.png");
        Bitmap MixedBitmap = new Bitmap(MixedBitmapStream);
        PixelArray MixedPixelArray = PixelArray.FromBitmap(MixedBitmap);

        bool IsMatch;

        IsMatch = PixelArrayHelper.IsRightMatch(BlackPixelArray, BlackPixelArray, 0);
        Assert.IsTrue(IsMatch);

        IsMatch = PixelArrayHelper.IsRightMatch(BlackPixelArray, MixedPixelArray, 0);
        Assert.IsFalse(IsMatch);

        IsMatch = PixelArrayHelper.IsRightMatch(LongPixelArray, MixedPixelArray, 0);
        Assert.IsFalse(IsMatch);

        IsMatch = PixelArrayHelper.IsRightMatch(MixedPixelArray, LongPixelArray, 0);
        Assert.IsFalse(IsMatch);

        PixelArray MixedPixelArray2 = PixelArray.FromBitmap(MixedBitmap);
        for (int x = 0; x < MixedPixelArray2.Width; x++)
            for (int y = 0; y < MixedPixelArray2.Height; y++)
            {
                byte Pixel = MixedPixelArray.GetPixel(x, y);
                if (Pixel == 0)
                    MixedPixelArray2.SetPixel(x, y, 1);
                else
                    MixedPixelArray2.SetPixel(x, y, (byte)(Pixel - 1));
            }

        IsMatch = PixelArrayHelper.IsRightMatch(MixedPixelArray, MixedPixelArray2, 0);
        Assert.IsFalse(IsMatch);

        PixelArray MixedPixelArray3 = PixelArray.FromBitmap(MixedBitmap);
        for (int x = 0; x < MixedPixelArray3.Width; x++)
            for (int y = 0; y < MixedPixelArray3.Height; y++)
            {
                byte Pixel = MixedPixelArray.GetPixel(x, y);
                if (Pixel == 0)
                    MixedPixelArray3.SetPixel(x, y, 6);
                else
                    MixedPixelArray3.SetPixel(x, y, (byte)(Pixel - 6));
            }

        IsMatch = PixelArrayHelper.IsRightMatch(MixedPixelArray, MixedPixelArray3, 0);
        Assert.IsFalse(IsMatch);

        PixelArray LongPixelArray2 = PixelArray.FromBitmap(LongBitmap);
        for (int x = 0; x < MixedPixelArray.Width; x++)
            for (int y = 0; y < MixedPixelArray.Height; y++)
            {
                byte Pixel = MixedPixelArray.GetPixel(x, y);
                LongPixelArray2.SetPixel(x, y, Pixel);
            }

        IsMatch = PixelArrayHelper.IsRightMatch(LongPixelArray2, MixedPixelArray, 0);
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

        PixelArray MergedPixelArray;

        MergedPixelArray = PixelArrayHelper.Merge(TestPixelArray, TestPixelArray, 0);
        Assert.AreEqual(TestPixelArray.Width * 2, MergedPixelArray.Width);
        Assert.AreEqual(TestPixelArray.Height, MergedPixelArray.Height);

        MergedPixelArray = PixelArrayHelper.Merge(TestPixelArray, TestPixelArray, 0, TestPixelArray.Width * 2);
        Assert.AreEqual(TestPixelArray.Width * 2, MergedPixelArray.Width);
        Assert.AreEqual(TestPixelArray.Height, MergedPixelArray.Height);

        MergedPixelArray = PixelArrayHelper.Merge(TestPixelArray, TestPixelArray, TestPixelArray.Width);
        Assert.AreEqual(TestPixelArray.Width, MergedPixelArray.Width);
        Assert.AreEqual(TestPixelArray.Height, MergedPixelArray.Height);

        MergedPixelArray = PixelArrayHelper.Merge(TestPixelArray, TestPixelArray, 0, TestPixelArray.Width  + 1);
        Assert.AreEqual(TestPixelArray.Width + 1, MergedPixelArray.Width);
        Assert.AreEqual(TestPixelArray.Height, MergedPixelArray.Height);

        Stream LargeBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Large.png");
        Bitmap LargeBitmap = new Bitmap(LargeBitmapStream);
        PixelArray LargePixelArray = PixelArray.FromBitmap(LargeBitmap);

        MergedPixelArray = PixelArrayHelper.Merge(TestPixelArray, LargePixelArray, TestPixelArray.Width);
        Assert.AreEqual(LargePixelArray.Width, MergedPixelArray.Width);
        Assert.AreEqual(LargePixelArray.Height, MergedPixelArray.Height);

        MergedPixelArray = PixelArrayHelper.Merge(TestPixelArray, LargePixelArray, 1);
        Assert.AreEqual(TestPixelArray.Width + LargePixelArray.Width - 1, MergedPixelArray.Width);
        Assert.AreEqual(LargePixelArray.Height, MergedPixelArray.Height);
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

        PixelArray LargePixelArray2 = PixelArray.FromBitmap(LargeBitmap, BlackPixelArray.Baseline);

        EnlargedPixelArray = PixelArrayHelper.Enlarge(BlackPixelArray, LargePixelArray2);
        Assert.AreEqual(EnlargedPixelArray.Width, LargePixelArray2.Width);
        Assert.AreEqual(EnlargedPixelArray.Height, LargePixelArray2.Height);
    }
}

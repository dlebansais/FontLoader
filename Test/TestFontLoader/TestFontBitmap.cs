namespace TestFontLoader;

using FontLoader;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

[TestFixture]
public class TestFontBitmap
{
    [Test, Order(1)]
    public void BasicTest()
    {
        using FontBitmapStream TestStream = CreateTestStream();
        using FontBitmap TestObject = new FontBitmap(TestStream);
        using Bitmap TestBitmap = TestObject.LoadBitmap();

        Assert.NotNull(TestBitmap);
    }

    [Test]
    public void BasicPropertiesTest()
    {
        using FontBitmapStream TestStream = CreateTestStream();
        using FontBitmap TestObject = new FontBitmap(TestStream);

        Assert.AreEqual(TestStream, TestObject.SourceStream);
    }

    [Test]
    public void NonBasicPropertiesTest()
    {
        using FontBitmapStream TestStream = CreateTestStream();
        using FontBitmap TestObject = new FontBitmap(TestStream);

        Assert.AreEqual(TestFontSize, TestObject.FontSize);
    }

    [Test]
    public void IsLoadedTest()
    {
        using FontBitmapStream TestStream = CreateTestStream();
        using FontBitmap TestObject = new FontBitmap(TestStream);

        Assert.IsTrue(TestObject.IsLoaded);
    }

    [Test]
    public void SingleUsingTest()
    {
        using (FontBitmapStream TestStream = CreateTestStream())
        {
            using (FontBitmap TestObject = new FontBitmap(TestStream))
            {
                using (Bitmap TestBitmap = TestObject.LoadBitmap())
                {
                }

                Bitmap OtherBitmap = TestObject.LoadBitmap();

                Assert.Throws<ArgumentException>(() =>
                {
                    _ = OtherBitmap.Width;
                });
            }
        }
    }

    [Test]
    public void GetBitmapBytesTest()
    {
        using FontBitmapStream TestStream = CreateTestStream();
        using FontBitmap TestObject = new FontBitmap(TestStream);
        Bitmap TestBitmap = TestObject.LoadBitmap();
        Rectangle Rect = new Rectangle(0, 0, TestBitmap.Width, TestBitmap.Height);
        BitmapData Data = TestBitmap.LockBits(Rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        int TestStride = Math.Abs(Data.Stride);
        int ByteCount = TestStride * TestBitmap.Height;
        byte[] TestArgbValues = new byte[ByteCount];

        Marshal.Copy(Data.Scan0, TestArgbValues, 0, ByteCount);
        TestBitmap.UnlockBits(Data);
        TestObject.GetBitmapBytes(out byte[] ArgbValues, out int Stride, out int Width, out int Height);
        Assert.AreEqual(TestStride, Stride);
        Assert.AreEqual(TestBitmap.Width, Width);
        Assert.AreEqual(TestBitmap.Height, Height);
        Assert.AreEqual(TestArgbValues.Length, ArgbValues.Length);

        for (int i = 0; i < ArgbValues.Length; i++)
            Assert.AreEqual(TestArgbValues[i], ArgbValues[i]);
    }

    private FontBitmapStream CreateTestStream()
    {
        var FontAssembly = typeof(Dummy).Assembly;
        var ResourceName = $"{typeof(Dummy).Namespace}.FullFontResources.Test._{TestFontSize}.black.normal.png";

        return new FontBitmapStream(FontAssembly, ResourceName, TestFontSize);
    }

    private const int TestFontSize = 75;
}

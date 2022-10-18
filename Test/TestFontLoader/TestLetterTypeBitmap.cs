namespace TestFontLoader;

using FontLoader;
using NUnit.Framework;
using System.Runtime.CompilerServices;

[TestFixture]
public class TestLetterTypeBitmap
{
    [Test, Order(1)]
    public void BasicTest()
    {
        using FontBitmapStream TestStream = CreateTestStream();
        using FontBitmap TestFontBitmap = new FontBitmap(TestStream);
        LetterTypeBitmap TestObject = new(0, 0, TestFontBitmap);

        Assert.NotNull(TestObject);
    }

    [Test]
    public void BasicPropertiesTest()
    {
        using FontBitmapStream TestStream = CreateTestStream();
        using FontBitmap TestFontBitmap = new FontBitmap(TestStream);
        LetterTypeBitmap TestObject = new(0, 0, TestFontBitmap);

        Assert.AreEqual(0, TestObject.Columns);
        Assert.AreEqual(0, TestObject.Rows);
        Assert.AreEqual(TestFontSize, TestObject.FontSize);

        int TestCellSize = Font.FontSizeToCellSize(TestFontBitmap.FontSize);

        Assert.AreEqual(TestCellSize, TestObject.CellSize);

        int TestBaseline = (int)(TestCellSize * FontBitmapCollection.DefaultBaselineRatio);

        Assert.AreEqual(TestBaseline, TestObject.Baseline);
    }

    [Test]
    public void GetBitmapBytesTest()
    {
        using FontBitmapStream TestStream = CreateTestStream();
        using FontBitmap TestFontBitmap = new FontBitmap(TestStream);
        LetterTypeBitmap TestObject = new(0, 0, TestFontBitmap);

        TestObject.GetBitmapBytes(out byte[] ArgbValues, out int Stride);
        Assert.NotNull(ArgbValues);
        Assert.Less(0, Stride);
        TestObject.GetBitmapBytes(out byte[] OtherArgbValues, out int OtherStride);
        Assert.AreEqual(OtherArgbValues, ArgbValues);
        Assert.IsFalse(OtherArgbValues == ArgbValues);
        Assert.AreEqual(OtherStride, Stride);
    }

    private FontBitmapStream CreateTestStream()
    {
        var FontAssembly = typeof(Dummy).Assembly;
        var ResourceName = $"{typeof(Dummy).Namespace}.FullFontResources.Test._{TestFontSize}.black.normal.png";

        return new FontBitmapStream(FontAssembly, ResourceName, TestFontSize);
    }

    private const int TestFontSize = 75;
}

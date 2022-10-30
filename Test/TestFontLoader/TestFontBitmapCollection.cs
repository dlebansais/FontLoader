namespace TestFontLoader;

using FontLoader;
using NUnit.Framework;
using System;
using System.Collections.Generic;

[TestFixture]
public class TestFontBitmapCollection
{
    [Test, Order(1)]
    public void BasicTest()
    {
        using FontBitmapStream TestBitmapStream = CreateTestStream();
        Dictionary<LetterType, FontBitmapStream> StreamTable = new()
        {
            { LetterType.Normal, TestBitmapStream },
        };
        FontBitmapCollection TestObject = new FontBitmapCollection(StreamTable);

        Assert.NotNull(TestObject);
    }

    [Test]
    public void BasicPropertiesTest()
    {
        using FontBitmapStream TestBitmapStream = CreateTestStream();
        Dictionary<LetterType, FontBitmapStream> StreamTable = new()
        {
            { LetterType.Normal, TestBitmapStream },
        };
        FontBitmapCollection TestObject = new FontBitmapCollection(StreamTable);

        Assert.AreEqual(FontBitmapCollection.DefaultColumns, TestObject.Columns);
        Assert.Less(0, TestObject.Rows);
        Assert.AreEqual(1, TestObject.SupportedLetterTypes.Count);

        LetterType TestLetterType = TestObject.SupportedLetterTypes[0];

        Assert.AreEqual(0, TestLetterType.FontSize);
        Assert.IsFalse(TestLetterType.IsItalic);
        Assert.IsFalse(TestLetterType.IsBold);
        Assert.IsFalse(TestLetterType.IsBlue);
    }

    [Test]
    public void EmptyCollectionTest()
    {
        Dictionary<LetterType, FontBitmapStream> StreamTable = new();
        FontBitmapCollection TestObject = new FontBitmapCollection(StreamTable);

        Assert.AreEqual(0, TestObject.Columns);
        Assert.AreEqual(0, TestObject.Rows);
        Assert.AreEqual(0, TestObject.SupportedLetterTypes.Count);
    }

    [Test]
    public void GetPixelArrayTest()
    {
        using FontBitmapStream TestBitmapStream = CreateTestStream();
        Dictionary<LetterType, FontBitmapStream> StreamTable = new()
        {
            { LetterType.Normal, TestBitmapStream },
        };
        FontBitmapCollection TestObject = new FontBitmapCollection(StreamTable);

        Assert.Less(0, TestObject.SupportedLetterTypes.Count);

        LetterType TestLetterType = TestObject.SupportedLetterTypes[0];

        _ = TestObject.GetPixelArray(0, 0, TestLetterType, isClipped: false);
        _ = TestObject.GetPixelArray(0, 0, TestLetterType, isClipped: true);
        Assert.Throws<ArgumentException>(() =>
        {
            TestObject.GetPixelArray(int.MaxValue, 0, TestLetterType, isClipped: false);
        });
        Assert.Throws<ArgumentException>(() =>
        {
            TestObject.GetPixelArray(0, int.MaxValue, TestLetterType, isClipped: false);
        });
        Assert.Throws<ArgumentException>(() =>
        {
            TestObject.GetPixelArray(0, 0, LetterType.ItalicBold, isClipped: false);
        });
    }

    private FontBitmapStream CreateTestStream()
    {
        var FontAssembly = typeof(Dummy).Assembly;
        var ResourceName = $"{typeof(Dummy).Namespace}.FullFontResources.Test._{TestFontSize}.black.normal.png";

        return new FontBitmapStream(FontAssembly, ResourceName, TestFontSize);
    }

    private const int TestFontSize = 75;
}

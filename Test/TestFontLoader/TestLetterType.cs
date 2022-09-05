namespace TestFontLoader;

using FontLoader;
using NUnit.Framework;

[TestFixture]
public class TestLetterType
{
    [Test]
    public void TypeTest()
    {
        Assert.IsTrue(LetterType.IsSameType(LetterType.Normal, LetterType.Normal));
        Assert.IsTrue(LetterType.IsSameType(LetterType.Italic, LetterType.Italic));
        Assert.IsTrue(LetterType.IsSameType(LetterType.Bold, LetterType.Bold));
        Assert.IsTrue(LetterType.IsSameType(LetterType.ItalicBold, LetterType.ItalicBold));

        Assert.IsFalse(LetterType.IsSameType(LetterType.Normal, LetterType.Italic));
        Assert.IsFalse(LetterType.IsSameType(LetterType.Normal, LetterType.Bold));
        Assert.IsFalse(LetterType.IsSameType(LetterType.Normal, LetterType.ItalicBold));

        Assert.IsFalse(LetterType.IsSameType(LetterType.Italic, LetterType.Normal));
        Assert.IsFalse(LetterType.IsSameType(LetterType.Italic, LetterType.Bold));
        Assert.IsFalse(LetterType.IsSameType(LetterType.Italic, LetterType.ItalicBold));

        Assert.IsFalse(LetterType.IsSameType(LetterType.Bold, LetterType.Italic));
        Assert.IsFalse(LetterType.IsSameType(LetterType.Bold, LetterType.Normal));
        Assert.IsFalse(LetterType.IsSameType(LetterType.Bold, LetterType.ItalicBold));

        Assert.IsFalse(LetterType.IsSameType(LetterType.ItalicBold, LetterType.Italic));
        Assert.IsFalse(LetterType.IsSameType(LetterType.ItalicBold, LetterType.Bold));
        Assert.IsFalse(LetterType.IsSameType(LetterType.ItalicBold, LetterType.Normal));
    }

    [Test]
    public void LetterTest()
    {
        Letter LetterANormal = new('A', LetterType.WithSizeAndColor(LetterType.Normal, 20, isBlue: false));
        Letter LetterAItalic = new('A', LetterType.WithSizeAndColor(LetterType.Italic, 20, isBlue: false));
        Letter LetterABlue = new('A', LetterType.WithSizeAndColor(LetterType.Normal, 20, isBlue: true));

        Assert.IsTrue(LetterType.IsEqual(LetterANormal.LetterType, LetterANormal.LetterType));
        Assert.IsFalse(LetterType.IsEqual(LetterANormal.LetterType, LetterAItalic.LetterType));
        Assert.IsTrue(LetterType.IsEqual(LetterANormal.LetterType, LetterABlue.LetterType));
    }
}

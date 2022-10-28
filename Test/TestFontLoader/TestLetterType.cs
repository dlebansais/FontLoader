namespace TestFontLoader;

using FontLoader;
using NUnit.Framework;

[TestFixture]
public class TestLetterType
{
    [Test]
    public void TypeTest()
    {
        Assert.IsTrue(LetterType.IsSameTypeItalicOrBold(LetterType.Normal, LetterType.Normal));
        Assert.IsTrue(LetterType.IsSameTypeItalicOrBold(LetterType.Italic, LetterType.Italic));
        Assert.IsTrue(LetterType.IsSameTypeItalicOrBold(LetterType.Bold, LetterType.Bold));
        Assert.IsTrue(LetterType.IsSameTypeItalicOrBold(LetterType.ItalicBold, LetterType.ItalicBold));
        Assert.IsFalse(LetterType.IsSameTypeItalicOrBold(LetterType.Normal, LetterType.Italic));
        Assert.IsFalse(LetterType.IsSameTypeItalicOrBold(LetterType.Normal, LetterType.Bold));
        Assert.IsFalse(LetterType.IsSameTypeItalicOrBold(LetterType.Normal, LetterType.ItalicBold));
        Assert.IsFalse(LetterType.IsSameTypeItalicOrBold(LetterType.Italic, LetterType.Normal));
        Assert.IsFalse(LetterType.IsSameTypeItalicOrBold(LetterType.Italic, LetterType.Bold));
        Assert.IsFalse(LetterType.IsSameTypeItalicOrBold(LetterType.Italic, LetterType.ItalicBold));
        Assert.IsFalse(LetterType.IsSameTypeItalicOrBold(LetterType.Bold, LetterType.Italic));
        Assert.IsFalse(LetterType.IsSameTypeItalicOrBold(LetterType.Bold, LetterType.Normal));
        Assert.IsFalse(LetterType.IsSameTypeItalicOrBold(LetterType.Bold, LetterType.ItalicBold));
        Assert.IsFalse(LetterType.IsSameTypeItalicOrBold(LetterType.ItalicBold, LetterType.Italic));
        Assert.IsFalse(LetterType.IsSameTypeItalicOrBold(LetterType.ItalicBold, LetterType.Bold));
        Assert.IsFalse(LetterType.IsSameTypeItalicOrBold(LetterType.ItalicBold, LetterType.Normal));
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

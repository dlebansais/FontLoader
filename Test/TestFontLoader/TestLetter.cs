namespace TestFontLoader;

using FontLoader;
using NUnit.Framework;

[TestFixture]
public class TestLetter
{
    [Test]
    public void BasicTest()
    {
        Letter LetterANormal = new('A', LetterType.Normal);

        Assert.AreEqual("A", LetterANormal.Text);
        Assert.AreEqual(LetterType.Normal, LetterANormal.LetterType);
        Assert.IsFalse(LetterANormal.IsEmpty);
        Assert.AreEqual("A", LetterANormal.DisplayText);
        Assert.IsFalse(LetterANormal.IsBlue);
        Assert.IsFalse(LetterANormal.IsItalic);
        Assert.IsFalse(LetterANormal.IsBold);
        Assert.IsFalse(LetterANormal.IsWhitespace);
        Assert.IsTrue(LetterANormal.IsSingleGlyph);

        Letter LetterSTNormal = new("st", LetterType.Normal);

        Assert.AreEqual("st", LetterSTNormal.Text);
        Assert.AreEqual(LetterType.Normal, LetterSTNormal.LetterType);
        Assert.IsFalse(LetterSTNormal.IsEmpty);
        Assert.AreEqual("st", LetterSTNormal.DisplayText);
        Assert.IsFalse(LetterSTNormal.IsBlue);
        Assert.IsFalse(LetterSTNormal.IsItalic);
        Assert.IsFalse(LetterSTNormal.IsBold);
        Assert.IsFalse(LetterSTNormal.IsWhitespace);
        Assert.IsFalse(LetterSTNormal.IsSingleGlyph);
        Assert.IsTrue(LetterHelper.IsWhitespace(LetterHelper.NoBreakSpace));

        Letter LetterWhitespace = new(LetterHelper.NoBreakSpace, LetterType.Normal);

        Assert.AreEqual($"{LetterHelper.NoBreakSpace}", LetterWhitespace.Text);
        Assert.AreEqual(LetterType.Normal, LetterWhitespace.LetterType);
        Assert.IsFalse(LetterWhitespace.IsEmpty);
        Assert.AreEqual($"{LetterHelper.NoBreakSpace}", LetterWhitespace.DisplayText);
        Assert.IsFalse(LetterWhitespace.IsBlue);
        Assert.IsFalse(LetterWhitespace.IsItalic);
        Assert.IsFalse(LetterWhitespace.IsBold);
        Assert.IsTrue(LetterWhitespace.IsWhitespace);
        Assert.IsTrue(LetterWhitespace.IsSingleGlyph);

        Letter LetterAForced = new("a", LetterType.Italic, isWhitespace: true, isSingleGlyph: false);

        Assert.AreEqual("a", LetterAForced.Text);
        Assert.AreEqual(LetterType.Italic, LetterAForced.LetterType);
        Assert.IsFalse(LetterAForced.IsEmpty);
        Assert.AreEqual($"{LetterAForced.LetterType.ItalicTag}a", LetterAForced.DisplayText);
        Assert.IsFalse(LetterAForced.IsBlue);
        Assert.IsTrue(LetterAForced.IsItalic);
        Assert.IsFalse(LetterAForced.IsBold);
        Assert.IsTrue(LetterAForced.IsWhitespace);
        Assert.IsFalse(LetterAForced.IsSingleGlyph);
    }

    [Test]
    public void TemplateConstructor()
    {
        Letter LetterANormal = new('A', LetterType.Normal);
        Letter LetterAItalic = new(LetterANormal, LetterType.Italic);

        Assert.AreEqual("A", LetterAItalic.Text);
        Assert.AreEqual(LetterType.Italic, LetterAItalic.LetterType);
        Assert.AreNotEqual("A", LetterAItalic.DisplayText);
        Assert.IsFalse(LetterAItalic.IsBlue);
        Assert.IsTrue(LetterAItalic.IsItalic);
        Assert.IsFalse(LetterAItalic.IsBold);
    }

    [Test]
    public void PredefinesLetters()
    {
        Letter Combo1 = Letter.EmptyNormal;

        Assert.IsTrue(Combo1.IsEmpty);
    }
}

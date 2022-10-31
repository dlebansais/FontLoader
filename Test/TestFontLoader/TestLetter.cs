namespace TestFontLoader;

using FontLoader;
using NUnit.Framework;
using System.Linq;
using System.Text;

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

        Letter LetterABlue = new('A', LetterType.Blue);

        Assert.AreEqual("A", LetterABlue.Text);
        Assert.AreEqual(LetterType.Blue, LetterABlue.LetterType);
        Assert.IsFalse(LetterABlue.IsEmpty);
        Assert.AreEqual($"{LetterABlue.LetterType.BlueTag}A", LetterABlue.DisplayText);
        Assert.IsTrue(LetterABlue.IsBlue);
        Assert.IsFalse(LetterABlue.IsItalic);
        Assert.IsFalse(LetterABlue.IsBold);
        Assert.IsFalse(LetterABlue.IsWhitespace);
        Assert.IsTrue(LetterABlue.IsSingleGlyph);

        Letter LetterAItalic = new('A', LetterType.Italic);

        Assert.AreEqual("A", LetterAItalic.Text);
        Assert.AreEqual(LetterType.Italic, LetterAItalic.LetterType);
        Assert.IsFalse(LetterAItalic.IsEmpty);
        Assert.AreEqual($"{LetterAItalic.LetterType.ItalicTag}A", LetterAItalic.DisplayText);
        Assert.IsFalse(LetterAItalic.IsBlue);
        Assert.IsTrue(LetterAItalic.IsItalic);
        Assert.IsFalse(LetterAItalic.IsBold);
        Assert.IsFalse(LetterAItalic.IsWhitespace);
        Assert.IsTrue(LetterAItalic.IsSingleGlyph);

        Letter LetterABold = new('A', LetterType.Bold);

        Assert.AreEqual("A", LetterABold.Text);
        Assert.AreEqual(LetterType.Bold, LetterABold.LetterType);
        Assert.IsFalse(LetterABold.IsEmpty);
        Assert.AreEqual($"{LetterABold.LetterType.BoldTag}A", LetterABold.DisplayText);
        Assert.IsFalse(LetterABold.IsBlue);
        Assert.IsFalse(LetterABold.IsItalic);
        Assert.IsTrue(LetterABold.IsBold);
        Assert.IsFalse(LetterABold.IsWhitespace);
        Assert.IsTrue(LetterABold.IsSingleGlyph);

        Letter LetterAItalicBold = new('A', LetterType.ItalicBold);

        Assert.AreEqual("A", LetterAItalicBold.Text);
        Assert.AreEqual(LetterType.ItalicBold, LetterAItalicBold.LetterType);
        Assert.IsFalse(LetterAItalicBold.IsEmpty);
        Assert.AreEqual($"{LetterAItalicBold.LetterType.ItalicTag}{LetterAItalicBold.LetterType.BoldTag}A", LetterAItalicBold.DisplayText);
        Assert.IsFalse(LetterAItalicBold.IsBlue);
        Assert.IsTrue(LetterAItalicBold.IsItalic);
        Assert.IsTrue(LetterAItalicBold.IsBold);
        Assert.IsFalse(LetterAItalicBold.IsWhitespace);
        Assert.IsTrue(LetterAItalicBold.IsSingleGlyph);
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

    [Test]
    public void SingleGlyphLetters()
    {
        foreach (char c in LetterHelper.MultipleGlyphCharacters)
            Assert.IsFalse(LetterHelper.IsSingleGlyph(c));

        int MaxCharCode = 0xFFFF;
        for (int i = 0; i < MaxCharCode; i++)
        {
            bool IsSurrogate = i >= 0xD800 && i <= 0xDFFF;
            if (!IsSurrogate)
            {
                string ConvertedCharacter = char.ConvertFromUtf32(i);
                if (ConvertedCharacter.Length == 1)
                {
                    char c = ConvertedCharacter[0];

                    if (!LetterHelper.MultipleGlyphCharacters.Contains(c))
                    {
                        Assert.IsTrue(LetterHelper.IsSingleGlyph(c));
                    }
                }
            }
        }
    }
}

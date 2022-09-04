﻿namespace TestFontScanner;

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

namespace TestFontLoader;

using FontLoader;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;

[TestFixture]
public class TestFont
{
    [OneTimeSetUp]
    public void CreateCelltable()
    {
        CellTable = FillCellTable();
    }

    public Dictionary<Letter, FontBitmapCell> CellTable = null!;

    [Test]
    public void BasicTest()
    {
        Font TestFont = new("Test", typeof(Dummy).Assembly, CellTable);

        Assert.AreEqual("Test", TestFont.Name);
    }

    [Test]
    public void ParsingFontResourceTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();
        Font TestFont = new("Test", TestAssembly, CellTable);

        Assert.AreEqual(0, TestFont.ProgressTable.Count);
        Assert.AreEqual(0, TestFont.CharacterTable.Count);
        Assert.AreEqual(0, TestFont.SupportedLetterTypes.Count);
        Assert.AreEqual(0, TestFont.FontSizeList.Count);
    }

    internal static Dictionary<Letter, FontBitmapCell> FillCellTable()
    {
        Dictionary<Letter, FontBitmapCell> FontCellTable = new();

        FillTestTable(FontCellTable, '£', new FontBitmapCell() { Row = 4, Column = 17 });
        FillTestTable(FontCellTable, '□', new FontBitmapCell() { Row = 4, Column = 18 });
        FillTestTable(FontCellTable, '₂', new FontBitmapCell() { Row = 5, Column = 0 });
        FillTestTable(FontCellTable, '©', new FontBitmapCell() { Row = 5, Column = 3 });
        FillTestTable(FontCellTable, '«', new FontBitmapCell() { Row = 5, Column = 5 });
        FillTestTable(FontCellTable, '®', new FontBitmapCell() { Row = 5, Column = 8 });
        FillTestTable(FontCellTable, 'ȕ', new FontBitmapCell() { Row = 5, Column = 16 });
        FillTestTable(FontCellTable, '•', new FontBitmapCell() { Row = 5, Column = 17 });
        FillTestTable(FontCellTable, 'Ī', new FontBitmapCell() { Row = 5, Column = 18 });
        FillTestTable(FontCellTable, '»', new FontBitmapCell() { Row = 6, Column = 1 });
        FillTestTable(FontCellTable, 'À', new FontBitmapCell() { Row = 6, Column = 6 });
        FillTestTable(FontCellTable, 'Â', new FontBitmapCell() { Row = 6, Column = 8 });
        FillTestTable(FontCellTable, 'Å', new FontBitmapCell() { Row = 6, Column = 11 });
        FillTestTable(FontCellTable, 'Ç', new FontBitmapCell() { Row = 6, Column = 13 });
        FillTestTable(FontCellTable, 'È', new FontBitmapCell() { Row = 6, Column = 14 });
        FillTestTable(FontCellTable, 'É', new FontBitmapCell() { Row = 6, Column = 15 });
        FillTestTable(FontCellTable, 'Ê', new FontBitmapCell() { Row = 6, Column = 16 });
        FillTestTable(FontCellTable, 'Ë', new FontBitmapCell() { Row = 6, Column = 17 });
        FillTestTable(FontCellTable, 'Ï', new FontBitmapCell() { Row = 7, Column = 1 });
        FillTestTable(FontCellTable, 'Ô', new FontBitmapCell() { Row = 7, Column = 6 });
        FillTestTable(FontCellTable, 'Ü', new FontBitmapCell() { Row = 7, Column = 14 });
        FillTestTable(FontCellTable, 'à', new FontBitmapCell() { Row = 7, Column = 18 });
        FillTestTable(FontCellTable, 'á', new FontBitmapCell() { Row = 7, Column = 19 });
        FillTestTable(FontCellTable, 'â', new FontBitmapCell() { Row = 8, Column = 0 });
        FillTestTable(FontCellTable, 'æ', new FontBitmapCell() { Row = 8, Column = 4 });
        FillTestTable(FontCellTable, 'ç', new FontBitmapCell() { Row = 8, Column = 5 });
        FillTestTable(FontCellTable, 'è', new FontBitmapCell() { Row = 8, Column = 6 });
        FillTestTable(FontCellTable, 'é', new FontBitmapCell() { Row = 8, Column = 7 });
        FillTestTable(FontCellTable, 'ê', new FontBitmapCell() { Row = 8, Column = 8 });
        FillTestTable(FontCellTable, 'ë', new FontBitmapCell() { Row = 8, Column = 9 });
        FillTestTable(FontCellTable, 'í', new FontBitmapCell() { Row = 8, Column = 11 });
        FillTestTable(FontCellTable, 'î', new FontBitmapCell() { Row = 8, Column = 12 });
        FillTestTable(FontCellTable, 'ï', new FontBitmapCell() { Row = 8, Column = 13 });
        FillTestTable(FontCellTable, 'ó', new FontBitmapCell() { Row = 8, Column = 17 });
        FillTestTable(FontCellTable, 'ô', new FontBitmapCell() { Row = 8, Column = 18 });
        FillTestTable(FontCellTable, 'ö', new FontBitmapCell() { Row = 9, Column = 0 });
        FillTestTable(FontCellTable, 'ù', new FontBitmapCell() { Row = 9, Column = 3 });
        FillTestTable(FontCellTable, 'û', new FontBitmapCell() { Row = 9, Column = 5 });
        FillTestTable(FontCellTable, 'ü', new FontBitmapCell() { Row = 9, Column = 6 });
        FillTestTable(FontCellTable, 'ć', new FontBitmapCell() { Row = 9, Column = 17 });
        FillTestTable(FontCellTable, '‒', new FontBitmapCell() { Row = 11, Column = 0 }); // Short

        FillTestTable(FontCellTable, 'œ', new FontBitmapCell() { Row = 11, Column = 1 });
        FillTestTable(FontCellTable, '—', new FontBitmapCell() { Row = 11, Column = 2 }); // Long

        FillTestTable(FontCellTable, '…', new FontBitmapCell() { Row = 11, Column = 3 });
        FillTestTable(FontCellTable, '“', new FontBitmapCell() { Row = 11, Column = 4 });
        FillTestTable(FontCellTable, '”', new FontBitmapCell() { Row = 11, Column = 5 });
        FillTestTable(FontCellTable, 'ŵ', new FontBitmapCell() { Row = 11, Column = 6 });
        FillTestTable(FontCellTable, 'þ', new FontBitmapCell() { Row = 11, Column = 8 });
        FillTestTable(FontCellTable, '‘', new FontBitmapCell() { Row = 11, Column = 9 });
        FillTestTable(FontCellTable, 'ᾱ', new FontBitmapCell() { Row = 11, Column = 13 });
        FillTestTable(FontCellTable, 'ῑ', new FontBitmapCell() { Row = 11, Column = 14 });
        FillTestTable(FontCellTable, 'ῡ', new FontBitmapCell() { Row = 11, Column = 15 });
        FillTestTable(FontCellTable, 'ʿ', new FontBitmapCell() { Row = 11, Column = 16 });
        FillTestTable(FontCellTable, 'ḥ', new FontBitmapCell() { Row = 11, Column = 17 });
        FillTestTable(FontCellTable, 'ṣ', new FontBitmapCell() { Row = 11, Column = 18 });
        FillTestTable(FontCellTable, '’', new FontBitmapCell() { Row = 11, Column = 19 });
        return FontCellTable;
    }

    internal static void FillTestTable(Dictionary<Letter, FontBitmapCell> fontCellTable, char character, FontBitmapCell bitmapCell)
    {
        fontCellTable.Add(new Letter(character, LetterType.Normal), bitmapCell);
        fontCellTable.Add(new Letter(character, LetterType.Italic), bitmapCell);
        fontCellTable.Add(new Letter(character, LetterType.Bold), bitmapCell);
        fontCellTable.Add(new Letter(character, LetterType.ItalicBold), bitmapCell);
    }
}

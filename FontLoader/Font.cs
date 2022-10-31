namespace FontLoader;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;

[DebuggerDisplay("{Name,nq}")]
public class Font
{
    #region Init
    public Font(string fontName, Assembly fontAssembly, Dictionary<Letter, FontBitmapCell> cellTable)
    {
        Name = fontName;
        ProgressTable = FillProgressTable(fontAssembly);

        FontBitmapCollection BitmapCollection = FillFontBitmap(fontAssembly);

        CharacterTable = FillCharacterTable(BitmapCollection, cellTable);
        SupportedLetterTypes = BitmapCollection.SupportedLetterTypes;
        FontSizeList = GetFontSizeList(SupportedLetterTypes);
    }

    private Dictionary<char, PixelArray> FillProgressTable(Assembly fontAssembly)
    {
        Dictionary<char, PixelArray> ProgressTable = new();
        string[] ResourceNames = fontAssembly.GetManifestResourceNames();

        foreach (string ResourceName in ResourceNames)
            if (TryParseProgressFontResource(ResourceName, out char Character))
            {
                Stream PageBitmapStream = fontAssembly.GetManifestResourceStream(ResourceName);
                using Bitmap PageBitmap = new(PageBitmapStream);
                PixelArray Array = PixelArray.FromBitmap(PageBitmap);

                ProgressTable.Add(Character, Array);
            }

        return ProgressTable;
    }

    private bool TryParseProgressFontResource(string resourceName, out char character)
    {
        character = '\0';

        string[] Splitted = resourceName.Split('.');

        Debug.Assert(Splitted.Length >= 3);

        if (Splitted[Splitted.Length - 3] != "PageResources")
            return false;

        string CharacterString = Splitted[Splitted.Length - 2];
        string Extension = Splitted[Splitted.Length - 1];

        if (!CharacterString.StartsWith("Page"))
            return false;

        CharacterString = CharacterString.Substring(4);

        if (CharacterString == "Slash")
            character = '/';
        else if (CharacterString.Length != 1)
            return false;
        else 
        {
            bool Test001=true;
            if(!(CharacterString[0] >= '0'))
                Test001=false;
            if(!(CharacterString[0] <= '9'))
                Test001=false;
            if (Test001)
                character = CharacterString[0];
            else
                return false;
        }

        if (Extension != "png")
            return false;

        return true;
    }

    private FontBitmapCollection FillFontBitmap(Assembly fontAssembly)
    {
        Dictionary<LetterType, FontBitmapStream> StreamTable = new();
        string[] ResourceNames = fontAssembly.GetManifestResourceNames();

        foreach (string ResourceName in ResourceNames)
            if (TryParseFontResource(ResourceName, out double FontSize, out TypeFlags TypeFlags))
            {
                LetterType FontLetterType = new(FontSize, TypeFlags);
                FontBitmapStream FontBitmapStream = new(fontAssembly, ResourceName, FontSize);

                StreamTable.Add(FontLetterType, FontBitmapStream);
            }

        return new FontBitmapCollection(StreamTable);
    }

    private bool TryParseFontResource(string resourceName, out double fontSize, out TypeFlags typeFlags)
    {
        fontSize = 0;
        typeFlags = TypeFlags.Normal;

        string[] Splitted = resourceName.Split('.');

        if (Splitted.Length < 7)
            return false;

        if (Splitted[Splitted.Length - 6] != "FullFontResources")
            return false;

        string FontSizeString = Splitted[Splitted.Length - 4];
        string FontColorString = Splitted[Splitted.Length - 3];
        string LetterTypeString = Splitted[Splitted.Length - 2];
        string Extension = Splitted[Splitted.Length - 1];

        if (!TryParseFontSizeColorAndType(FontSizeString, FontColorString, LetterTypeString, out fontSize, out typeFlags))
            return false;

        if (Extension != "png")
            return false;

        return true;
    }

    private bool TryParseFontSizeColorAndType(string fontSizeString, string fontColorString, string letterTypeString, out double fontSize, out TypeFlags typeFlags)
    {
        typeFlags = TypeFlags.Normal;

        while (fontSizeString.StartsWith("_") || fontSizeString.StartsWith("0"))
            fontSizeString = fontSizeString.Substring(1);

        fontSizeString = fontSizeString.Replace("x", ".");

        if (!double.TryParse(fontSizeString, NumberStyles.Float, CultureInfo.InvariantCulture, out fontSize))
            return false;

        if (fontSize < LetterType.MinFontSize)
            return false;

        if (fontColorString == "blue")
        {
            typeFlags |= TypeFlags.Blue;
        }
        else if (fontColorString == "black")
        {
        }
        else
            return false;

        if (letterTypeString == "normal")
        {
        }
        else if (letterTypeString == "italic")
        {
            typeFlags |= TypeFlags.Italic;
        }
        else if (letterTypeString == "bold")
        {
            typeFlags |= TypeFlags.Bold;
        }
        else if (letterTypeString == "italic+bold")
        {
            typeFlags |= TypeFlags.Italic | TypeFlags.Bold;
        }
        else
            return false;

        return true;
    }

    private Dictionary<Letter, PixelArray> FillCharacterTable(FontBitmapCollection bitmap, Dictionary<Letter, FontBitmapCell> cellTable)
    {
        Dictionary<Letter, PixelArray> CharacterTable = new();
        bool[,] CellTaken = new bool[bitmap.Columns, bitmap.Rows];

        foreach (KeyValuePair<Letter, FontBitmapCell> Entry in cellTable)
        {
            Letter Letter = Entry.Key;
            FontBitmapCell Cell = Entry.Value;

            Debug.Assert(Cell.Column >= 0);
            Debug.Assert(Cell.Row >= 0);

            bool Test002=true;if(!(Cell.Column < bitmap.Columns))Test002=false;if(!(Cell.Row < bitmap.Rows))Test002=false;if (Test002)
            {
                CellTaken[Cell.Column, Cell.Row] = true;
                AddLetter(bitmap, Cell.Column, Cell.Row, CharacterTable, Letter);
            }
        }

        Debug.Assert(new List<Letter>(CharacterTable.Keys).TrueForAll((Letter l) => l.LetterType.FontSize >= LetterType.MinFontSize));
        return CharacterTable;
    }

    private void AddLetter(FontBitmapCollection bitmap, int column, int row, Dictionary<Letter, PixelArray> characterTable, Letter letter)
    {
        foreach (LetterType Key in bitmap.SupportedLetterTypes)
            if (LetterType.IsSameTypeItalicOrBold(Key, letter.LetterType))
            {
                PixelArray CellArray = bitmap.GetPixelArray(column, row, Key, isClipped: true);

                Debug.Assert(CellArray != PixelArray.Empty);
                AddLetter(characterTable, letter, Key.FontSize, Key.IsBlue, CellArray);
            }
    }

    private void AddLetter(Dictionary<Letter, PixelArray> characterTable, Letter letter, double fontSize, bool isBlue, PixelArray cellArray)
    {
        LetterType NewLetterType = LetterType.WithSizeAndColor(letter.LetterType, fontSize, isBlue);
        Letter NewLetter = new(letter, NewLetterType);

        characterTable.Add(NewLetter, cellArray);
    }

    private static List<double> GetFontSizeList(List<LetterType> letterTypeList)
    {
        List<double> FontSizeList = new();

        foreach (LetterType Item in letterTypeList)
            if (!FontSizeList.Contains(Item.FontSize))
                FontSizeList.Add(Item.FontSize);

        FontSizeList.Sort();
        return FontSizeList;
    }
    #endregion

    #region Properties
    public string Name { get; }
    public Dictionary<char, PixelArray> ProgressTable { get; }
    public Dictionary<Letter, PixelArray> CharacterTable { get; }
    public List<LetterType> SupportedLetterTypes { get; }
    public List<double> FontSizeList { get; }
    #endregion

    #region Tools
    public static int FontSizeToCellSize(double fontSize)
    {
        return (int)Math.Round(fontSize * 0.8);
    }
    #endregion
}

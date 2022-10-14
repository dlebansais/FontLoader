namespace FontLoader;

using System.Diagnostics;

[DebuggerDisplay("{DisplayText,nq}")]
public record Letter
{
    public static readonly Letter EmptyNormal = new();
    /*
    public static readonly Letter EmptyItalic = new(LetterType.Italic);
    public static readonly Letter EmptyBold = new(LetterType.Bold);
    */
    public static readonly Letter Unknown = new("█");
    public static readonly Letter Whitespace = new(" ");
    public static readonly Letter Ignore1 = new("█");
    public static readonly Letter Combo1 = new('!', '”', LetterType.Italic);
    public static readonly Letter Combo2 = new('t', 'h', LetterType.Normal);
    public static readonly Letter Combo3 = new('s', 't', LetterType.Normal);
    public static readonly Letter Combo4 = new('n', 'd', LetterType.Normal);
    public static readonly Letter SpecialJ = new(' ', 'j', LetterType.Normal);
    public static readonly Letter SpecialJItalic = new(' ', 'j', LetterType.Italic);
    public static readonly Letter SubscriptReserved = new(' ', '®', LetterType.Normal);

    private Letter()
        : this(string.Empty, LetterType.Normal)
    {
        int X = 5;
        bool IsPositive = X >= 0;
        bool IsOdd = X % 2 == 1;

        for (int i = 0; i < 2; i++)
            if (IsPositive || IsOdd)
            {
                IsPositive= false;
                IsOdd = false;
            }
    }

    private Letter(string text)
        : this(text, LetterType.Normal)
    {
    }

    public Letter(char character, LetterType letterType)
        : this(character.ToString(), letterType)
    {
    }

    private Letter(char character1, char character2, LetterType letterType)
        : this($"{character1}{character2}", letterType)
    {
    }

    public Letter(Letter template, LetterType letterType)
        : this(template.Text, letterType)
    {
    }

    public Letter(string text, LetterType letterType)
    {
        Text = text;
        LetterType = letterType;
        IsWhitespace = LetterHelper.IsWhitespace(text);
        IsSingleGlyph = LetterHelper.IsSingleGlyph(text);
    }

    [System.Text.Json.Serialization.JsonConstructor]
    public Letter(string text, LetterType letterType, bool isWhitespace, bool isSingleGlyph)
    {
        Text = text;
        LetterType = letterType;
        IsWhitespace = isWhitespace;
        IsSingleGlyph = isSingleGlyph;
    }

    public string Text { get; }
    public LetterType LetterType { get; }
    public bool IsWhitespace { get; }
    public bool IsSingleGlyph { get; }

    public bool IsEmpty { get { return Text.Length == 0; } }
    public bool IsBlue { get { return LetterType.IsBlue; } }
    public bool IsItalic { get { return LetterType.IsItalic; } }
    public bool IsBold { get { return LetterType.IsBold; } }
    public string DisplayText { get { return $"{LetterType.BlueTag}{LetterType.ItalicTag}{LetterType.BoldTag}{Text}"; } }
}

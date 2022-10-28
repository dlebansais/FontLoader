namespace FontLoader;

using System.Diagnostics;
using System.Text.Json.Serialization;

[DebuggerDisplay("{DisplayText,nq}")]
public record Letter
{
    public static readonly Letter EmptyNormal = new();
    public static readonly Letter Unknown = new("█");
    public static readonly Letter Whitespace = new(" ");

    private Letter()
        : this(string.Empty, LetterType.Normal)
    {
    }

    private Letter(string text)
        : this(text, LetterType.Normal)
    {
    }

    public Letter(char character, LetterType letterType)
        : this(character.ToString(), letterType)
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

    [JsonConstructor]
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

    public bool IsEmpty
    {
        get { return Text.Length == 0; }
    }

    public bool IsBlue
    {
        get { return LetterType.IsBlue; }
    }

    public bool IsItalic
    {
        get { return LetterType.IsItalic; }
    }

    public bool IsBold
    {
        get { return LetterType.IsBold; }
    }

    public string DisplayText
    {
        get { return $"{LetterType.BlueTag}{LetterType.ItalicTag}{LetterType.BoldTag}{Text}"; }
    }
}

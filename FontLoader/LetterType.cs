namespace FontLoader;

using System.Diagnostics;

[DebuggerDisplay("{FontSize}{!IsBlue && !IsItalic && !IsBold ? \" Normal\" : \"\",nq}{IsBlue ? \" Blue\" : \"\",nq}{IsItalic ? \" Italic\" : \"\",nq}{IsBold ? \" Bold\" : \"\",nq}")]
public record LetterType
{
    public const int MinFontSize = 8;
    public static readonly LetterType Normal = new(TypeFlags.Normal);
    public static readonly LetterType Italic = new(TypeFlags.Italic);
    public static readonly LetterType Bold = new(TypeFlags.Bold);
    public static readonly LetterType ItalicBold = new(TypeFlags.Italic | TypeFlags.Bold);

    private LetterType(TypeFlags typeFlags)
        : this(fontSize: 0, typeFlags)
    {
    }

    public LetterType(double fontSize, TypeFlags typeFlags)
    {
        FontSize = fontSize;
        TypeFlags = typeFlags;
    }

    public double FontSize { get; }

    public bool IsBlue
    {
        get { return TypeFlags.HasFlag(TypeFlags.Blue); }
    }

    public bool IsItalic
    {
        get { return TypeFlags.HasFlag(TypeFlags.Italic); }
    }

    public bool IsBold
    {
        get { return TypeFlags.HasFlag(TypeFlags.Bold); }
    }

    private TypeFlags TypeFlags;

    [DebuggerHidden]
    public string BlueTag
    {
        get { return IsBlue ? "§" : string.Empty; }
    }

    [DebuggerHidden]
    public string ItalicTag
    {
        get { return IsItalic ? "*" : string.Empty; }
    }

    [DebuggerHidden]
    public string BoldTag
    {
        get { return IsBold ? "#" : string.Empty; }
    }

    public static bool IsSameTypeItalicOrBold(LetterType l1, LetterType l2)
    {
        return l1.IsItalic == l2.IsItalic && l1.IsBold == l2.IsBold;
    }

    public static bool IsEqual(LetterType l1, LetterType l2)
    {
        Debug.Assert(l1.FontSize > 0);
        Debug.Assert(l2.FontSize > 0);
        return IsSameTypeItalicOrBold(l1, l2) && l1.FontSize == l2.FontSize;
    }

    public static LetterType WithSizeAndColor(LetterType l, double fontSize, bool isBlue)
    {
        Debug.Assert(l.FontSize == 0);
        Debug.Assert(fontSize >= MinFontSize);
        Debug.Assert(!l.IsBlue);
        return new(fontSize, (isBlue ? TypeFlags.Blue : TypeFlags.Normal) | l.TypeFlags);
    }
}

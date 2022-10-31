namespace FontLoader;

using System.Linq;

public static class LetterHelper
{
    public const char NoBreakSpace = '\x00A0';
    public const char SoftHypen = '\x00AD';
    public const char Cedilla = '\x00B8';
    public static readonly char[] WhitespaceCharacters = new char[]
    {
        NoBreakSpace, SoftHypen, Cedilla,
    };
    public static readonly char[] MultipleGlyphCharacters = new char[]
    {
        '"', '…', '“', '”', '¨', 'ď',
    };

    public static bool IsWhitespace(string text) => text.Length == 1 && IsWhitespace(text[0]);
    public static bool IsWhitespace(char c) => WhitespaceCharacters.Contains(c);
    public static bool IsSingleGlyph(string text) => text.Length == 1 && IsSingleGlyph(text[0]);
    public static bool IsSingleGlyph(char c) => !MultipleGlyphCharacters.Contains(c);
}

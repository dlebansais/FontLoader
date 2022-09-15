namespace FontLoader;

using System.Diagnostics;

[DebuggerDisplay("{CellSize}")]
public record LetterTypeBitmap
{
    public LetterTypeBitmap(int columns, int rows, int cellSize, int baseline, int stride, byte[] argbValues)
    {
        Columns = columns;
        Rows = rows;
        CellSize = cellSize;
        Baseline = baseline;
        Stride = stride;
        ArgbValues = argbValues;
    }

    public int Columns { get; }
    public int Rows { get; }
    public int CellSize { get; }
    public int Baseline { get; }
    public int Stride { get; }
    public byte[] ArgbValues { get; }
}

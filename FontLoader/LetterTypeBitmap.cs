namespace FontLoader;

using System;
using System.Diagnostics;

[DebuggerDisplay("{FontSize}")]
public record LetterTypeBitmap
{
    #region Init
    public LetterTypeBitmap(int columns, int rows, FontBitmap sourceBitmap)
    {
        Columns = columns;
        Rows = rows;
        SourceBitmap = sourceBitmap;

        CellSize = Font.FontSizeToCellSize(SourceBitmap.FontSize);
        Baseline = (int)Math.Round(CellSize * FontBitmapCollection.DefaultBaselineRatio);
    }
    #endregion

    #region Properties
    public int Columns { get; }
    public int Rows { get; }
    public double FontSize { get { return SourceBitmap.FontSize; } }
    public int CellSize { get; }
    public int Baseline { get; }
    #endregion

    #region Client Interface
    public void GetBitmapBytes(out byte[] argbValues, out int stride)
    {
        SourceBitmap.GetBitmapBytes(out argbValues, out stride, out int Width, out _);
        Debug.Assert(SourceBitmap.IsLoaded);

        int ExpectedCellSize = Width / FontBitmapCollection.DefaultColumns;
        Debug.Assert(CellSize == ExpectedCellSize);

        int ExpectedWidth = CellSize * FontBitmapCollection.DefaultColumns;
        Debug.Assert(Width == ExpectedWidth);
    }

    private FontBitmap SourceBitmap;
    #endregion
}

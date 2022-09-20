namespace FontLoader;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

[DebuggerDisplay("{Columns} Column(s), {Rows} Row(s)")]
public class FontBitmapCollection
{
    #region Constants
    public const int DefaultColumns = 20;
    public const double DefaultBaselineRatio = 0.65;
    #endregion

    #region Init
    public FontBitmapCollection(Dictionary<LetterType, FontBitmapStream> streamTable)
    {
        SupportedLetterTypes = new List<LetterType>(streamTable.Keys);
        LetterTypeBitmapTable = new();

        FillBitmapTable(streamTable, out Dictionary<LetterType, FontBitmap> BitmapTable, out int MaxColumns, out int MaxRows);

        if (MaxColumns > 0 && MaxRows > 0)
        {
            Columns = MaxColumns;
            Rows = MaxRows;

            foreach (LetterType Key in SupportedLetterTypes)
            {
                FontBitmap SourceBitmap = BitmapTable[Key];
                LetterTypeBitmap NewLetterTypeBitmap = new(Columns, Rows, SourceBitmap);
                LetterTypeBitmapTable.Add(Key, NewLetterTypeBitmap);
            }
        }
    }

    private void FillBitmapTable(Dictionary<LetterType, FontBitmapStream> streamTable, out Dictionary<LetterType, FontBitmap> bitmapTable, out int maxColumns, out int maxRows)
    {
        bitmapTable = new();
        maxColumns = 0;
        maxRows = 0;

        foreach (LetterType Key in SupportedLetterTypes)
        {
            FontBitmapStream StreamEntry = streamTable[Key];
            FontBitmap NewFontBitmap = new FontBitmap(StreamEntry);
            bitmapTable.Add(Key, NewFontBitmap);

            if (maxColumns == 0 && maxRows == 0)
            {
                Bitmap FirstBitmap = NewFontBitmap.LoadBitmap();
                int Columns = DefaultColumns;
                int CellSize = FirstBitmap.Width / Columns;
                int Rows = FirstBitmap.Height / CellSize;

                Debug.Assert(Columns > 0);
                Debug.Assert(Rows > 0);

                maxColumns = Columns;
                maxRows = Rows;
            }
        }
    }
    #endregion

    #region Properties
    public int Columns { get; }
    public int Rows { get; }
    public List<LetterType> SupportedLetterTypes { get; }

    public PixelArray GetPixelArray(int column, int row, LetterType letterType, bool isClipped)
    {
        if (!LetterTypeBitmapTable.ContainsKey(letterType))
            throw new ArgumentException(nameof(letterType));

        LetterTypeBitmap LetterTypeBitmap = LetterTypeBitmapTable[letterType];

        if (column >= LetterTypeBitmap.Columns)
            throw new ArgumentException(nameof(column));

        if (row >= LetterTypeBitmap.Rows)
            throw new ArgumentException(nameof(row));

        int CellSize = LetterTypeBitmap.CellSize;
        int Baseline = LetterTypeBitmap.Baseline;
        PixelArray Result = new PixelArray(LetterTypeBitmap, column, row, CellSize, CellSize, Baseline, clearEdges: true, isClipped);

        return Result;
    }

    private Dictionary<LetterType, LetterTypeBitmap> LetterTypeBitmapTable;
    #endregion
}

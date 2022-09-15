﻿namespace FontLoader;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

[DebuggerDisplay("{Columns} Column(s), {Rows} Row(s)")]
public class FontBitmap
{
    #region Constants
    public const int DefaultColumns = 20;
    public const double DefaultBaselineRatio = 0.65;
    #endregion

    #region Init
    public FontBitmap(Dictionary<LetterType, Stream> streamTable)
    {
        SupportedLetterTypes = new List<LetterType>(streamTable.Keys);
        LetterTypeBitmapTable = new();

        FillBitmapTable(streamTable, out Dictionary<LetterType, Bitmap> BitmapTable, out int MaxColumns, out int MaxRows);

        if (MaxColumns > 0 && MaxRows > 0)
        {
            Columns = MaxColumns;
            Rows = MaxRows;

            foreach (LetterType Key in SupportedLetterTypes)
            {
                int Width = BitmapTable[Key].Width;
                int Height = BitmapTable[Key].Height;

                int CellSize = Width / Columns;
                int Baseline = (int)(CellSize * DefaultBaselineRatio);
                Rectangle Rect = new Rectangle(0, 0, Width, Height);
                GetBitmapBytes(BitmapTable[Key], Rect, out int Stride, out byte[] ArgbValues);

                LetterTypeBitmap NewLetterTypeBitmap = new(Width / CellSize, Height / CellSize, CellSize, Baseline, Stride, ArgbValues);
                LetterTypeBitmapTable.Add(Key, NewLetterTypeBitmap);
            }
        }
    }

    private void FillBitmapTable(Dictionary<LetterType, Stream> streamTable, out Dictionary<LetterType, Bitmap> bitmapTable, out int maxColumns, out int maxRows)
    {
        bitmapTable = new();
        maxColumns = 0;
        maxRows = 0;

        foreach (LetterType Key in SupportedLetterTypes)
        {
            using Stream FontBitmapStream = streamTable[Key];
            Bitmap NewBitmap = new Bitmap(FontBitmapStream);
            bitmapTable.Add(Key, NewBitmap);

            int Columns = DefaultColumns;
            int CellSize = NewBitmap.Width / Columns;
            int Rows = NewBitmap.Height / CellSize;

            if (maxColumns <= Columns && maxRows <= Rows)
            {
                maxColumns = Columns;
                maxRows = Rows;
            }
        }

        // Check that for a given font size all bitmaps have the same with and height.
        for (int i = 0; i < SupportedLetterTypes.Count; i++)
            for (int j = i + 1; j < SupportedLetterTypes.Count; j++)
                if (SupportedLetterTypes[i].FontSize == SupportedLetterTypes[j].FontSize)
                {
                    Bitmap Bitmap1 = bitmapTable[SupportedLetterTypes[i]];
                    Bitmap Bitmap2 = bitmapTable[SupportedLetterTypes[j]];
                    Debug.Assert(Bitmap1.Width == Bitmap2.Width);
                    Debug.Assert(Bitmap1.Height == Bitmap2.Height);
                }
    }
    #endregion

    #region Properties
    public int Columns { get; }
    public int Rows { get; }
    public List<LetterType> SupportedLetterTypes { get; }

    public PixelArray GetPixelArray(int column, int row, LetterType letterType)
    {
        LetterTypeBitmap LetterTypeBitmap = LetterTypeBitmapTable[letterType];
        if (column < LetterTypeBitmap.Columns && row < LetterTypeBitmap.Rows)
        {
            int CellSize = LetterTypeBitmap.CellSize;
            int Baseline = LetterTypeBitmap.Baseline;
            int Stride = LetterTypeBitmap.Stride;
            byte[] ArgbValues = LetterTypeBitmap.ArgbValues;

            return new PixelArray(column * CellSize, CellSize, row * CellSize, CellSize, ArgbValues, Stride, Baseline, clearEdges: true);
        }
        else
            return PixelArray.Empty;
    }
    #endregion

    #region Implementation
    private static void GetBitmapBytes(Bitmap bitmap, Rectangle rect, out int stride, out byte[] argbValues)
    {
        BitmapData Data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        stride = Math.Abs(Data.Stride);

        int ByteCount = stride * rect.Height;
        argbValues = new byte[ByteCount];

        // Copy the RGB values into the array.
        Marshal.Copy(Data.Scan0, argbValues, 0, ByteCount);

        bitmap.UnlockBits(Data);
    }

    private Dictionary<LetterType, LetterTypeBitmap> LetterTypeBitmapTable;
    #endregion
}

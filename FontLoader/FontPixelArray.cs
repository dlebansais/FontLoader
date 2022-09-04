namespace FontLoader;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

[DebuggerDisplay("{Width} x {Height}, {Baseline}")]
public class FontPixelArray
{
    public static readonly FontPixelArray Empty = new();
    public const double MaxSuportedDiffRatio = 0.2;

    private FontPixelArray()
    {
        Width = 0;
        Height = 0;
        Baseline = 0;
        Array = new byte[0, 0];
        IsWhiteColumn = new bool[0];
        ColoredCountColumn = new int[0];
    }

    public FontPixelArray(int width, int height, int baseline)
    {
        Width = width;
        Height = height;
        Baseline = baseline;

        Array = new byte[Width, Height];
        IsWhiteColumn = new bool[Width];
        ColoredCountColumn = new int[Width];
    }

    public FontPixelArray(int left, int width, int top, int height, byte[] argbValues, int stride, int baseline, bool clearEdges)
    {
        Width = width;
        Height = height;
        Baseline = baseline;

        Array = new byte[Width, Height];
        IsWhiteColumn = new bool[Width];
        ColoredCountColumn = new int[Width];

        for (int x = 0; x < width; x++)
        {
            bool IsWhite = true;
            int ColoredCount = 0;

            for (int y = 0; y < height; y++)
            {
                int Offset = ((top + y) * stride) + ((left + x) * 4);

                int B = argbValues[Offset + 0];
                int G = argbValues[Offset + 1];
                int R = argbValues[Offset + 2];

                byte Pixel;

                if (clearEdges && (x == 0 || y == 0))
                    Pixel = 0xFF;
                else
                    Pixel = (byte)((R + G + B) / 3);

                Array[x, y] = Pixel;

                if (Pixel != 0xFF)
                {
                    IsWhite = false;

                    if (Pixel != 0)
                        ColoredCount++;
                }
            }

            IsWhiteColumn[x] = IsWhite;
            ColoredCountColumn[x] = ColoredCount;
        }
    }

    public static FontPixelArray FromBitmap(Bitmap bitmap)
    {
        return FromBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
    }

    public static FontPixelArray FromBitmap(Bitmap bitmap, Rectangle rect)
    {
        int Width = bitmap.Width;
        int Height = bitmap.Height;
        int Baseline = Height - 1;

        Rectangle FullRect = new Rectangle(0, 0, Width, Height);
        BitmapData Data = bitmap.LockBits(FullRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        int Stride = Math.Abs(Data.Stride);

        int ByteCount = Data.Stride * FullRect.Height;
        byte[] ArgbValues = new byte[ByteCount];

        Marshal.Copy(Data.Scan0, ArgbValues, 0, ByteCount);

        bitmap.UnlockBits(Data);

        return new FontPixelArray(rect.Left, rect.Width, rect.Top, rect.Height, ArgbValues, Stride, Baseline, clearEdges: false);
    }

    public int Width { get; }
    public int Height { get; }
    public int Baseline { get; }

    private byte[,] Array;
    private bool[] IsWhiteColumn;
    private int[] ColoredCountColumn;

    public bool IsWhite(int x, int y)
    {
        return Array[x, y] == 0xFF;
    }

    private static void CopyPixel(FontPixelArray p1, int x1, int y1, FontPixelArray p2, int x2, int y2, ref bool isWhite, ref int coloredCount)
    {
        byte Pixel = p1.Array[x1, y1];

        p2.Array[x2, y2] = Pixel;
        UpdatePixelFlags(Pixel, ref isWhite, ref coloredCount);
    }

    private static void UpdatePixelFlags(byte pixel, ref bool isWhite, ref int coloredCount)
    {
        if (pixel != 0xFF)
        {
            isWhite = false;

            if (pixel != 0)
                coloredCount++;
        }
    }

    public FontPixelArray Clipped()
    {
        int LeftEdge;
        int RightEdge;
        int TopEdge;
        int BottomEdge;

        for (LeftEdge = 0; LeftEdge < Width; LeftEdge++)
        {
            bool IsEmptyColumn = true;
            for (int y = 0; y < Height; y++)
            {
                if (!IsWhite(LeftEdge, y))
                {
                    IsEmptyColumn = false;
                    break;
                }
            }

            if (!IsEmptyColumn)
                break;
        }

        for (RightEdge = Width; RightEdge > 0; RightEdge--)
        {
            bool IsEmptyColumn = true;
            for (int y = 0; y < Height; y++)
            {
                if (!IsWhite(RightEdge - 1, y))
                {
                    IsEmptyColumn = false;
                    break;
                }
            }

            if (!IsEmptyColumn)
                break;
        }

        for (TopEdge = 0; TopEdge < Height; TopEdge++)
        {
            bool IsEmptyLine = true;
            for (int x = 0; x < Width; x++)
            {
                if (!IsWhite(x, TopEdge))
                {
                    IsEmptyLine = false;
                    break;
                }
            }

            if (!IsEmptyLine)
                break;
        }


        for (BottomEdge = Height; BottomEdge > 0; BottomEdge--)
        {
            bool IsEmptyLine = true;
            for (int x = 0; x < Width; x++)
            {
                if (!IsWhite(x, BottomEdge - 1))
                {
                    IsEmptyLine = false;
                    break;
                }
            }

            if (!IsEmptyLine)
                break;
        }

        if (LeftEdge < RightEdge && TopEdge < BottomEdge)
        {
            if (LeftEdge > 0 || RightEdge < Width || TopEdge > 0 || BottomEdge < Height)
            {
                FontPixelArray Result = new(RightEdge - LeftEdge, BottomEdge - TopEdge, Baseline - TopEdge);

                for (int x = 0; x < Result.Width; x++)
                {
                    bool IsWhite = true;
                    int ColoredCount = 0;

                    for (int y = 0; y < Result.Height; y++)
                        CopyPixel(this, LeftEdge + x, TopEdge + y, Result, x, y, ref IsWhite, ref ColoredCount);

                    Result.IsWhiteColumn[x] = IsWhite;
                    Result.ColoredCountColumn[x] = ColoredCount;
                }

                Debug.Assert(Result.IsClipped);
                return Result;
            }
            else
                return this;
        }
        else
            return Empty;
    }

    public bool IsClipped
    {
        get
        {
            return IsClippedColumn(0) && IsClippedColumn(Width - 1) && IsClippedRow(0) && IsClippedRow(Height - 1);
        }
    }

    public bool IsClippedColumn(int column)
    {
        bool IsWhiteColumn = true;
        for (int y = 0; y < Height; y++)
            if (!IsWhite(column, y))
            {
                IsWhiteColumn = false;
                break;
            }

        return !IsWhiteColumn;
    }

    public bool IsClippedRow(int row)
    {
        bool IsWhiteRow = true;
        for (int x = 0; x < Width; x++)
            if (!IsWhite(x, row))
            {
                IsWhiteRow = false;
                break;
            }

        return !IsWhiteRow;
    }

    public void DebugPrint()
    {
        for (int y = 0; y < Height; y++)
        {
            string Line = string.Empty;

            for (int x = 0; x < Width; x++)
            {
                uint RGB = Array[x, y];
                uint Pixel = (((RGB >> 0) & 0xFF) + ((RGB >> 8) & 0xFF) + ((RGB >> 16) & 0xFF)) / 3;
                Line += Pixel < 0x40 ? "X" : (y == Baseline ? "." : " ");
            }

            Debug.WriteLine(Line);
        }
    }
}

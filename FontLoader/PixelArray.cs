namespace FontLoader;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

[DebuggerDisplay("{Width} x {Height}, {Baseline}")]
public class PixelArray
{
    public static readonly PixelArray Empty = new();
    public const double MaxSuportedDiffRatio = 0.2;

    private PixelArray()
    {
        Width = 0;
        Height = 0;
        Baseline = 0;
        Array = new byte[0];
        WhiteColumn = new bool[0];
        ColoredCountColumn = new int[0];
    }

    public PixelArray(int width, int height, int baseline)
    {
        Width = width;
        Height = height;
        Baseline = baseline;
        Array = new byte[Width * Height];
        WhiteColumn = new bool[Width];
        ColoredCountColumn = new int[Width];
    }

    public PixelArray(int left, int width, int top, int height, byte[] argbValues, int stride, int baseline, bool clearEdges)
    {
        Width = width;
        Height = height;
        Baseline = baseline;
        Array = new byte[Width * Height];
        WhiteColumn = new bool[Width];
        ColoredCountColumn = new int[Width];

        for (int x = 0; x < Width; x++)
            WhiteColumn[x] = true;

        int ArrayOffset = 0;

        for (int y = 0; y < Height; y++)
        {
            int ArgbValuesOffset = ((top + y) * stride) + (left * 4);

            for (int x = 0; x < Width; x++)
            {
                int B = argbValues[ArgbValuesOffset + 0];
                int G = argbValues[ArgbValuesOffset + 1];
                int R = argbValues[ArgbValuesOffset + 2];
                int Pixel = (clearEdges && (x == 0 || y == 0)) ? 0xFF : (R + G + B) / 3;
                Array[ArrayOffset] = (byte)Pixel;

                if (Pixel != 0xFF)
                {
                    WhiteColumn[x] = false;

                    if (Pixel != 0)
                        ColoredCountColumn[x]++;
                }

                ArgbValuesOffset += 4;
                ArrayOffset++;
            }
        }
    }

    public static PixelArray FromBitmap(Bitmap bitmap)
    {
        return FromBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 1);
    }

    public static PixelArray FromBitmap(Bitmap bitmap, int baselineDiff)
    {
        return FromBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), baselineDiff);
    }

    public static PixelArray FromBitmap(Bitmap bitmap, Rectangle rect)
    {
        return FromBitmap(bitmap, rect, 1);
    }

    public static PixelArray FromBitmap(Bitmap bitmap, Rectangle rect, int baselineDiff)
    {
        int Width = bitmap.Width;
        int Height = bitmap.Height;
        int Baseline = Height - baselineDiff;

        Rectangle FullRect = new Rectangle(0, 0, Width, Height);
        BitmapData Data = bitmap.LockBits(FullRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        int Stride = Math.Abs(Data.Stride);

        int ByteCount = Data.Stride * FullRect.Height;
        byte[] ArgbValues = new byte[ByteCount];

        Marshal.Copy(Data.Scan0, ArgbValues, 0, ByteCount);

        bitmap.UnlockBits(Data);

        return new PixelArray(rect.Left, rect.Width, rect.Top, rect.Height, ArgbValues, Stride, Baseline, clearEdges: false);
    }

    public int Width { get; }
    public int Height { get; }
    public int Baseline { get; }
    private byte[] Array;
    private bool[] WhiteColumn;
    private int[] ColoredCountColumn;

    internal byte GetPixel(int x, int y)
    {
        return Array[x + y * Width];
    }

    internal void SetPixel(int x, int y, byte value)
    {
        Array[x + y * Width] = value;
    }

    internal void ClearPixel(int x, int y)
    {
        Array[x + y * Width] = 0xFF;
    }

    internal bool IsWhite(int x, int y)
    {
        return Array[x + y * Width] == 0xFF;
    }

    internal bool IsWhiteColumn(int x)
    {
        return WhiteColumn[x];
    }

    internal void SetWhiteColumn(int x, bool isWhite)
    {
        WhiteColumn[x] = isWhite;
    }

    internal int GetColoredCountColumn(int x)
    {
        return ColoredCountColumn[x];
    }

    internal void SetColoredCountColumn(int x, int coloredCount)
    {
        ColoredCountColumn[x] = coloredCount;
    }

    public bool IsColored(int x, int y, out byte color)
    {
        color = Array[x + y * Width];

        bool IsWhite = color == 0xFF;
        bool IsBlack = color == 0;
        bool Result = !IsWhite & !IsBlack;

        return Result;
    }

    public PixelArray Clipped()
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
                PixelArray Result = new(RightEdge - LeftEdge, BottomEdge - TopEdge, Baseline - TopEdge);

                for (int x = 0; x < Result.Width; x++)
                {
                    bool IsWhite = true;
                    int ColoredCount = 0;

                    for (int y = 0; y < Result.Height; y++)
                        PixelArrayHelper.CopyPixel(this, LeftEdge + x, TopEdge + y, Result, x, y, ref IsWhite, ref ColoredCount);

                    Result.WhiteColumn[x] = IsWhite;
                    Result.ColoredCountColumn[x] = ColoredCount;
                }

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
            bool Result = true;
            Result &= IsClippedColumn(0);
            Result &= IsClippedColumn(Width - 1);
            Result &= IsClippedRow(0);
            Result &= IsClippedRow(Height - 1);

            return Result;
        }
    }

    public bool IsClippedColumn(int column)
    {
        bool WhiteColumn = true;
        for (int y = 0; y < Height; y++)
            if (!IsWhite(column, y))
            {
                WhiteColumn = false;
                break;
            }

        return !WhiteColumn;
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

    public PixelArray GetLeftSide(int leftWidth)
    {
        PixelArray Result = new PixelArray(leftWidth, Height, Baseline);

        for (int x = 0; x < leftWidth; x++)
        {
            bool IsWhite = true;
            int ColoredCount = 0;

            for (int y = 0; y < Height; y++)
                PixelArrayHelper.CopyPixel(this, x, y, Result, x, y, ref IsWhite, ref ColoredCount);

            Result.WhiteColumn[x] = IsWhite;
            Result.ColoredCountColumn[x] = ColoredCount;
        }

        return Result;
    }

    public PixelArray GetRightSide(int rightWidth)
    {
        PixelArray Result = new PixelArray(rightWidth, Height, Baseline);

        for (int x = 0; x < rightWidth; x++)
        {
            bool IsWhite = true;
            int ColoredCount = 0;

            for (int y = 0; y < Height; y++)
                PixelArrayHelper.CopyPixel(this, Width - rightWidth + x, y, Result, x, y, ref IsWhite, ref ColoredCount);

            Result.WhiteColumn[x] = IsWhite;
            Result.ColoredCountColumn[x] = ColoredCount;
        }

        return Result;
    }

    public string GetDebugString()
    {
        string Result = string.Empty;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                uint RGB = Array[x + y * Width];
                uint Pixel = (((RGB >> 0) & 0xFF) + ((RGB >> 8) & 0xFF) + ((RGB >> 16) & 0xFF)) / 3;
                Result += Pixel < 0x40 ? "X" : (y == Baseline ? "." : " ");
            }

            Result += "\n";
        }

        return Result;
    }
}

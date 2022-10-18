namespace FontLoader;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;

[DebuggerDisplay("{_Width} x {Height}, {Baseline}")]
public class PixelArray
{
    public static readonly PixelArray Empty = new();
    public const double MaxSuportedDiffRatio = 0.2;

    private PixelArray()
    {
        _Width = 0;
        Height = 0;
        Baseline = 0;
        Array = new byte[0];
        WhiteColumn = new bool[0];
        ColoredCountColumn = new int[0];
        IsLoaded = false;
    }

    public PixelArray(int width, int height, int baseline)
    {
        Debug.Assert(width > 0);
        Debug.Assert(height > 0);
        _Width = width;
        Height = height;
        Baseline = baseline;
        Array = new byte[_Width * Height];
        WhiteColumn = new bool[_Width];
        ColoredCountColumn = new int[_Width];
        IsLoaded = true;
    }

    public PixelArray(LetterTypeBitmap sourceBitmap, int column, int row, int width, int height, int baseline, bool clearEdges, bool loadClipped)
        : this(width, height, baseline)
    {
        SourceBitmap = sourceBitmap;
        SourceColumn = column;
        SourceRow = row;
        SourceClearEdges = clearEdges;
        SourceLoadClipped = loadClipped;
        IsLoaded = false;
    }

    public PixelArray(byte[] argbValues, int stride, int left, int width, int top, int height, int baseline, bool clearEdges)
        : this(width, height, baseline)
    {
        Load(argbValues, stride, left, top, clearEdges);
    }

    public void Load(byte[] argbValues, int stride, int left, int top, bool clearEdge)
    {
        for (int x = 0; x < _Width; x++)
            WhiteColumn[x] = true;

        int ArrayOffset = 0;

        for (int y = 0; y < Height; y++)
        {
            int ArgbValuesOffset = ((top + y) * stride) + (left * 4);

            for (int x = 0; x < _Width; x++)
            {
                int B = argbValues[ArgbValuesOffset + 0];
                int G = argbValues[ArgbValuesOffset + 1];
                int R = argbValues[ArgbValuesOffset + 2];
                int Pixel = (clearEdge && (x == 0 || y == 0)) ? 0xFF : (R + G + B) / 3;

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

        IsLoaded = true;
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
        int BitmapWidth = bitmap.Width;
        int BitmapHeight = bitmap.Height;
        int Baseline = BitmapHeight - baselineDiff;
        Rectangle FullRect = new Rectangle(0, 0, BitmapWidth, BitmapHeight);
        BitmapData Data = bitmap.LockBits(FullRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        int Stride = Math.Abs(Data.Stride);
        int ByteCount = Data.Stride * BitmapHeight;
        byte[] ArgbValues = new byte[ByteCount];

        Marshal.Copy(Data.Scan0, ArgbValues, 0, ByteCount);
        bitmap.UnlockBits(Data);
        return new PixelArray(ArgbValues, Stride, rect.Left, rect.Width, rect.Top, rect.Height, Baseline, clearEdges: false);
    }

    public LetterTypeBitmap? SourceBitmap { get; }
    public int SourceColumn { get; }
    public int SourceRow { get; }
    public bool SourceClearEdges { get; }
    public bool SourceLoadClipped { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Width
    {
        get
        {
            CommitSource();
            return _Width;
        }
    }

    private int _Width;

    public int Height { get; private set; }
    public int Baseline { get; private set; }

    private byte[] Array;
    private bool[] WhiteColumn;
    private int[] ColoredCountColumn;
    private bool IsLoaded;

    public byte GetPixel(int x, int y)
    {
        Debug.Assert(IsLoaded);
        return Array[x + y * _Width];
    }

    internal void SetPixel(int x, int y, byte value)
    {
        Array[x + y * _Width] = value;
    }

    internal void ClearPixel(int x, int y)
    {
        Array[x + y * _Width] = 0xFF;
    }

    internal bool IsWhite(int x, int y)
    {
        return Array[x + y * _Width] == 0xFF;
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
        Debug.Assert(IsLoaded);
        color = Array[x + y * _Width];

        bool IsWhite = color == 0xFF;
        bool IsBlack = color == 0;
        bool Result = !IsWhite & !IsBlack;

        return Result;
    }

    public PixelArray Clipped()
    {
        CommitSource();
        GetClipZone(out int LeftEdge, out int TopEdge, out int RightEdge, out int BottomEdge);

        if (LeftEdge < RightEdge && TopEdge < BottomEdge)
        {
            if (LeftEdge > 0 || RightEdge < _Width || TopEdge > 0 || BottomEdge < Height)
            {
                return Clipped(LeftEdge, TopEdge, RightEdge, BottomEdge);
            }
            else
                return this;
        }
        else
            return Empty;
    }

    public void GetClipZone(out int leftEdge, out int topEdge, out int rightEdge, out int bottomEdge)
    {
        for (leftEdge = 0; leftEdge < _Width; leftEdge++)
        {
            bool IsEmptyColumn = true;

            for (int y = 0; y < Height; y++)
            {
                if (!IsWhite(leftEdge, y))
                {
                    IsEmptyColumn = false;
                    break;
                }
            }

            if (!IsEmptyColumn)
                break;
        }

        for (rightEdge = _Width; rightEdge > 0; rightEdge--)
        {
            bool IsEmptyColumn = true;

            for (int y = 0; y < Height; y++)
            {
                if (!IsWhite(rightEdge - 1, y))
                {
                    IsEmptyColumn = false;
                    break;
                }
            }

            if (!IsEmptyColumn)
                break;
        }

        for (topEdge = 0; topEdge < Height; topEdge++)
        {
            bool IsEmptyLine = true;

            for (int x = 0; x < _Width; x++)
            {
                if (!IsWhite(x, topEdge))
                {
                    IsEmptyLine = false;
                    break;
                }
            }

            if (!IsEmptyLine)
                break;
        }

        for (bottomEdge = Height; bottomEdge > 0; bottomEdge--)
        {
            bool IsEmptyLine = true;

            for (int x = 0; x < _Width; x++)
            {
                if (!IsWhite(x, bottomEdge - 1))
                {
                    IsEmptyLine = false;
                    break;
                }
            }

            if (!IsEmptyLine)
                break;
        }
    }

    public PixelArray Clipped(int leftEdge, int topEdge, int rightEdge, int bottomEdge)
    {
        PixelArray Result = new(rightEdge - leftEdge, bottomEdge - topEdge, Baseline - topEdge);

        for (int x = 0; x < Result.Width; x++)
        {
            bool IsWhite = true;
            int ColoredCount = 0;

            for (int y = 0; y < Result.Height; y++)
                PixelArrayHelper.CopyPixel(this, leftEdge + x, topEdge + y, Result, x, y, ref IsWhite, ref ColoredCount);

            Result.WhiteColumn[x] = IsWhite;
            Result.ColoredCountColumn[x] = ColoredCount;
        }

        return Result;
    }

    public bool IsClipped
    {
        get
        {
            if (IsLoaded)
            {
                bool Result = true;

                Result &= IsClippedColumn(0);
                Result &= IsClippedColumn(_Width - 1);
                Result &= IsClippedRow(0);
                Result &= IsClippedRow(Height - 1);
                return Result;
            }
            else
                return SourceLoadClipped;
        }
    }

    internal bool IsClippedColumn(int column)
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

    internal bool IsClippedRow(int row)
    {
        bool IsWhiteRow = true;

        for (int x = 0; x < _Width; x++)
            if (!IsWhite(x, row))
            {
                IsWhiteRow = false;
                break;
            }

        return !IsWhiteRow;
    }

    public PixelArray GetLeftSide(int leftWidth)
    {
        CommitSource();

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

        Debug.Assert(Result.IsLoaded);
        return Result;
    }

    public PixelArray GetRightSide(int rightWidth)
    {
        CommitSource();

        PixelArray Result = new PixelArray(rightWidth, Height, Baseline);

        for (int x = 0; x < rightWidth; x++)
        {
            bool IsWhite = true;
            int ColoredCount = 0;

            for (int y = 0; y < Height; y++)
                PixelArrayHelper.CopyPixel(this, _Width - rightWidth + x, y, Result, x, y, ref IsWhite, ref ColoredCount);

            Result.WhiteColumn[x] = IsWhite;
            Result.ColoredCountColumn[x] = ColoredCount;
        }

        Debug.Assert(Result.IsLoaded);
        return Result;
    }

    internal void CommitSource()
    {
        Debug.Assert(SourceBitmap is not null || IsLoaded);

        if (SourceBitmap is not null)
            if (!IsLoaded)
            {
                SourceBitmap.GetBitmapBytes(out byte[] ArgbValues, out int Stride);
                Debug.Assert(_Width == SourceBitmap.CellSize);
                Debug.Assert(Height == SourceBitmap.CellSize);
                Debug.Assert(Baseline == SourceBitmap.Baseline);
                Array = new byte[_Width * Height];
                WhiteColumn = new bool[_Width];
                ColoredCountColumn = new int[_Width];
                Load(ArgbValues, Stride, SourceColumn * SourceBitmap.CellSize, SourceRow * SourceBitmap.CellSize, SourceClearEdges);

                if (SourceLoadClipped)
                {
                    PixelArray Modified = Clipped();

                    _Width = Modified._Width;
                    Height = Modified.Height;
                    Baseline = Modified.Baseline;
                    Array = Modified.Array;
                    WhiteColumn = Modified.WhiteColumn;
                    ColoredCountColumn = Modified.ColoredCountColumn;
                }
            }
    }

    public string GetDebugString()
    {
        CommitSource();

        string Result = string.Empty;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < _Width; x++)
            {
                uint RGB = Array[x + y * _Width];
                uint Pixel = (((RGB >> 0) & 0xFF) + ((RGB >> 8) & 0xFF) + ((RGB >> 16) & 0xFF)) / 3;

                Result += Pixel < 0x40 ? "X" : (y == Baseline ? "." : " ");
            }

            Result += "\n";
        }

        return Result;
    }
}

namespace FontLoader;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class FontBitmap : IDisposable
{
    #region Init
    public FontBitmap(FontBitmapStream sourceStream)
    {
        SourceStream = sourceStream;
    }
    #endregion

    #region Properties
    public FontBitmapStream SourceStream { get; }
    public double FontSize { get { return SourceStream.FontSize; } }
    public bool IsLoaded { get { return SourceStream is not null; } }
    #endregion

    #region Client Interface
    public Bitmap LoadBitmap()
    {
        if (LoadedBitmap is null)
        {
            Stream Stream = SourceStream.LoadStream();
            LoadedBitmap = new Bitmap(Stream);

            int Width = LoadedBitmap.Width;
            int CellSize = Width / FontBitmapCollection.DefaultColumns;

            int ExpectedCellSize = Font.FontSizeToCellSize(FontSize);
            Debug.Assert(ExpectedCellSize == CellSize);

            int ExpectedWidth = CellSize * FontBitmapCollection.DefaultColumns;
            Debug.Assert(ExpectedWidth == Width);
        }

        Bitmap? CheckedBitmap = LoadedBitmap;
        Debug.Assert(CheckedBitmap is not null);

        return LoadedBitmap;
    }

    public void GetBitmapBytes(out byte[] argbValues, out int stride, out int width, out int height)
    {
        Bitmap Bitmap = LoadBitmap();
        width = Bitmap.Width;
        height = Bitmap.Height;
        Rectangle Rect = new Rectangle(0, 0, width, height);
        BitmapData Data = Bitmap.LockBits(Rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        stride = Math.Abs(Data.Stride);

        int ByteCount = stride * height;
        argbValues = new byte[ByteCount];

        // Copy the RGB values into the array.
        Marshal.Copy(Data.Scan0, argbValues, 0, ByteCount);

        Bitmap.UnlockBits(Data);
    }

    private Bitmap? LoadedBitmap;
    #endregion

    #region Implementation of IDisposable
    public void Dispose()
    {
        if (LoadedBitmap is not null)
        {
            LoadedBitmap.Dispose();
            LoadedBitmap = null;
        }
    }
    #endregion
}

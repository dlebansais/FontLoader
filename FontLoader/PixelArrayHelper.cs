namespace FontLoader;

using System;
using System.Diagnostics;

public static partial class PixelArrayHelper
{
    public static double MaxSuportedDiffRatio { get; set; } = 0.2;
    public static double MaxSoftTakenPixelCount { get; set; } = 14;

    internal static void CopyPixel(PixelArray p1, int x1, int y1, PixelArray p2, int x2, int y2, ref bool isWhite, ref int coloredCount)
    {
        byte Pixel = p1.GetPixel(x1, y1);

        p2.SetPixel(x2, y2, Pixel);
        UpdatePixelFlags(Pixel, ref isWhite, ref coloredCount);
    }

    internal static void MixPixel(PixelArray p, int x, int y, PixelArray p1, int x1, int y1, PixelArray p2, int x2, int y2, ref bool isWhite, ref int coloredCount)
    {
        byte Pixel1 = p1.GetPixel(x1, y1);
        byte Pixel2 = p2.GetPixel(x2, y2);
        byte Pixel;

        if (Pixel1 != 0xFF && Pixel2 != 0xFF)
            Pixel = (byte)((Pixel1 * Pixel2) / 255);
        else
            Pixel = (byte)(Pixel1 + Pixel2 - 0xFF);

        p.SetPixel(x, y, Pixel);
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

    private static void MergePixel(PixelArray Result, PixelArray p1, PixelArray p2, int x, int y, int TotalWidth, int Baseline, int offsetY, ref bool isWhite, ref int coloredCount)
    {
        Debug.Assert(x >= 0);
        Debug.Assert(y >= 0);

        if (x < p1.Width && x >= TotalWidth - p2.Width)
        {
            int OffsetY1 = Baseline - p1.Baseline;
            int OffsetY2 = Baseline - p2.Baseline - offsetY;

            bool IsMixedPixel = true;
            IsMixedPixel &= y >= OffsetY1;
            IsMixedPixel &= y < OffsetY1 + p1.Height;
            IsMixedPixel &= y >= OffsetY2;
            IsMixedPixel &= y < OffsetY2 + p2.Height;

            if (IsMixedPixel)
            {
                MixPixel(Result, x, y, p1, x, y - OffsetY1, p2, x - TotalWidth + p2.Width, y - OffsetY2, ref isWhite, ref coloredCount);
                return;
            }
        }

        if (x < p1.Width)
        {
            int OffsetY = Baseline - p1.Baseline;

            bool IsPixelFromArray1 = true;
            IsPixelFromArray1 &= y >= OffsetY;
            IsPixelFromArray1 &= y < OffsetY + p1.Height;

            if (IsPixelFromArray1)
            {
                CopyPixel(p1, x, y - OffsetY, Result, x, y, ref isWhite, ref coloredCount);
                return;
            }
        }

        if (x >= TotalWidth - p2.Width)
        {
            int OffsetY = Baseline - p2.Baseline - offsetY;

            bool IsPixelFromArray2 = true;
            IsPixelFromArray2 &= y >= OffsetY;
            IsPixelFromArray2 &= y < OffsetY + p2.Height;

            if (IsPixelFromArray2)
            {
                CopyPixel(p2, x - TotalWidth + p2.Width, y - OffsetY, Result, x, y, ref isWhite, ref coloredCount);
                return;
            }
        }

        Result.ClearPixel(x, y);
    }

    private static bool IsWhiteColumn(PixelArray p, int x)
    {
        for (int y = 0; y < p.Height; y++)
            if (!p.IsWhite(x, y))
            {
                Debug.Assert(!p.IsWhiteColumn(x));
                return false;
            }

        Debug.Assert(p.IsWhiteColumn(x));
        return true;
    }

    private static bool IsMatchPixed(PixelArray p1, PixelArray p2, int x, int y, ref int diffTotal)
    {
        uint RGB1 = p1.GetPixel(x, y);
        uint RGB2 = p2.GetPixel(x, y);

        return IsMatchPixedValue(RGB1, RGB2, ref diffTotal);
    }

    private static bool IsLeftMatchPixed(PixelArray p1, PixelArray p2, int x, int y1, int y2, ref int diffTotal)
    {
        uint RGB1 = p1.GetPixel(x, y1);
        uint RGB2 = p2.GetPixel(x, y2);

        return IsMatchPixedValue(RGB1, RGB2, ref diffTotal);
    }

    private static bool IsRightMatchPixed(PixelArray p1, PixelArray p2, int x, int y1, int y2, ref int diffTotal)
    {
        uint RGB1 = p1.GetPixel(p1.Width - x - 1, y1);
        uint RGB2 = p2.GetPixel(p2.Width - x - 1, y2);

        return IsMatchPixedValue(RGB1, RGB2, ref diffTotal);
    }

    private static bool IsMatchPixedValue(uint rgb1, uint rgb2, ref int diffTotal)
    {
        int Diff = Math.Abs((int)rgb1 - (int)rgb2);

        diffTotal += Diff;
        return Diff <= 5;
    }
}

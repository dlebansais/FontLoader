namespace FontLoader;

using System;

public static partial class PixelArrayHelper
{
    public static bool IsLeftDiagonalMatch(PixelArray p1, double perfectMatchRatio, int rightOverlapWidth, PixelArray p2, int verticalOffset)
    {
        p1.CommitSource();
        p2.CommitSource();

        int Baseline1 = p1.Baseline;
        int Baseline2 = p2.Baseline + verticalOffset;
        int Width = Math.Max(p1.Width, p2.Width);
        int Baseline = Math.Max(Baseline1, Baseline2);
        int Height = Baseline + Math.Max(p1.Height - Baseline1, p2.Height - Baseline2);
        int DiffTotal = 0;
        int MaxSupportedDiff = (int)((Width * Height) * MaxSuportedDiffRatio);
        bool[,] PixelSoftTaken = new bool[Width, Height];
        bool[,] PixelHardTaken = new bool[Width, Height];
        int PerfectMatchWidth = (int)(Width * (1.0 - perfectMatchRatio));

        InitializeTakenPixels(p1, rightOverlapWidth, Width, Height, Baseline, Baseline1, PerfectMatchWidth, PixelSoftTaken, PixelHardTaken);

        int SoftTakenPixelCount = 0;

        for (int x = 0; x < Width; x++)
        {
            if (x < p1.Width && x < p2.Width)
            {
                if (!IsLeftDiagonalMatchColumn(p1, p2, x, Baseline, Baseline1, Baseline2, Height, MaxSupportedDiff, PixelSoftTaken, PixelHardTaken, ref DiffTotal, ref SoftTakenPixelCount))
                    return false;
            }
            else if (x < p1.Width)
            {
                if (!IsWhiteColumn(p1, x))
                    return false;
            }
        }

        return true;
    }

    private static void InitializeTakenPixels(PixelArray p1, int rightOverlapWidth, int width, int height, int baseline, int baseline1, int perfectMatchWidth, bool[,] pixelSoftTaken, bool[,] pixelHardTaken)
    {
        for (int y = 0; y < height; y++)
        {
            int x;

            for (x = 0; x < perfectMatchWidth; x++)
            {
                int x1 = width - x - 1;
                int y1 = y - baseline + baseline1;

                if (x1 < p1.Width && y1 >= 0 && y1 < p1.Height)
                    if (!p1.IsWhite(x1, y1))
                        break;
            }

            for (int x2 = x; x2 < width; x2++)
                pixelSoftTaken[width - x2 - 1, y] = true;

            for (int x2 = x; x2 + rightOverlapWidth < width; x2++)
                pixelHardTaken[width - x2 - rightOverlapWidth - 1, y] = true;
        }
    }

    private static bool IsLeftDiagonalMatchColumn(PixelArray p1, PixelArray p2, int x, int baseline, int baseline1, int baseline2, int height, int maxSupportedDiff, bool[,] pixelSoftTaken, bool[,] pixelHardTaken, ref int diffTotal, ref int softTakenPixelCount)
    {
        for (int y = 0; y < height; y++)
            if (IsLeftDiagonalMatchPixelDifferent(p1, p2, x, y, baseline, baseline1, baseline2, maxSupportedDiff, pixelSoftTaken, ref diffTotal))
            {
                if (pixelHardTaken[x, y] || softTakenPixelCount >= MaxSoftTakenPixelCount)
                    return false;

                softTakenPixelCount++;
            }

        return true;
    }

    private static bool IsLeftDiagonalMatchPixelDifferent(PixelArray p1, PixelArray p2, int x, int y, int baseline, int baseline1, int baseline2, int maxSupportedDiff, bool[,] pixelSoftTaken, ref int diffTotal)
    {
        int y1 = y - baseline + baseline1;
        int y2 = y - baseline + baseline2;

        bool IsComparable = true;
        IsComparable &= y1 >= 0;
        IsComparable &= y1 < p1.Height;
        IsComparable &= y2 >= 0;
        IsComparable &= y2 < p2.Height;

        if (IsComparable)
            return IsLeftDiagonalMatchPixelComparableDifferent(p1, p2, x, y, y1, y2, maxSupportedDiff, pixelSoftTaken, ref diffTotal);
        else
            return IsLeftDiagonalMatchPixelNotComparableDifferent(p1, p2, x, y, y1, y2, pixelSoftTaken);
    }

    private static bool IsLeftDiagonalMatchPixelComparableDifferent(PixelArray p1, PixelArray p2, int x, int y, int y1, int y2, int maxSupportedDiff, bool[,] pixelSoftTaken, ref int diffTotal)
    {
        bool IsDifferent = false;

        if (pixelSoftTaken[x, y])
        {
            if (!IsLeftMatchPixed(p1, p2, x, y1, y2, ref diffTotal))
                IsDifferent = true;
            else if (diffTotal > maxSupportedDiff)
                IsDifferent = true;
        }

        return IsDifferent;
    }

    private static bool IsLeftDiagonalMatchPixelNotComparableDifferent(PixelArray p1, PixelArray p2, int x, int y, int y1, int y2, bool[,] pixelSoftTaken)
    {
        bool IsDifferent = false;

        if (y1 >= 0 && y1 < p1.Height)
        {
            if (!p1.IsWhite(x, y1))
                IsDifferent = true;
        }
        else if (y1 < 0)
        {
        }
        else if (y1 >= p1.Height)
        {
        }

        if (y2 >= 0 && y2 < p2.Height)
        {
            if (pixelSoftTaken[x, y] && !p2.IsWhite(x, y2))
                IsDifferent = true;
        }
        else if (y2 < 0)
        {
        }
        else if (y2 >= p2.Height)
        {
        }

        return IsDifferent;
    }
}

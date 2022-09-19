namespace FontLoader;

using System;
using System.Diagnostics;

public static class PixelArrayHelper
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

            if (y >= OffsetY1 && y < OffsetY1 + p1.Height && y >= OffsetY2 && y < OffsetY2 + p2.Height)
            {
                MixPixel(Result, x, y, p1, x, y - OffsetY1, p2, x - TotalWidth + p2.Width, y - OffsetY2, ref isWhite, ref coloredCount);
                return;
            }
        }

        if (x < p1.Width)
        {
            int OffsetY = Baseline - p1.Baseline;

            if (y >= OffsetY && y < OffsetY + p1.Height)
            {
                CopyPixel(p1, x, y - OffsetY, Result, x, y, ref isWhite, ref coloredCount);
                return;
            }
        }

        if (x >= TotalWidth - p2.Width)
        {
            int OffsetY = Baseline - p2.Baseline - offsetY;

            if (y >= OffsetY && y < OffsetY + p2.Height)
            {
                CopyPixel(p2, x - TotalWidth + p2.Width, y - OffsetY, Result, x, y, ref isWhite, ref coloredCount);
                return;
            }
        }

        Result.ClearPixel(x, y);
    }

    public static bool IsMatch(PixelArray p1, PixelArray p2, int verticalOffset)
    {
        Debug.Assert(p1.IsClipped);
        Debug.Assert(p2.IsClipped);

        if (p1.Width != p2.Width || p1.Height != p2.Height)
            return false;

        int Width = p1.Width;
        int Height = p1.Height;

        int BaselineDifference = p2.Baseline - p1.Baseline + verticalOffset;
        if (BaselineDifference != 0)
            return false;

        int DiffTotal = 0;
        //int MaxSupportedDiff = (int)((Width * Height) * MaxSuportedDiffRatio);
        int MaxSupportedDiff = 5;

        p1.CommitSource();
        p2.CommitSource();

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                if (!IsMatchPixed(p1, p2, x, y, ref DiffTotal))
                    return false;

                if (DiffTotal > MaxSupportedDiff)
                    return false;
            }

        return true;
    }

    public static bool IsPixelToPixelMatch(PixelArray p1, PixelArray p2)
    {
        if (p1.Width != p2.Width || p1.Height != p2.Height)
            return false;

        int Width = p1.Width;
        int Height = p1.Height;
        int DiffTotal = 0;

        p1.CommitSource();
        p2.CommitSource();

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                if (!IsMatchPixed(p1, p2, x, y, ref DiffTotal))
                    return false;

                if (DiffTotal > 0)
                    return false;
            }

        return true;
    }

    public static bool IsLeftMatch(PixelArray p1, PixelArray p2, int verticalOffset, out int firstDiffX)
    {
        int Baseline1 = p1.Baseline;
        int Baseline2 = p2.Baseline + verticalOffset;

        int Width = Math.Max(p1.Width, p2.Width);
        int Baseline = Math.Max(Baseline1, Baseline2);
        int Height = Baseline + Math.Max(p1.Height - Baseline1, p2.Height - Baseline2);

        int DiffTotal = 0;
        //int MaxSupportedDiff = (int)((Width * Height) * MaxSuportedDiffRatio);
        int MaxSupportedDiff = 5;

        p1.CommitSource();
        p2.CommitSource();

        firstDiffX = -1;

        for (int x = 0; x < Width; x++)
        {
            if (x < p1.Width && x < p2.Width)
            {
                for (int y = 0; y < Height; y++)
                {
                    int y1 = y - Baseline + Baseline1;
                    int y2 = y - Baseline + Baseline2;

                    if (y1 >= 0 && y2 >= 0 && y1 < p1.Height && y2 < p2.Height)
                    {
                        if (!IsLeftMatchPixed(p1, p2, x, y1, y2, ref DiffTotal))
                            return false;

                        if (DiffTotal > MaxSupportedDiff)
                            return false;

                        if (DiffTotal > 0 && firstDiffX < 0)
                            firstDiffX = x;
                    }
                    else
                    {
                        if (y1 >= 0 && y1 < p1.Height)
                            if (!p1.IsWhite(x, y1))
                                return false;

                        if (y2 >= 0 && y2 < p2.Height)
                            if (!p2.IsWhite(x, y2))
                                return false;
                    }
                }
            }
            else if (x < p1.Width)
            {
                for (int y = 0; y < p1.Height; y++)
                    if (!p1.IsWhite(x, y))
                    {
                        Debug.Assert(!p1.IsWhiteColumn(x));
                        return false;
                    }

                Debug.Assert(p1.IsWhiteColumn(x));
            }
        }

        return true;
    }

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

        for (int y = 0; y < Height; y++)
        {
            int x;

            for (x = 0; x < PerfectMatchWidth; x++)
            {
                int x1 = Width - x - 1;
                int y1 = y - Baseline + Baseline1;

                if (x1 < p1.Width && y1 >= 0 && y1 < p1.Height && !p1.IsWhite(x1, y1))
                    break;
            }

            for (int x2 = x; x2 < Width; x2++)
                PixelSoftTaken[Width - x2 - 1, y] = true;

            for (int x2 = x; x2 + rightOverlapWidth < Width; x2++)
                PixelHardTaken[Width - x2 - rightOverlapWidth - 1, y] = true;
        }

        int SoftTakenPixelCount = 0;

        for (int x = 0; x < Width; x++)
        {
            if (x < p1.Width && x < p2.Width)
            {
                for (int y = 0; y < Height; y++)
                {
                    int y1 = y - Baseline + Baseline1;
                    int y2 = y - Baseline + Baseline2;
                    bool IsDifferent = false;

                    if (y1 >= 0 && y2 >= 0 && y1 < p1.Height && y2 < p2.Height)
                    {
                        if (PixelSoftTaken[x, y])
                        {
                            if (!IsLeftMatchPixed(p1, p2, x, y1, y2, ref DiffTotal))
                                IsDifferent = true;
                            else if (DiffTotal > MaxSupportedDiff)
                                IsDifferent = true;
                        }
                    }
                    else
                    {
                        if (y1 >= 0 && y1 < p1.Height && !p1.IsWhite(x, y1))
                            IsDifferent = true;

                        if (y2 >= 0 && y2 < p2.Height && PixelSoftTaken[x, y] && !p2.IsWhite(x, y2))
                            IsDifferent = true;
                    }

                    if (IsDifferent)
                    {
                        if (PixelHardTaken[x, y] || SoftTakenPixelCount >= MaxSoftTakenPixelCount)
                            return false;

                        SoftTakenPixelCount++;
                    }
                }
            }
            else if (x < p1.Width)
            {
                for (int y = 0; y < p1.Height; y++)
                    if (!p1.IsWhite(x, y))
                    {
                        Debug.Assert(!p1.IsWhiteColumn(x));
                        return false;
                    }

                Debug.Assert(p1.IsWhiteColumn(x));
            }
        }

        return true;
    }

    public static bool IsRightMatch(PixelArray p1, PixelArray p2, int verticalOffset)
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

        for (int x = 0; x < Width; x++)
        {
            if (x < p1.Width && x < p2.Width)
            {
                for (int y = 0; y < Height; y++)
                {
                    int y1 = y - Baseline + Baseline1;
                    int y2 = y - Baseline + Baseline2;

                    if (y1 >= 0 && y2 >= 0 && y1 < p1.Height && y2 < p2.Height)
                    {
                        if (!IsRightMatchPixed(p1, p2, x, y1, y2, ref DiffTotal))
                            return false;

                        if (DiffTotal > MaxSupportedDiff)
                            return false;
                    }
                    else
                    {
                        if (y1 >= 0 && y1 < p1.Height)
                            if (!p1.IsWhite(p1.Width - x - 1, y1))
                                return false;

                        if (y2 >= 0 && y2 < p2.Height)
                            if (!p2.IsWhite(p2.Width - x - 1, y2))
                                return false;
                    }
                }

                Debug.Assert(p1.GetColoredCountColumn(p1.Width - x - 1) == p2.GetColoredCountColumn(p2.Width - x - 1));
            }
            else if (x < p1.Width)
            {
                for (int y = 0; y < p1.Height; y++)
                {
                    if (!p1.IsWhite(p1.Width - x - 1, y))
                    {
                        Debug.Assert(!p1.IsWhiteColumn(p1.Width - x - 1));
                        return false;
                    }
                }

                Debug.Assert(p1.IsWhiteColumn(p1.Width - x - 1));
            }
        }

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

    public static bool IsCompatible(PixelArray p1, PixelArray p2, int testWidth)
    {
        Debug.Assert(p1.Width >= testWidth);
        Debug.Assert(p2.Width >= testWidth);

        p1.CommitSource();
        p2.CommitSource();

        for (int x = 0; x < testWidth; x++)
            if (p1.IsWhiteColumn(x) != p2.IsWhiteColumn(x) || p1.GetColoredCountColumn(x) != p2.GetColoredCountColumn(x))
                return false;

        return true;
    }

    public static PixelArray Merge(PixelArray p1, int offsetY, PixelArray p2, int inside)
    {
        p1.CommitSource();
        p2.CommitSource();

        return Merge(p1, offsetY, p2, inside, p1.Width - inside + p2.Width);
    }

    public static PixelArray Merge(PixelArray p1, int offsetY, PixelArray p2, int inside, int maxWidth)
    {
        p1.CommitSource();
        p2.CommitSource();

        int Baseline = Math.Max(p1.Baseline, p2.Baseline + offsetY);
        int TotalWidth = p1.Width - inside + p2.Width;
        int Width = Math.Min(p1.Width - inside + p2.Width, maxWidth);
        int Height = Baseline + Math.Max(p1.Height - p1.Baseline, p2.Height - p2.Baseline - offsetY);

        PixelArray Result = new PixelArray(Width, Height, Baseline);

        for (int x = 0; x < Width; x++)
        {
            bool IsWhite = true;
            int ColoredCount = 0;

            for (int y = 0; y < Height; y++)
                MergePixel(Result, p1, p2, x, y, TotalWidth, Baseline, offsetY, ref IsWhite, ref ColoredCount);

            Result.SetWhiteColumn(x, IsWhite);
            Result.SetColoredCountColumn(x, ColoredCount);
        }

        return Result;
    }

    public static PixelArray CutRight(PixelArray p1, int endCutoff)
    {
        p1.CommitSource();

        if (endCutoff <= 0 || p1.Width <= endCutoff * 2)
            return p1;

        PixelArray Result = new PixelArray(p1.Width - endCutoff, p1.Height, p1.Baseline);

        for (int x = 0; x < Result.Width; x++)
        {
            bool IsWhite = true;
            int ColoredCount = 0;

            for (int y = 0; y < p1.Height; y++)
                CopyPixel(p1, x, y, Result, x, y, ref IsWhite, ref ColoredCount);

            Result.SetWhiteColumn(x, IsWhite);
            Result.SetColoredCountColumn(x, ColoredCount);
        }

        return Result;
    }

    public static PixelArray CutLeft(PixelArray p1, int startCutoff)
    {
        p1.CommitSource();

        if (startCutoff <= 0 || p1.Width <= startCutoff * 2)
            return p1;

        PixelArray Result = new PixelArray(p1.Width - startCutoff, p1.Height, p1.Baseline);

        for (int x = 0; x < Result.Width; x++)
        {
            bool IsWhite = true;
            int ColoredCount = 0;

            for (int y = 0; y < p1.Height; y++)
                CopyPixel(p1, x + startCutoff, y, Result, x, y, ref IsWhite, ref ColoredCount);

            Result.SetWhiteColumn(x, IsWhite);
            Result.SetColoredCountColumn(x, ColoredCount);
        }

        return Result;
    }

    public static PixelArray Enlarge(PixelArray p1, PixelArray p2)
    {
        if (p1.Baseline >= p2.Baseline && p1.Height - p1.Baseline >= p2.Height - p2.Baseline)
            return p1;

        p1.CommitSource();
        p2.CommitSource();

        int TopHeight = Math.Max(p2.Baseline - p1.Baseline, 0);
        int BottomHeight = Math.Max((p2.Height - p2.Baseline) - (p1.Height - p1.Baseline), 0);
        Debug.Assert(TopHeight > 0 || BottomHeight > 0);

        PixelArray Result = new PixelArray(p1.Width, TopHeight + p1.Height + BottomHeight, p1.Baseline + TopHeight);

        for (int x = 0; x < p1.Width; x++)
        {
            bool IsWhite = true;
            int ColoredCount = 0;

            for (int y = 0; y < TopHeight; y++)
                Result.ClearPixel(x, y);

            for (int y = 0; y < p1.Height; y++)
                CopyPixel(p1, x, y, Result, x, TopHeight + y, ref IsWhite, ref ColoredCount);

            for (int y = 0; y < BottomHeight; y++)
                Result.ClearPixel(x, TopHeight + p1.Height + y);

            Result.SetWhiteColumn(x, IsWhite);
            Result.SetColoredCountColumn(x, ColoredCount);
        }

        return Result;
    }
}

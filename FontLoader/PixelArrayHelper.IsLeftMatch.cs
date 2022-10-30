namespace FontLoader;

using System;

public static partial class PixelArrayHelper
{
    public static bool IsLeftMatch(PixelArray p1, PixelArray p2, int verticalOffset, out int firstDiffX)
    {
        int Baseline1 = p1.Baseline;
        int Baseline2 = p2.Baseline + verticalOffset;
        int Width = Math.Max(p1.Width, p2.Width);
        int Baseline = Math.Max(Baseline1, Baseline2);
        int Height = Baseline + Math.Max(p1.Height - Baseline1, p2.Height - Baseline2);
        int DiffTotal = 0;
        int MaxSupportedDiff = 5;

        p1.CommitSource();
        p2.CommitSource();
        firstDiffX = -1;

        for (int x = 0; x < Width; x++)
        {
            if (x < p1.Width && x < p2.Width)
            {
                if (!IsLeftMatchColumn(p1, p2, x, Baseline, Baseline1, Baseline2, Height, MaxSupportedDiff, ref DiffTotal, ref firstDiffX))
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

    private static bool IsLeftMatchColumn(PixelArray p1, PixelArray p2, int x, int baseline, int baseline1, int baseline2, int height, int maxSupportedDiff, ref int diffTotal, ref int firstDiffX)
    {
        for (int y = 0; y < height; y++)
            if (!IsLeftMatchPixel(p1, p2, x, y, baseline, baseline1, baseline2, maxSupportedDiff, ref diffTotal, ref firstDiffX))
                return false;

        return true;
    }

    private static bool IsLeftMatchPixel(PixelArray p1, PixelArray p2, int x, int y, int baseline, int baseline1, int baseline2, int maxSupportedDiff, ref int diffTotal, ref int firstDiffX)
    {
        int y1 = y - baseline + baseline1;
        int y2 = y - baseline + baseline2;

        if (y1 >= 0 && y2 >= 0 && y1 < p1.Height && y2 < p2.Height)
        {
            if (!IsLeftMatchPixed(p1, p2, x, y1, y2, ref diffTotal))
                return false;

            if (diffTotal > maxSupportedDiff)
                return false;

            if (diffTotal > 0 && firstDiffX < 0)
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

        return true;
    }
}

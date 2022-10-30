namespace FontLoader;

using System;
using System.Diagnostics;

public static partial class PixelArrayHelper
{
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
                if (!IsRightMatchColumn(p1, p2, x, Baseline, Baseline1, Baseline2, Height, MaxSupportedDiff, ref DiffTotal))
                    return false;
            }
            else if (x < p1.Width)
            {
                if (!IsWhiteColumn(p1, p1.Width - x - 1))
                    return false;
            }
        }

        return true;
    }

    private static bool IsRightMatchColumn(PixelArray p1, PixelArray p2, int x, int baseline, int baseline1, int baseline2, int height, int maxSupportedDiff, ref int diffTotal)
    {
        for (int y = 0; y < height; y++)
        {
            int y1 = y - baseline + baseline1;
            int y2 = y - baseline + baseline2;

            if (y1 >= 0 && y2 >= 0 && y1 < p1.Height && y2 < p2.Height)
            {
                if (!IsRightMatchPixed(p1, p2, x, y1, y2, ref diffTotal))
                    return false;

                if (diffTotal > maxSupportedDiff)
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

        return true;
    }
}

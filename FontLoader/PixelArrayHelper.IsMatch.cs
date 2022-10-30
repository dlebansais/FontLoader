namespace FontLoader;

using System.Diagnostics;

public static partial class PixelArrayHelper
{
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
}

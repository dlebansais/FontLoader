namespace FontLoader;

using System;

public static partial class PixelArrayHelper
{
    public static PixelArray Merge(PixelArray p1, int offsetY, PixelArray p2, int inside)
    {
        p1.CommitSource();
        p2.CommitSource();
        return MergeCommitted(p1, offsetY, p2, inside, p1.Width - inside + p2.Width);
    }

    public static PixelArray Merge(PixelArray p1, int offsetY, PixelArray p2, int inside, int maxWidth)
    {
        p1.CommitSource();
        p2.CommitSource();
        return MergeCommitted(p1, offsetY, p2, inside, maxWidth);
    }

    private static PixelArray MergeCommitted(PixelArray p1, int offsetY, PixelArray p2, int inside, int maxWidth)
    {
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
}

namespace FontLoader;

using System;
using System.Diagnostics;

public static partial class PixelArrayHelper
{
    public static PixelArray Replace(PixelArray p1, PixelArray p2, int verticalOffset)
    {
        p1.CommitSource();
        p2.CommitSource();
        Debug.Assert(p1.Width <= p2.Width);

        int Baseline = Math.Max(p1.Baseline, p2.Baseline - verticalOffset);
        int Width = p2.Width;
        int Height = Baseline + Math.Max(p1.Height - p1.Baseline, p2.Height - p2.Baseline + verticalOffset);
        PixelArray Result = new PixelArray(Width, Height, Baseline + verticalOffset);

        for (int x = 0; x < Width; x++)
        {
            bool IsWhite = true;
            int ColoredCount = 0;

            for (int y = 0; y < Height; y++)
            {
                int y1 = y - Baseline + p1.Baseline;
                int y2 = y - Baseline + p2.Baseline - verticalOffset;

                if (x < p1.Width && y1 >= 0 && y1 < p1.Height)
                    CopyPixel(p1, x, y1, Result, x, y, ref IsWhite, ref ColoredCount);
                else if (y2 >= 0 && y2 < p2.Height)
                    CopyPixel(p2, x, y2, Result, x, y, ref IsWhite, ref ColoredCount);
                else
                    Result.ClearPixel(x, y);
            }

            Result.SetWhiteColumn(x, IsWhite);
            Result.SetColoredCountColumn(x, ColoredCount);
        }

        return Result;
    }
}

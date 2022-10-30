namespace FontLoader;

using System;
using System.Diagnostics;

public static partial class PixelArrayHelper
{
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

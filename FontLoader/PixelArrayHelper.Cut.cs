namespace FontLoader;

public static partial class PixelArrayHelper
{
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
}

namespace FontLoader;

public static partial class PixelArrayHelper
{
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
}

namespace FontLoader;

using System.Diagnostics;

public static partial class PixelArrayHelper
{
    public static bool IsCompatible(PixelArray p1, PixelArray p2, int testWidth)
    {
        Debug.Assert(p1.Width >= testWidth);
        Debug.Assert(p2.Width >= testWidth);
        p1.CommitSource();
        p2.CommitSource();

        for (int x = 0; x < testWidth; x++)
        {
            bool IsColumnDifferent = false;
            IsColumnDifferent |= p1.IsWhiteColumn(x) != p2.IsWhiteColumn(x);
            IsColumnDifferent |= p1.GetColoredCountColumn(x) != p2.GetColoredCountColumn(x);

            if (IsColumnDifferent)
                return false;
        }

        return true;
    }
}

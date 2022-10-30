namespace FontLoader;

using System;
using System.Diagnostics;

public static partial class PixelArrayHelper
{
    public static double Distance(PixelArray p1, PixelArray p2, int separation)
    {
        Debug.Assert(separation >= 0);

        int MinY = Math.Min(-p1.Baseline, -p2.Baseline);
        int MaxY = 0;
        int ComparisonHeight = MaxY - MinY;

        InitializePositionAndIntensity(p1, p2, ComparisonHeight, out HorizontalDistance[] LeftDistances, out HorizontalDistance[] RightDistances);
        MeasureDistances(p1, p2, ComparisonHeight, MinY, LeftDistances, RightDistances);

        double Result = MeasureDistanceMin(p1, p2, ComparisonHeight, LeftDistances, RightDistances, separation);

        return Result;
    }

    private static void InitializePositionAndIntensity(PixelArray p1, PixelArray p2, int comparisonHeight, out HorizontalDistance[] leftDistances, out HorizontalDistance[] rightDistances)
    {
        leftDistances = new HorizontalDistance[comparisonHeight];
        rightDistances = new HorizontalDistance[comparisonHeight];

        for (int y = 0; y < comparisonHeight; y++)
        {
            leftDistances[y] = new HorizontalDistance(p1.Width, 0);
            rightDistances[y] = new HorizontalDistance(p2.Width - 1, 0);
        }
    }

    private static void MeasureDistances(PixelArray p1, PixelArray p2, int comparisonHeight, int minY, HorizontalDistance[] leftDistances, HorizontalDistance[] rightDistances)
    {
        for (int y = 0; y < comparisonHeight; y++)
        {
            int y1 = y + minY + p1.Baseline;

            if (y1 >= 0 && y1 < p1.Height)
            {
                int Left = 0;

                while (Left < p1.Width && p1.IsWhite(p1.Width - 1 - Left, y1))
                    Left++;

                byte Intensity = Left < p1.Width ? p1.GetPixel(p1.Width - 1 - Left, y1) : (byte)0xFF;

                leftDistances[y] = leftDistances[y] with { ColoredPixelPosition = Left, ColoredPixelIntensity = Intensity };
            }

            int y2 = y + minY + p2.Baseline;

            if (y2 >= 0 && y2 < p2.Height)
            {
                int Right = 0;

                while (Right < p2.Width && p2.IsWhite(Right, y2))
                    Right++;

                byte Intensity = Right < p2.Width ? p2.GetPixel(Right, y2) : (byte)0xFF;

                rightDistances[y] = rightDistances[y] with { ColoredPixelPosition = Right, ColoredPixelIntensity = Intensity };
            }
        }
    }

    private static double MeasureDistanceMin(PixelArray p1, PixelArray p2, int comparisonHeight, HorizontalDistance[] leftDistances, HorizontalDistance[] rightDistances, int separation)
    {
        double[,] SquareDistances = new double[comparisonHeight, comparisonHeight];

        for (int i = 0; i < comparisonHeight; i++)
            for (int j = 0; j < comparisonHeight; j++)
            {
                double X1 = p1.Width - leftDistances[i].Distance;
                double Y1 = i;
                double X2 = p1.Width + separation + rightDistances[j].Distance;
                double Y2 = j;

                SquareDistances[i, j] = Square(X2 - X1) + Square(Y2 - Y1);
            }

        double MinSquareDistance = double.PositiveInfinity;

        for (int i = 0; i < comparisonHeight; i++)
            for (int j = 0; j < comparisonHeight; j++)
                if (MinSquareDistance > SquareDistances[i, j])
                    MinSquareDistance = SquareDistances[i, j];

        return Math.Sqrt(MinSquareDistance);
    }

    private static double Square(double x)
    {
        return x * x;
    }
}

namespace FontLoader;

internal record HorizontalDistance(int ColoredPixelPosition, byte ColoredPixelIntensity)
{
    public double Distance => ColoredPixelPosition + ((double)ColoredPixelIntensity / 0xFF);
}

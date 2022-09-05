namespace TestFontLoader;

using FontLoader;
using NUnit.Framework;
using System.Drawing;
using System.IO;
using System.Reflection;

[TestFixture]
public class TestPixelArrayHelper
{
    [Test]
    public void IsMatchTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();
        Stream TestBitmapStream = TestAssembly.GetManifestResourceStream($"{typeof(TestPixelArray).Namespace}.Mixed.png");
        Bitmap TestBitmap = new Bitmap(TestBitmapStream);
        PixelArray TestPixelArray = PixelArray.FromBitmap(TestBitmap);
        TestPixelArray = TestPixelArray.Clipped();

        bool IsMatch;

        IsMatch = PixelArrayHelper.IsMatch(TestPixelArray, TestPixelArray, 0);
        Assert.IsTrue(IsMatch);

        IsMatch = PixelArrayHelper.IsMatch(TestPixelArray, TestPixelArray, 1);
        Assert.IsFalse(IsMatch);
    }
}

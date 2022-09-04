namespace TestFontScanner;

using FontLoader;
using NUnit.Framework;

[TestFixture]
public class TestFontLoader
{
    [Test]
    public void BasicTest()
    {
        ScannerFont TestFont = new("Test", typeof(Dummy).Assembly);
    }
}

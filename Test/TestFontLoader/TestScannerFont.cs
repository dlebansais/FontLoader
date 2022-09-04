namespace TestFontScanner;

using FontLoader;
using NUnit.Framework;
using System.Reflection;

[TestFixture]
public class TestScannerFont
{
    [Test]
    public void BasicTest()
    {
        ScannerFont TestFont = new("Test", typeof(Dummy).Assembly);
        Assert.AreEqual("Test", TestFont.Name);
    }

    [Test]
    public void ParsingFontResourceTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();
        ScannerFont TestFont = new("Test", TestAssembly);
        Assert.AreEqual(0, TestFont.ProgressTable.Count);
        Assert.AreEqual(0, TestFont.CharacterTable.Count);
    }
}

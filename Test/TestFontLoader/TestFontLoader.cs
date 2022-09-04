namespace TestFontScanner;

using FontLoader;
using NUnit.Framework;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;

[TestFixture]
public class TestFontLoader
{
    [Test]
    public void BasicTest()
    {
        ScannerFont TestFont = new("Test", typeof(Dummy).Assembly);
    }
}

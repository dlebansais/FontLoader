namespace TestFontLoader;

using FontLoader;
using NUnit.Framework;
using System.Reflection;

[TestFixture]
public class TestFont
{
    [Test]
    public void BasicTest()
    {
        Font TestFont = new("Test", typeof(Dummy).Assembly);
        Assert.AreEqual("Test", TestFont.Name);
    }

    [Test]
    public void ParsingFontResourceTest()
    {
        Assembly TestAssembly = Assembly.GetExecutingAssembly();
        Font TestFont = new("Test", TestAssembly);
        Assert.AreEqual(0, TestFont.ProgressTable.Count);
        Assert.AreEqual(0, TestFont.CharacterTable.Count);
    }
}

namespace FontLoader;

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

public class FontBitmapStream : IDisposable
{
    #region Init
    public FontBitmapStream(Assembly fontAssembly, string resourceName, double fontSize)
    {
        FontAssembly = fontAssembly;
        ResourceName = resourceName;
        FontSize = fontSize;
    }
    #endregion

    #region Properties
    public Assembly FontAssembly { get; }
    public string ResourceName { get; }
    public double FontSize { get; }

    public bool IsLoaded
    {
        get { return LoadedStream is not null; }
    }
    #endregion

    #region Client Interface
    public Stream LoadStream()
    {
        if (LoadedStream is null)
            LoadedStream = FontAssembly.GetManifestResourceStream(ResourceName);

        Stream? CheckedStream = LoadedStream;

        Debug.Assert(CheckedStream is not null);
        return LoadedStream;
    }

    private Stream? LoadedStream;
    #endregion

    #region Implementation of IDisposable
    public void Dispose()
    {
        if (LoadedStream is not null)
        {
            LoadedStream.Dispose();
            LoadedStream = null;
        }
    }
    #endregion
}

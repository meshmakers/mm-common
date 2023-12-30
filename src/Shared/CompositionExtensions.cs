using System.Composition.Hosting;
using System.Reflection;

namespace Meshmakers.Common.Shared;

public static class CompositionExtensions
{
    public static ContainerConfiguration WithDirectory(this ContainerConfiguration @this, string path,
        string filePattern)
    {
        var files = Directory.EnumerateFiles(path, filePattern, SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            @this.WithAssembly(Assembly.LoadFrom(file));
        }

        return @this;
    }
}

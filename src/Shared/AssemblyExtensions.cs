using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace Meshmakers.Common.Shared;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
// ReSharper disable once UnusedType.Global
public static class AssemblyExtensions
{
    public static string GetAssemblyDirectory(this Assembly assembly)
    {
        var codeBase = assembly.Location;
        if (string.IsNullOrWhiteSpace(codeBase))
            throw new InvalidOperationException($"Assembly '{assembly.FullName}' has no location.");

        var path = Path.GetDirectoryName(codeBase);
        if (string.IsNullOrWhiteSpace(path))
            throw new InvalidOperationException($"Assembly '{assembly.FullName}' location '{codeBase}' directory" +
                                                " cannot be determined.");
        return path;
    }
}
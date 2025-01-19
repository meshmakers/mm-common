namespace Meshmakers.Common.Shared;

/// <summary>
/// Extension methods for path handling
/// </summary>
public static class MmPath
{
    /// <summary>
    /// Normalizes separators \ and / to the representation of the current OS
    /// </summary>
    /// <param name="path">Path to be validated</param>
    /// <returns>Validated path</returns>
    public static string NormalizePath(string path)
    {
        var r = path.Replace('/', Path.DirectorySeparatorChar);
        return r.Replace('\\', Path.DirectorySeparatorChar);
    }
}

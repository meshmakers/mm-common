using System.Globalization;

namespace Meshmakers.Common.Shared.Localization;

/// <summary>
///     Interface of a localization provider
/// </summary>
public interface ILocalizationProvider
{
    /// <summary>
    ///     Returns the priority of the resource (the higher the less important)
    /// </summary>
    int Priority { get; }

    /// <summary>
    ///     Sets the used culture
    /// </summary>
    /// <param name="cultureInfo">The culture, that has to be used.</param>
    void SetCulture(CultureInfo cultureInfo);

    /// <summary>
    ///     Returns the resource of the give key
    /// </summary>
    /// <param name="keyName">The key of the resource</param>
    /// <param name="resource">Returns the resource</param>
    /// <returns>True, when the resource exists, otherwise false</returns>
    bool TryGet(string keyName, out object? resource);

    /// <summary>
    ///     Returns the given resource text.
    /// </summary>
    /// <param name="keyName">The key of the resource</param>
    /// <returns></returns>
    string GetString(string keyName);
}
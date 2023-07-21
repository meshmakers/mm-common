using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace Meshmakers.Common.Shared.Localization;

/// <summary>
///     Base class for localization extensions
/// </summary>
public abstract class LocalizationProviderBase : ILocalizationProvider
{
    private readonly Assembly _assembly;
    private readonly string _resourcePath;
    private readonly ResourceSet? _resourceSet;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="assembly">Assembly with the XAML resources (e. g. images)</param>
    /// <param name="xamlResourceName">
    ///     The name of the resource, the images are saved (e. g.
    ///     Meshmakers.Common.Localization.g.resources)
    /// </param>
    /// <param name="resourcePath">The path within the resource set (e. g. images/{0})</param>
    protected LocalizationProviderBase(Assembly assembly, string? xamlResourceName, string resourcePath)
        : this(assembly, resourcePath)
    {
        if (!string.IsNullOrWhiteSpace(xamlResourceName))
        {
            using var resourceStream = assembly.GetManifestResourceStream(xamlResourceName);
            if (resourceStream != null)
            {
                _resourceSet = new ResourceSet(resourceStream);
            }
        }
    }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="assembly">Assembly with the embedded resources (e. g. images)</param>
    /// <param name="resourcePath">The path within the resource set (e. g. images/{0})</param>
    protected LocalizationProviderBase(Assembly assembly, string resourcePath)
    {
        ArgumentValidation.ValidateString(nameof(resourcePath), resourcePath);

        _assembly = assembly;

        _resourcePath = resourcePath;
        CurrentCultureInfo = CultureInfo.CurrentUICulture;
    }

    /// <summary>
    ///     Returns the current culture information
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    protected CultureInfo CurrentCultureInfo { get; private set; }


    /// <summary>
    ///     Sets the used culture
    /// </summary>
    /// <param name="cultureInfo">The culture, that has to be used.</param>
    public void SetCulture(CultureInfo cultureInfo)
    {
        CurrentCultureInfo = cultureInfo;
    }

    /// <summary>
    ///     Returns the resource of the give key
    /// </summary>
    /// <param name="keyName">The key of the resource</param>
    /// <param name="resource">Returns the resource</param>
    /// <returns>True, when the resource exists, otherwise false</returns>
    public virtual bool TryGet(string keyName, out object? resource)
    {
        if (TryGetProductResources(keyName, out resource))
        {
            return true;
        }

        if (ResourceExist(keyName))
        {
            resource = GetResource(keyName);
            return true;
        }

        resource = null;
        return false;
    }


    /// <summary>
    ///     Returns the given resource text.
    /// </summary>
    /// <param name="keyName">The key of the resource</param>
    /// <returns></returns>
    public string GetString(string keyName)
    {
        if (TryGet(keyName, out var resource) && resource is string resourceString)
        {
            return resourceString;
        }

        return keyName;
    }

    /// <summary>
    ///     Returns the priority of the resource (the higher the less important)
    /// </summary>
    public virtual int Priority => 99;

    /// <summary>
    ///     Checks, if a resource is existing
    /// </summary>
    /// <param name="key">The key of the resource</param>
    /// <returns>True, when the resource is existing, otherwise false</returns>
    protected virtual bool ResourceExist(string key)
    {
        var resourcePath = string.Format(_resourcePath, key).ToLower();

        var resourceExists = _resourceSet?.Cast<DictionaryEntry>()
            .Any(e => Equals(e.Key, resourcePath)) ?? false;

        if (!resourceExists)
        {
            resourceExists = _assembly.GetManifestResourceNames().Any(n => n.ToLower() == resourcePath);
        }

        return resourceExists;
    }

    /// <summary>
    ///     Returns a resource, when existing
    /// </summary>
    /// <param name="key">The key of the resource</param>
    /// <returns></returns>
    // ReSharper disable once MemberCanBePrivate.Global
    protected Stream GetResource(string key)
    {
        if (_resourceSet != null)
        {
            var resourcePath = $"images/{key}".ToLower();
            var resource = _resourceSet.GetObject(resourcePath);
            if (resource == null)
            {
                throw new LocalizationException($"Resource '{key}' not found in resource set " +
                                                $"at assembly '{_assembly.FullName}'.");
            }

            return (Stream)resource;
        }
        else
        {
            var resourcePath = string.Format(_resourcePath, key).ToLower();
            var resourcesStreamPath = _assembly.GetManifestResourceNames().SingleOrDefault(n => n.ToLower() == resourcePath);
            if (!string.IsNullOrWhiteSpace(resourcesStreamPath))
            {
                var resourcesStream = _assembly.GetManifestResourceStream(resourcesStreamPath);
                if (resourcesStream == null)
                {
                    throw new LocalizationException($"Resource with path '{resourcesStreamPath}' not found in assembly '{_assembly.FullName}'.");
                }
                return resourcesStream;
            }
            throw new LocalizationException($"Resource '{resourcePath}' not found in assembly '{_assembly.FullName}'.");
        }
    }

    /// <summary>
    ///     Gets product information specific metadata, defined as assembly attributes
    /// </summary>
    /// <param name="keyName">The resource key identifier</param>
    /// <param name="resource">The evaluated value</param>
    /// <returns>True, if the resource has been found, otherwise false</returns>
    protected virtual bool TryGetProductResources(string keyName, out object? resource)
    {
        if (keyName == LocalizationWellKnownKeys.GeneralProductVersion)
        {
            resource = GetVersion();
            return true;
        }

        if (keyName == LocalizationWellKnownKeys.GeneralCompany)
        {
            resource = GetCompany();
            return true;
        }

        if (keyName == LocalizationWellKnownKeys.GeneralCopyright)
        {
            resource = GetCopyright();
            return true;
        }

        resource = null;
        return false;
    }

    private string GetVersion()
    {
        var version = _assembly.GetName().Version;
        if (version != null)
        {
            return version.ToString();
        }

        throw new InvalidOperationException();
    }

    private string GetCopyright()
    {
        // Get all Copyright attributes on this assembly
        var attributes = _assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
        // If there aren't any Copyright attributes, return an empty string
        if (attributes.Length == 0)
        {
            return "";
        }

        // If there is a Copyright attribute, return its value
        return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
    }

    private string GetCompany()
    {
        // Get all Company attributes on this assembly
        var attributes = _assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
        // If there aren't any Company attributes, return an empty string
        if (attributes.Length == 0)
        {
            return "";
        }

        // If there is a Company attribute, return its value
        return ((AssemblyCompanyAttribute)attributes[0]).Company;
    }
}

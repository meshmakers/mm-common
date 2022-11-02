using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Meshmakers.Common.Configuration;

public class ConfigWriter : IConfigWriter
{
    private readonly IFileSystem _fileSystem;
    private readonly Dictionary<string, IOptions<object>> _optionDictionary;

    public ConfigWriter(IFileSystem fileSystem)
    {
        _optionDictionary = new Dictionary<string, IOptions<object>>();
        _fileSystem = fileSystem;
    }

    public ConfigWriter()
        : this(new FileSystem())
    {
    }

    public void AddOptions<TOptions>(string name, IOptions<TOptions> options) where TOptions : class, new()
    {
        _optionDictionary[name] = options;
    }

    public void WriteSettings(string applicationName)
    {
        var directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            $".{applicationName}{Path.DirectorySeparatorChar}");
        var filePath = Path.Combine(directoryPath, "settings.json");

        var serializeObject = new JObject();

        foreach (var optionKeyValue in _optionDictionary)
        {
            var token = JToken.FromObject(optionKeyValue.Value.Value, new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            serializeObject.Add(optionKeyValue.Key, token);
        }

        var json = JsonConvert.SerializeObject(serializeObject, Formatting.Indented, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        //write string to file
        if (!_fileSystem.Directory.Exists(directoryPath))
        {
            _fileSystem.Directory.CreateDirectory(directoryPath);
        }

        _fileSystem.File.WriteAllText(filePath, json);
    }
}

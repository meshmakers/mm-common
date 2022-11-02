using Microsoft.Extensions.Options;

namespace Meshmakers.Common.Configuration;

public interface IConfigWriter
{
    void AddOptions<TOptions>(string name, IOptions<TOptions> options) where TOptions : class, new();

    void WriteSettings(string applicationName);
}

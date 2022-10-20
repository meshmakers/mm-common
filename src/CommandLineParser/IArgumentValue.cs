using System.Collections.ObjectModel;

namespace Meshmakers.Common.CommandLineParser;

public interface IArgumentValue
{
    IArgument Argument { get; }
    ReadOnlyCollection<string> Values { get; }

    T GetValue<T>(int index = 0);
    T GetValue<T>(T defaultValue);
    T GetValue<T>(int index, T defaultValue);
}
using Meshmakers.Common.Shared.Services;

namespace Meshmakers.Common.CommandLineParser;

public interface IArgument
{
    int MandatoryValuesCount { get; }
    bool AreOptionalValuesAllowed { get; }
    string[] Description { get; }
    bool IsMandatoryArgument { get; }
    string LongTerm { get; }
    string ShortTerm { get; }

    bool Compare(string value);

    void ShowUsage(int emptySpacesOnStartCount, IConsoleService consoleService);
}

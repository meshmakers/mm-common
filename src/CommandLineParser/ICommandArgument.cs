namespace Meshmakers.Common.CommandLineParser;

public interface ICommandArgument : IArgument
{
    IEnumerable<ICommandArgumentValue> CommandValues { get; }

    void AddCommandValue(ICommandArgumentValue commandArgumentValue);

    bool TryGetCommandValue(string value, out ICommandArgumentValue? commandArgumentValue);
}

namespace Meshmakers.Common.CommandLineParser;

public interface IArgumentParser
{
    /// <summary>
    ///     Adds an argument definition
    /// </summary>
    /// <param name="shortTerm">Short term of argument e. g. c</param>
    /// <param name="longTerm">Long term of argument e. g. create</param>
    /// <param name="description">The description of the argument. Every array entry creates a single line in usage messages.</param>
    /// <param name="isMandatoryArgument">True, if the argument is mandatory</param>
    ICommandArgument AddCommandArgument(string shortTerm, string longTerm, string[] description,
        bool isMandatoryArgument);

    IArgument AddArgument(string shortTerm, string longTerm, string[] description);
    IArgument AddArgument(string shortTerm, string longTerm, string[] description, int mandatoryValuesCount);
    IArgument AddArgument(string shortTerm, string longTerm, string[] description, bool isMandatoryArgument);

    IArgument AddArgument(string shortTerm, string longTerm, string[] description, int mandatoryValuesCount,
        bool optionalValuesCount);

    IArgument AddArgument(string shortTerm, string longTerm, string[] description, bool isMandatoryArgument,
        int mandatoryValuesCount);

    IArgument AddArgument(string shortTerm, string longTerm, string[] description, bool isMandatoryArgument,
        int mandatoryValuesCount, bool areOptionalValuesAllowed);

    IArgumentValue GetArgumentValue(IArgument argDefinition);

    bool IsArgumentUsed(IArgument argDefinition);
}
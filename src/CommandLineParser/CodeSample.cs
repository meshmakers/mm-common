using Meshmakers.Common.Shared;

namespace Meshmakers.Common.CommandLineParser;

public class CodeSample
{
    public CodeSample(string sampleCode, string description)
    {
        ArgumentValidation.ValidateString(nameof(sampleCode), sampleCode);
        ArgumentValidation.ValidateString(nameof(description), description);

        SampleCode = sampleCode;
        Description = description;
    }

    public string SampleCode { get; }
    public string Description { get; }
}

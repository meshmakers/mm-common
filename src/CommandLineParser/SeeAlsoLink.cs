using Meshmakers.Common.Shared;

namespace Meshmakers.Common.CommandLineParser;

/// <summary>
///     A link to related documentation surfaced by <see cref="CommandDocumentation.SeeAlso" />.
/// </summary>
public sealed class SeeAlsoLink
{
    public SeeAlsoLink(string text, string url)
    {
        ArgumentValidation.ValidateString(nameof(text), text);
        ArgumentValidation.ValidateString(nameof(url), url);

        Text = text;
        Url = url;
    }

    public string Text { get; }
    public string Url { get; }
}

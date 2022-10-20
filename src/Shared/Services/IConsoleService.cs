namespace Meshmakers.Common.Shared.Services;

/// <summary>
///     Interface for console interaction
/// </summary>
public interface IConsoleService
{
    /// <summary>
    ///     Writes the specified string value as error, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="text">The value to write.</param>
    void WriteErrorLine(string text);

    /// <summary>
    ///     Writes the specified string value as error, followed by the current line terminator, to the standard output stream.
    ///     Line breaks are added if the text is longer than the window width.
    /// </summary>
    /// <param name="text">The value to write.</param>
    void WriteErrorLineRegardSpace(string text);

    /// <summary>
    ///     Writes the specified string values as error as a 2-column table form, followed by the current line terminator, to
    ///     the standard output stream.
    /// </summary>
    /// <param name="column1Text">The value to write for column 1.</param>
    /// <param name="column1Length">The maximum length of column 1.</param>
    /// <param name="column2Text">The value to write for column 2.</param>
    void WriteErrorColumnLine(string column1Text, int column1Length, string column2Text);

    /// <summary>
    ///     Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="text">The value to write.</param>
    void WriteLine(string text);

    /// <summary>
    ///     Writes the specified string value, followed by the current line terminator, to the standard output stream.
    ///     Line breaks are added if the text is longer than the window width.
    /// </summary>
    /// <param name="text">The value to write.</param>
    void WriteLineRegardSpace(string text);

    /// <summary>
    ///     Writes the specified string values an 2-column table form, followed by the current line terminator, to the standard
    ///     output stream.
    /// </summary>
    /// <param name="column1Text">The value to write for column 1.</param>
    /// <param name="column1Length">The maximum length of column 1.</param>
    /// <param name="column2Text">The value to write for column 2.</param>
    void WriteColumnLine(string column1Text, int column1Length, string column2Text);
}
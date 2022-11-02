using System;
using System.IO;

namespace Meshmakers.Common.Shared.Services;

/// <summary>
///     Helper class for console interaction
/// </summary>
public class ConsoleService : IConsoleService
{
    private const int UsageMaxLineLength = 160;
    private readonly int _maxLineLength = UsageMaxLineLength;

    /// <summary>
    ///     Constructor
    /// </summary>
    public ConsoleService()
    {
        try
        {
            if (Console.WindowWidth > _maxLineLength)
            {
                _maxLineLength = Console.WindowWidth;
            }
        }
        catch (IOException)
        {
            // We ignore the exception, in case the console window is hosted in an application (e. g. nuget package manager console)
        }
    }

    /// <summary>
    ///     Writes the specified string value as error, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="text">The value to write.</param>
    public void WriteErrorLine(string text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        WriteLine(text);
        Console.ResetColor();
    }

    /// <summary>
    ///     Writes the specified string value as error, followed by the current line terminator, to the standard output stream.
    ///     Line breaks are added if the text is longer than the window width.
    /// </summary>
    /// <param name="text">The value to write.</param>
    public void WriteErrorLineRegardSpace(string text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        WriteLineRegardSpace(text);
        Console.ResetColor();
    }

    /// <summary>
    ///     Writes the specified string values as error as a 2-column table form, followed by the current line terminator, to
    ///     the standard output stream.
    /// </summary>
    /// <param name="column1Text">The value to write for column 1.</param>
    /// <param name="column1Length">The maximum length of column 1.</param>
    /// <param name="column2Text">The value to write for column 2.</param>
    public void WriteErrorColumnLine(string column1Text, int column1Length, string column2Text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        WriteColumnLine(column1Text, column1Length, column2Text);
        Console.ResetColor();
    }

    /// <summary>
    ///     Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="text">The value to write.</param>
    public void WriteLine(string text)
    {
        Console.WriteLine(text);
    }

    /// <summary>
    ///     Writes the specified string value, followed by the current line terminator, to the standard output stream.
    ///     Line breaks are added if the text is longer than the window width.
    /// </summary>
    /// <param name="text">The value to write.</param>
    public void WriteLineRegardSpace(string text)
    {
        WriteLineRegardSpace(text, fragment => fragment);
    }

    /// <summary>
    ///     Writes the specified string values as a 2-column table form, followed by the current line terminator, to the
    ///     standard output stream.
    /// </summary>
    /// <param name="column1Text">The value to write for column 1.</param>
    /// <param name="column1Length">The maximum length of column 1.</param>
    /// <param name="column2Text">The value to write for column 2.</param>
    public void WriteColumnLine(string column1Text, int column1Length, string column2Text)
    {
        ArgumentValidation.ValidateInt(nameof(column1Length), column1Length, 1);

        if (column1Text.Length >= column1Length)
        {
            throw new Exception(
                $"Fatal programming error: The text'{column1Text}' is longer than the defined column length '{column1Length}'!");
        }

        var fullName = column1Text.PadRight(column1Length);

        WriteLineRegardSpace($"{fullName}{column2Text}", fragment => $"{"".PadRight(column1Length)}{fragment}");
    }


    private void WriteLineRegardSpace(string line, Func<string, string> remainingTextTreatment)
    {
        var lineBuilder = line;

        while (!string.IsNullOrEmpty(lineBuilder))
        {
            string nextLine;
            if (lineBuilder.Length >= _maxLineLength)
            {
                var splitIndex = lineBuilder.LastIndexOf(' ', _maxLineLength - 1);
                if (splitIndex < 0)
                {
                    nextLine = lineBuilder;
                    lineBuilder = null;
                }
                else
                {
                    nextLine = lineBuilder.Substring(0, splitIndex);
                    lineBuilder = remainingTextTreatment(lineBuilder.Substring(splitIndex + 1));
                }
            }
            else
            {
                nextLine = lineBuilder;
                lineBuilder = null;
            }

            WriteLine(nextLine);
        }
    }
}

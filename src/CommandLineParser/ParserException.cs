using System;
using System.Runtime.Serialization;

namespace Meshmakers.Common.CommandLineParser;

/// <summary>
///     Thrown, when an problem occured while parsing.
///     Attention! This class is equivalent to the class in MeshmakersBaseLibrary!
/// </summary>
[Serializable]
public class ParserException : Exception
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    /// <summary>
    ///     Constructor
    /// </summary>
    public ParserException()
    {
    }


    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ParserException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">
    ///     The exception that is the cause of the current exception, or a null reference (Nothing in Visual
    ///     Basic) if no inner exception is specified.
    /// </param>
    public ParserException(string message, Exception inner) : base(message, inner)
    {
    }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="info">
    ///     The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the
    ///     exception being thrown.
    /// </param>
    /// <param name="context">
    ///     The System.Runtime.Serialization.StreamingContext that contains contextual information about the
    ///     source or destination.
    /// </param>
    protected ParserException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }
}

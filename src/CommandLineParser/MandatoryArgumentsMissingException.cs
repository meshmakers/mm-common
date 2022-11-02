using System;
using System.Runtime.Serialization;

namespace Meshmakers.Common.CommandLineParser;

[Serializable]
public class MandatoryArgumentsMissingException : ParserException
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public MandatoryArgumentsMissingException()
    {
    }

    public MandatoryArgumentsMissingException(string message) : base(message)
    {
    }

    public MandatoryArgumentsMissingException(string message, Exception inner) : base(message, inner)
    {
    }

    protected MandatoryArgumentsMissingException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}

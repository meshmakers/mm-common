namespace Meshmakers.Common.CommandLineParser;

[Serializable]
public class UnknownArgumentException : ParserException
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public UnknownArgumentException()
    {
    }

    public UnknownArgumentException(string message) : base(message)
    {
    }

    public UnknownArgumentException(string message, Exception inner) : base(message, inner)
    {
    }
}

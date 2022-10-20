using System;
using System.Runtime.Serialization;

namespace Meshmakers.Common.Shared.Localization;

[Serializable]
public class LocalizationException : Exception
{
    public LocalizationException()
    {
    }

    public LocalizationException(string message) : base(message)
    {
    }

    public LocalizationException(string message, Exception inner) : base(message, inner)
    {
    }

    protected LocalizationException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}
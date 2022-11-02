using System.Diagnostics.CodeAnalysis;

namespace Meshmakers.Common.Shared;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
// ReSharper disable once UnusedType.Global
public static class IntExtensions
{
    public static char ToHexChar(this int value)
    {
        return (char)(value + (value < 10 ? 48 : 87));
    }
}

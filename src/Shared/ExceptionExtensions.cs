using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Meshmakers.Common.Shared;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
// ReSharper disable once UnusedType.Global
public static class ExceptionExtensions
{
    public static string GetDirectAndIndirectMessages(this Exception e)
    {
        var stringBuilder = new StringBuilder();
        var prefix = "";

        var tmp = e;
        while (tmp != null)
        {
            stringBuilder.AppendLine(prefix + tmp.Message);
            prefix += "\t";
            tmp = tmp.InnerException;
        }

        return stringBuilder.ToString();
    }
}

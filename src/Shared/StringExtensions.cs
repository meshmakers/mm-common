using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace Meshmakers.Common.Shared;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
// ReSharper disable once UnusedType.Global
public static class StringExtensions
{
    private static readonly Regex ReWords =
        new(
            "[A-Z\\xc0-\\xd6\\xd8-\\xde]?[a-z\\xdf-\\xf6\\xf8-\\xff]+(?:['’](?:d|ll|m|re|s|t|ve))?(?=[\\xac\\xb1\\xd7\\xf7\\x00-\\x2f\\x3a-\\x40\\x5b-\\x60\\x7b-\\xbf\\u2000-\\u206f \\t\\x0b\\f\\xa0\\ufeff\\n\\r\\u2028\\u2029\\u1680\\u180e\\u2000\\u2001\\u2002\\u2003\\u2004\\u2005\\u2006\\u2007\\u2008\\u2009\\u200a\\u202f\\u205f\\u3000]|[A-Z\\xc0-\\xd6\\xd8-\\xde]|$)|(?:[A-Z\\xc0-\\xd6\\xd8-\\xde]|[^\\ud800-\\udfff\\xac\\xb1\\xd7\\xf7\\x00-\\x2f\\x3a-\\x40\\x5b-\\x60\\x7b-\\xbf\\u2000-\\u206f \\t\\x0b\\f\\xa0\\ufeff\\n\\r\\u2028\\u2029\\u1680\\u180e\\u2000\\u2001\\u2002\\u2003\\u2004\\u2005\\u2006\\u2007\\u2008\\u2009\\u200a\\u202f\\u205f\\u3000\\d+\\u2700-\\u27bfa-z\\xdf-\\xf6\\xf8-\\xffA-Z\\xc0-\\xd6\\xd8-\\xde])+(?:['’](?:D|LL|M|RE|S|T|VE))?(?=[\\xac\\xb1\\xd7\\xf7\\x00-\\x2f\\x3a-\\x40\\x5b-\\x60\\x7b-\\xbf\\u2000-\\u206f \\t\\x0b\\f\\xa0\\ufeff\\n\\r\\u2028\\u2029\\u1680\\u180e\\u2000\\u2001\\u2002\\u2003\\u2004\\u2005\\u2006\\u2007\\u2008\\u2009\\u200a\\u202f\\u205f\\u3000]|[A-Z\\xc0-\\xd6\\xd8-\\xde](?:[a-z\\xdf-\\xf6\\xf8-\\xff]|[^\\ud800-\\udfff\\xac\\xb1\\xd7\\xf7\\x00-\\x2f\\x3a-\\x40\\x5b-\\x60\\x7b-\\xbf\\u2000-\\u206f \\t\\x0b\\f\\xa0\\ufeff\\n\\r\\u2028\\u2029\\u1680\\u180e\\u2000\\u2001\\u2002\\u2003\\u2004\\u2005\\u2006\\u2007\\u2008\\u2009\\u200a\\u202f\\u205f\\u3000\\d+\\u2700-\\u27bfa-z\\xdf-\\xf6\\xf8-\\xffA-Z\\xc0-\\xd6\\xd8-\\xde])|$)|[A-Z\\xc0-\\xd6\\xd8-\\xde]?(?:[a-z\\xdf-\\xf6\\xf8-\\xff]|[^\\ud800-\\udfff\\xac\\xb1\\xd7\\xf7\\x00-\\x2f\\x3a-\\x40\\x5b-\\x60\\x7b-\\xbf\\u2000-\\u206f \\t\\x0b\\f\\xa0\\ufeff\\n\\r\\u2028\\u2029\\u1680\\u180e\\u2000\\u2001\\u2002\\u2003\\u2004\\u2005\\u2006\\u2007\\u2008\\u2009\\u200a\\u202f\\u205f\\u3000\\d+\\u2700-\\u27bfa-z\\xdf-\\xf6\\xf8-\\xffA-Z\\xc0-\\xd6\\xd8-\\xde])+(?:['’](?:d|ll|m|re|s|t|ve))?|[A-Z\\xc0-\\xd6\\xd8-\\xde]+(?:['’](?:D|LL|M|RE|S|T|VE))?|\\d+|(?:[\\u2700-\\u27bf]|(?:\\ud83c[\\udde6-\\uddff]){2}|[\\ud800-\\udbff][\\udc00-\\udfff])[\\ufe0e\\ufe0f]?(?:[\\u0300-\\u036f\\ufe20-\\ufe23\\u20d0-\\u20f0]|\\ud83c[\\udffb-\\udfff])?(?:\\u200d(?:[^\\ud800-\\udfff]|(?:\\ud83c[\\udde6-\\uddff]){2}|[\\ud800-\\udbff][\\udc00-\\udfff])[\\ufe0e\\ufe0f]?(?:[\\u0300-\\u036f\\ufe20-\\ufe23\\u20d0-\\u20f0]|\\ud83c[\\udffb-\\udfff])?)*");

    /// <summary>Parses a hex string into its equivalent byte array.</summary>
    /// <param name="s">The hex string to parse.</param>
    /// <returns>The byte equivalent of the hex string.</returns>
    public static byte[] ParseHexString(this string s)
    {
        if (!s.TryParseHexString(out var bytes) || bytes == null)
        {
            throw new FormatException("String should contain only hexadecimal digits.");
        }

        return bytes;
    }

    /// <summary>Tries to parse a hex string to a byte array.</summary>
    /// <param name="s">The hex string.</param>
    /// <param name="bytes">A byte array.</param>
    /// <returns>True if the hex string was successfully parsed.</returns>
    public static bool TryParseHexString(this string s, out byte[]? bytes)
    {
        bytes = null;
        var numArray = new byte[(s.Length + 1) / 2];
        var num1 = 0;
        var num2 = 0;
        if (s.Length % 2 == 1)
        {
            if (!TryParseHexChar(s[num1++], out var num3))
            {
                return false;
            }

            numArray[num2++] = (byte)num3;
        }

        while (num1 < s.Length)
        {
            var str1 = s;
            var index1 = num1;
            var num3 = index1 + 1;
            if (!TryParseHexChar(str1[index1], out var num4))
            {
                return false;
            }

            var str2 = s;
            var index2 = num3;
            num1 = index2 + 1;
            if (!TryParseHexChar(str2[index2], out var num5))
            {
                return false;
            }

            numArray[num2++] = (byte)((num4 << 4) | num5);
        }

        bytes = numArray;
        return true;
    }

    public static string EnsureEndsWith(this string s, string value)
    {
        if (!s.EndsWith(value))
        {
            return s + value;
        }

        return s;
    }

    public static string ToCamelCase(this string s)
    {
        return string.IsNullOrWhiteSpace(s)
            ? string.Empty
            : $"{(object)char.ToLowerInvariant(s[0])}{(object)s.Substring(1)}";
    }

    public static string ToPascalCase(this string s)
    {
        return string.IsNullOrWhiteSpace(s)
            ? string.Empty
            : $"{(object)char.ToUpperInvariant(s[0])}{(object)s.Substring(1)}";
    }

    public static string ToConstantCase(this string s)
    {
        return ChangeCase(s, "_", w => w.ToUpperInvariant());
    }

    public static string ChangeCase(this string s, string separator, Func<string, string> composer)
    {
        return ChangeCase(s, separator, (w, _) => composer(w));
    }

    public static string ChangeCase(this string s, string separator, Func<string, int, string> composer)
    {
        var str1 = "";
        var num = 0;
        foreach (var word in ToWords(s))
        {
            str1 = str1 + (num == 0 ? "" : separator) + composer(word, num++);
        }

        return str1;
    }

    public static string EncodeBase64(this string s)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(s);
        return Convert.ToBase64String(plainTextBytes);
    }
    
    public static string DecodeBase64(this string s)
    {
        var encodedTextBytes = Convert.FromBase64String(s);
        return Encoding.UTF8.GetString(encodedTextBytes);
    }

    public static IEnumerable<string> ToWords(this string s)
    {
        foreach (Capture match in ReWords.Matches(s))
        {
            yield return match.Value;
        }
    }

    private static bool TryParseHexChar(char c, out int value)
    {
        if (c >= '0' && c <= '9')
        {
            value = c - 48;
            return true;
        }

        if (c >= 'a' && c <= 'f')
        {
            value = 10 + (c - 97);
            return true;
        }

        if (c >= 'A' && c <= 'F')
        {
            value = 10 + (c - 65);
            return true;
        }

        value = 0;
        return false;
    }
}

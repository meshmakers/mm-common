using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Meshmakers.Common.Shared;

// ReSharper disable once UnusedType.Global
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class EnumerableExtensions
{
    public static IEnumerable<T> Slice<T>(this IEnumerable<T> collection, int start, int end)
    {
        var index = 0;
        var count = 0;

        var list = collection.ToList();
        count = list.Count;

        // Get start/end indexes, negative numbers start at the end of the collection.
        if (start < 0)
        {
            start += count;
        }

        if (end < 0)
        {
            end += count;
        }

        foreach (var item in list)
        {
            if (index >= end)
            {
                yield break;
            }

            if (index >= start)
            {
                yield return item;
            }

            ++index;
        }
    }
}
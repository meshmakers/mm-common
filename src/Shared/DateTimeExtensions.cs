using System;
using System.Diagnostics.CodeAnalysis;

namespace Meshmakers.Common.Shared;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
// ReSharper disable once UnusedType.Global
public static class DateTimeExtensions
{
    /// <summary>
    ///     Converts a DateTime to UTC (with special handling for MinValue and MaxValue).
    /// </summary>
    /// <param name="dateTime">A DateTime.</param>
    /// <returns>The DateTime in UTC.</returns>
    public static DateTime ToUniversalTime(this DateTime dateTime)
    {
        if (dateTime == DateTime.MinValue)
        {
            return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
        }

        return dateTime == DateTime.MaxValue
            ? DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc)
            : dateTime.ToUniversalTime();
    }
}
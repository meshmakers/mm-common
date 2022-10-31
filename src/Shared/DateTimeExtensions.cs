using System;
using System.Diagnostics.CodeAnalysis;

namespace Meshmakers.Common.Shared;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
// ReSharper disable once UnusedType.Global
public static class DateTimeExtensions
{
    private const string InvalidUnixEpochErrorMessage = "Unix epoc starts January 1st, 1970";
    private const string InvalidNegativeUnixEpochErrorMessage = "Unix epoc must be a positive number";
    
    /// <summary>
    ///     Converts a DateTime to UTC (with special handling for MinValue and MaxValue).
    /// </summary>
    /// <param name="dateTime">A DateTime.</param>
    /// <returns>The DateTime in UTC.</returns>
    public static DateTime ToUtc(this DateTime dateTime)
    {
        if (dateTime == DateTime.MinValue)
            return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
        
        return dateTime == DateTime.MaxValue
            ? DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc)
            : dateTime.ToUniversalTime();
    }
    
    /// <summary>
    ///     Convert a millisecond long into a DateTime.
    /// </summary>
    public static DateTime FromUnixEpochMilliSecondsTime(this long dateTime)
    {
        if (dateTime < 0)
            throw new ArgumentOutOfRangeException(InvalidNegativeUnixEpochErrorMessage);
        
        var ret = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return ret.AddMilliseconds(dateTime);
    }

    /// <summary>
    ///     Convert a second long into a DateTime.
    /// </summary>
    public static DateTime FromUnixEpochSecondsTime(this long dateTime)
    {
        if (dateTime < 0)
            throw new ArgumentOutOfRangeException(InvalidNegativeUnixEpochErrorMessage);
        
        var ret = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return ret.AddSeconds(dateTime);
    }
    
    /// <summary>
    ///     Convert a DateTime into a millisecond long.
    /// </summary>
    public static long ToUnixEpochInMilliSecondsTime(this DateTime dateTime)
    {
        if (dateTime == DateTime.MinValue)
            return 0;

        var epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var delta = dateTime - epoc;

        if (delta.TotalMilliseconds < 0) 
            throw new ArgumentOutOfRangeException(InvalidUnixEpochErrorMessage);

        return (long)delta.TotalMilliseconds;
    }

    /// <summary>
    ///     Convert a DateTime into a second long.
    /// </summary>
    public static long ToUnixEpochInSecondsTime(this DateTime dateTime)
    {
        if (dateTime == DateTime.MinValue)
            return 0;

        var epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var delta = dateTime - epoc;

        if (delta.TotalSeconds < 0) 
            throw new ArgumentOutOfRangeException(InvalidUnixEpochErrorMessage);

        return (long)delta.TotalSeconds;
    }
}
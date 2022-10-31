
using System;
using Meshmakers.Common.Shared;
using Xunit;

namespace Meshmakers.Common.SharedTests;

public class DateTimeExtensionTests
{
    private readonly DateTime _testDateTimeOutOfRangeMin = new(1969, 10, 31, 17, 00, 53, millisecond: 123, DateTimeKind.Utc);
    private readonly DateTime _testDateTime = new(2022, 10, 31, 17, 0, 53, millisecond: 123, DateTimeKind.Utc);
    private readonly DateTime _testDateTimeWithoutMilliSeconds = new(2022, 10, 31, 17, 0, 53, millisecond: 0, DateTimeKind.Utc);
    private readonly DateTime _epocDateTime = new(1970, 1, 1, 0, 0, 0, millisecond: 0, DateTimeKind.Utc);
    private readonly DateTime _maxDateTimeWithoutMilliSeconds = new(9999, 12, 31, 23, 59, 59, millisecond: 0, DateTimeKind.Utc);
    private readonly DateTime _maxDateTimeWithoutTicks = new(9999, 12, 31, 23, 59, 59, millisecond: 999, DateTimeKind.Utc);
    private readonly DateTime _TestDateTimeLocal = new(2022, 12, 31, 23, 59, 59, millisecond: 999, DateTimeKind.Local);
    
    [Fact]
    public void ToUtc_MinValue_OK()
    {
        Assert.Equal(DateTime.MinValue, DateTime.MinValue.ToUtc());
    }
    
    [Fact]
    public void ToUtc_MaxValue_OK()
    {
        Assert.Equal(DateTime.MaxValue, DateTime.MaxValue.ToUtc());
    }
    
    [Fact]
    public void ToUtc_Local_OK()
    {
        Assert.Equal(new DateTime(2022, 12, 31, 22,59,59,999, DateTimeKind.Utc), _TestDateTimeLocal.ToUtc());
    }
    
    [Fact]
    public void ToUtc_Utc_OK()
    {
        Assert.Equal(_testDateTime, _testDateTime.ToUtc());
    }
    
    [Fact]
    public void ToUnixEpochInSecondsTime_OK()
    {
        Assert.Equal(1667235653, _testDateTime.ToUnixEpochInSecondsTime());
    }
    
    [Fact]
    public void ToUnixEpochInSecondsTime_MinDate_OK()
    {
        Assert.Equal(0, DateTime.MinValue.ToUnixEpochInSecondsTime());
    }
    
    [Fact]
    public void ToUnixEpochInSecondsTime_MaxDate_OK()
    {
        Assert.Equal(253402300800, DateTime.MaxValue.ToUnixEpochInSecondsTime());
    }
    
    [Fact]
    public void ToUnixEpochInSecondsTime_Before1970_Fail()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _testDateTimeOutOfRangeMin.ToUnixEpochInSecondsTime());
    }
    
    [Fact]
    public void ToUnixEpochInMilliSecondsTime_OK()
    {
        Assert.Equal(1667235653123, _testDateTime.ToUnixEpochInMilliSecondsTime());
    }
    
    [Fact]
    public void ToUnixEpochInMilliSecondsTime_MinDate_OK()
    {
        Assert.Equal(0, DateTime.MinValue.ToUnixEpochInMilliSecondsTime());
    }
    
    [Fact]
    public void ToUnixEpochInMilliSecondsTime_MaxDate_OK()
    {
        Assert.Equal(253402300800000, DateTime.MaxValue.ToUnixEpochInMilliSecondsTime());
    }
    
    [Fact]
    public void ToUnixEpochInMilliSecondsTime_Before1970_Fail()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _testDateTimeOutOfRangeMin.ToUnixEpochInMilliSecondsTime());
    }
    
    [Fact]
    public void FromUnixEpochSecondsTime_OK()
    {
        long test = 1667235653;
        Assert.Equal(_testDateTimeWithoutMilliSeconds, test.FromUnixEpochSecondsTime());
    }
    
    [Fact]
    public void FromUnixEpochSecondsTime_MinDate_Fail()
    {
        long test = long.MinValue;
        Assert.Throws<ArgumentOutOfRangeException>(() => test.FromUnixEpochSecondsTime());
    }
    
    [Fact]
    public void FromUnixEpochSecondsTime_Zero_OK()
    {
        long test = 0;
        Assert.Equal(_epocDateTime, test.FromUnixEpochSecondsTime());
    }
    
    [Fact]
    public void FromUnixEpochSecondsTime_MaxLong_Fail()
    {
        long test = long.MaxValue;
        Assert.Throws<ArgumentOutOfRangeException>(() => test.FromUnixEpochSecondsTime());
    }
    
    [Fact]
    public void FromUnixEpochSecondsTime_MaxDate_Fail()
    {
        long test = 253402300799;
        Assert.Equal(_maxDateTimeWithoutMilliSeconds, test.FromUnixEpochSecondsTime());
    }
    
    [Fact]
    public void FromUnixEpochSecondsTime_Fail()
    {
        long test = -1;
        Assert.Throws<ArgumentOutOfRangeException>(() => test.FromUnixEpochSecondsTime());
    }
    
    
    
    [Fact]
    public void FromUnixEpochMilliSecondsTime_OK()
    {
        long test = 1667235653123;
        Assert.Equal(_testDateTime, test.FromUnixEpochMilliSecondsTime());
    }
    
    [Fact]
    public void FromUnixEpochMilliSecondsTime_MinDate_Fail()
    {
        long test = long.MinValue;
        Assert.Throws<ArgumentOutOfRangeException>(() => test.FromUnixEpochMilliSecondsTime());
    }
    
    [Fact]
    public void FromUnixEpochMilliSecondsTime_Zero_OK()
    {
        long test = 0;
        Assert.Equal(_epocDateTime, test.FromUnixEpochMilliSecondsTime());
    }
    
    [Fact]
    public void FromUnixEpochMilliSecondsTime_MaxLong_Fail()
    {
        long test = long.MaxValue;
        Assert.Throws<ArgumentOutOfRangeException>(() => test.FromUnixEpochMilliSecondsTime());
    }
    
    [Fact]
    public void FromUnixEpochMilliSecondsTime_MaxDate_Fail()
    {
        long test = 253402300799999;
        Assert.Equal(_maxDateTimeWithoutTicks, test.FromUnixEpochMilliSecondsTime());
    }
    
    [Fact]
    public void FromUnixEpochMilliSecondsTime_Fail()
    {
        long test = -1;
        Assert.Throws<ArgumentOutOfRangeException>(() => test.FromUnixEpochMilliSecondsTime());
    }
}
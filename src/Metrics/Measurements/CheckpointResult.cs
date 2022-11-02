using Meshmakers.Common.Metrics.Meters.Data;

namespace Meshmakers.Common.Metrics.Measurements;

public class CheckpointResult
{
    private DateTime _firstStart;
    private DateTime _lastStop;

    internal CheckpointResult(string name)
    {
        Name = name;
    }

    internal CheckpointResult(Checkpoint checkpoint) : this(checkpoint.Name)
    {
        Add(checkpoint);
    }

    public string Name { get; }
    public int Calls { get; private set; }
    public long MinOfDeltaInMs { get; private set; }
    public long MaxOfDeltaInMs { get; private set; }
    public long AvgOfDeltaInMs { get; private set; }
    public long MinOfTotalInMs { get; private set; }
    public long MaxOfTotalInMs { get; private set; }
    public long AvgOfTotalInMs { get; private set; }

    public DateTime FirstStart
    {
        get
        {
            if (Calls == 0)
            {
                throw new MeasurementException("There is no data available");
            }

            return _firstStart;
        }
    }

    public DateTime LastStop
    {
        get
        {
            if (Calls == 0)
            {
                throw new MeasurementException("There is no data available");
            }

            return _lastStop;
        }
    }

    internal void Add(Checkpoint checkpoint)
    {
        if (!Name.Equals(checkpoint.Name))
        {
            throw new MeasurementException($"The data does not fit (Expected: '{Name}', Given: '{checkpoint.Name}')");
        }

        CalcMinOfDelta(checkpoint);
        CalcMaxOfDelta(checkpoint);
        CalcAvgOfDelta(checkpoint);
        CalcMinOfTotal(checkpoint);
        CalcMaxOfTotal(checkpoint);
        CalcAvgOfTotal(checkpoint);
        SetFirstStart(checkpoint);
        SetLastStop(checkpoint);
        Calls++;
    }

    private void CalcMinOfDelta(Checkpoint checkpoint)
    {
        if (Calls > 0 && checkpoint.DeltaMilliseconds >= MinOfDeltaInMs)
        {
            return;
        }

        MinOfDeltaInMs = checkpoint.DeltaMilliseconds;
    }

    private void CalcMaxOfDelta(Checkpoint checkpoint)
    {
        if (checkpoint.DeltaMilliseconds <= MaxOfDeltaInMs)
        {
            return;
        }

        MaxOfDeltaInMs = checkpoint.DeltaMilliseconds;
    }

    private void CalcMinOfTotal(Checkpoint checkpoint)
    {
        if (Calls > 0 && checkpoint.TotalMilliseconds >= MinOfTotalInMs)
        {
            return;
        }

        MinOfTotalInMs = checkpoint.TotalMilliseconds;
    }

    private void CalcMaxOfTotal(Checkpoint checkpoint)
    {
        if (checkpoint.TotalMilliseconds <= MaxOfTotalInMs)
        {
            return;
        }

        MaxOfTotalInMs = checkpoint.TotalMilliseconds;
    }

    private void CalcAvgOfDelta(Checkpoint checkpoint)
    {
        AvgOfDeltaInMs = CalcAvg(AvgOfDeltaInMs, Calls, checkpoint.DeltaMilliseconds);
    }

    private void CalcAvgOfTotal(Checkpoint checkpoint)
    {
        AvgOfTotalInMs = CalcAvg(AvgOfTotalInMs, Calls, checkpoint.TotalMilliseconds);
    }

    private void SetFirstStart(Checkpoint checkpoint)
    {
        if (Calls > 0 && _firstStart <= checkpoint.CreationDateTime)
        {
            return;
        }

        _firstStart = checkpoint.CreationDateTime;
    }

    private void SetLastStop(Checkpoint checkpoint)
    {
        if (Calls > 0 && _lastStop >= checkpoint.CreationDateTime)
        {
            return;
        }

        _lastStop = checkpoint.CreationDateTime;
    }

    private static long CalcAvg(long avg, int count, long newValue)
    {
        if (count == 0)
        {
            return newValue;
        }

        return (avg * count + newValue) / (count + 1);
    }
}
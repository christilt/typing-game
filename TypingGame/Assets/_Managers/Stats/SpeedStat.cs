using System;

public class SpeedStat
{
    public SpeedStat(TimeSpan timeTaken, TimeSpan timeBenchmark)
    {
        TimeTaken = timeTaken;
        TimeBenchmark = timeBenchmark;
    }

    public TimeSpan TimeTaken { get; }
    public TimeSpan TimeBenchmark { get; }

    public override string ToString() => $"{TimeTaken} (Benchmark: {TimeBenchmark})";

}
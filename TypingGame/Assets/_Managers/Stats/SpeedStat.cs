using System;
using UnityEngine;

public class SpeedStat : IStat
{
    private const float MaxTimeProportion = 2.0f;
    private SpeedStat(TimeSpan timeTaken, TimeSpan timeBenchmark, StatCategory category, float timeProportion, float rating)
    {
        TimeTaken = timeTaken;
        TimeBenchmark = timeBenchmark;
        Category = category;
        TimeProportion = timeProportion;
        Rating = rating;
    }

    public TimeSpan TimeTaken { get; }
    public TimeSpan TimeBenchmark { get; }
    public StatCategory Category { get; }
    public float TimeProportion { get; }
    // 1-0
    public float Rating { get; }

    public static SpeedStat Calculate(TimeSpan timeTaken, TimeSpan timeBenchmark)
    {
        var timeProportion = CalculateTimeProportion(timeTaken, timeBenchmark);
        var rating = CalculateRating(timeProportion);
        var category = CalculateCategory(rating);
        return new SpeedStat(timeTaken, timeBenchmark, category, timeProportion, rating);
    }

    private static StatCategory CalculateCategory(float rating)
    {
        if (rating >= 0.90f)
            return StatCategory.Great;
        else if (rating >= 0.70f)
            return StatCategory.Good;
        else if (rating >= 0.30f)
            return StatCategory.Average;
        else
            return StatCategory.Bad;
    }

    private static float CalculateTimeProportion(TimeSpan timeTaken, TimeSpan timeBenchmark)
    {
        var timeProportion = (float)timeTaken.TotalSeconds / (float)timeBenchmark.TotalSeconds;
        var timeProportionCapped = Mathf.Min(timeProportion, MaxTimeProportion);
        return timeProportionCapped;
    }

    private static float CalculateRating(float timeProportion)
    {
        return 1f - (timeProportion / MaxTimeProportion);
    }

    public override string ToString() => $"{TimeTaken} (Benchmark: {TimeBenchmark}) - {Category}";

}
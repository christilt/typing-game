using System;
using UnityEngine;

public class LevelStats
{
    private LevelStats(AccuracyStat accuracy, SpeedStat speed, LevelGrade grade)
    {
        if (accuracy is null || accuracy.Proportion is null) throw new ArgumentException(nameof(accuracy));
        if (speed is null) throw new ArgumentException(nameof(speed));

        Accuracy = accuracy;
        Speed = speed;
        Grade = grade;
    }

    public LevelGrade Grade { get; }
    public AccuracyStat Accuracy { get; }
    public SpeedStat Speed { get; }

    public static LevelStats Calculate(AccuracyRecorder accuracyRecorder, SpeedRecorder speedRecorder, LevelSettings settings)
    {
        var accuracy = accuracyRecorder.CalculateAccuracy();
        var speed = speedRecorder.CalculateSpeed(settings.BenchmarkDurationSeconds);
        var grade = CalculateGrade(accuracy, speed);

        return new LevelStats(accuracy, speed, grade);
    }

    private static LevelGrade CalculateGrade(AccuracyStat accuracy, SpeedStat speed)
    {
        const float AccuracyRatingMultiplier = 1.0f;
        const float SpeedRatingMultiplier = 1.0f;

        var accuracyScore = GetAccuracyRating(accuracy) * AccuracyRatingMultiplier;
        var maxAccuracyScore = 1 * AccuracyRatingMultiplier;

        var speedScore = GetSpeedRating(speed) * SpeedRatingMultiplier;
        var maxSpeedScore = 1 * SpeedRatingMultiplier;

        var score = accuracyScore + speedScore;
        var maxScore = maxAccuracyScore + maxSpeedScore;

        var scorePercentage = score / maxScore;

        if (scorePercentage >= 95)
            return LevelGrade.S;
        else if (scorePercentage >= 90)
            return LevelGrade.A;
        else if (scorePercentage >= 85)
            return LevelGrade.B;
        else if (scorePercentage >= 70)
            return LevelGrade.C;
        else if (scorePercentage >= 60)
            return LevelGrade.D;
        else
            return LevelGrade.E;
    }

    // 1-0
    private static float GetAccuracyRating(AccuracyStat accuracy) => accuracy.Proportion.Value;

    // 1-0
    private static float GetSpeedRating(SpeedStat speed)
    {
        const float MaxTimeProportion = 2.0f;
        var timeProportion = (float)speed.TimeTaken.TotalSeconds / (float)speed.TimeBenchmark.TotalSeconds;
        var timeProportionCapped = Mathf.Min(timeProportion, MaxTimeProportion);
        return 1f - (timeProportionCapped / MaxTimeProportion);
    }
}
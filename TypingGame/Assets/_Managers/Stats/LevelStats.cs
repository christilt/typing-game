using System;
using UnityEngine;

// TODO: Maybe show text for mini achievements
// TODO: Achievement for burst of high speed, say 10 letters > x speed?
// TODO: Achievement for high accuracy over period?
public class LevelStats : IStat
{
    private LevelStats(AccuracyStat accuracy, SpeedStat speed, LevelRank rank, float rating, StatCategory category, StreakStat bestStreak)
    {
        if (accuracy is null) throw new ArgumentException(nameof(accuracy));
        if (speed is null) throw new ArgumentException(nameof(speed));

        Accuracy = accuracy;
        Speed = speed;
        Rank = rank;
        Rating = rating;
        Category = category;
        BestStreak = bestStreak;
    }

    public LevelRank Rank { get; }
    public AccuracyStat Accuracy { get; }
    public SpeedStat Speed { get; }
    public StreakStat BestStreak { get; }
    public float Rating { get;}
    public StatCategory Category { get; }

    public static LevelStats Calculate(TypingRecorder typingRecorder, SpeedRecorder speedRecorder, LevelSettings settings)
    {
        var accuracy = typingRecorder.CalculateAccuracy();
        var speed = speedRecorder.CalculateSpeed(settings.BenchmarkDurationSeconds);
        var rating = CalculateRating(accuracy, speed);
        var rank = CalculateRank(rating);
        var category = CalculateCategory(rank);
        var bestStreak = typingRecorder.CalculateBestStreak();

        return new LevelStats(accuracy, speed, rank, rating, category, bestStreak);
    }

    private static float CalculateRating(AccuracyStat accuracy, SpeedStat speed)
    {
        const float AccuracyRatingMultiplier = 2.5f;
        const float SpeedRatingMultiplier = 0.5f;

        var accuracyScore = accuracy.Rating * AccuracyRatingMultiplier;
        var maxAccuracyScore = 1 * AccuracyRatingMultiplier;

        var speedScore = speed.Rating * SpeedRatingMultiplier;
        var maxSpeedScore = 1 * SpeedRatingMultiplier;

        var score = accuracyScore + speedScore;
        var maxScore = maxAccuracyScore + maxSpeedScore;

        return score / maxScore;
    }

    private static LevelRank CalculateRank(float rating)
    {
        if (rating >= 0.95f)
            return LevelRank.S;
        else if (rating >= 0.90f)
            return LevelRank.A;
        else if (rating >= 0.80f)
            return LevelRank.B;
        else if (rating >= 0.70f)
            return LevelRank.C;
        else if (rating >= 0.60f)
            return LevelRank.D;
        else
            return LevelRank.E;
    }

    private static StatCategory CalculateCategory(LevelRank rank)
    {
        switch (rank)
        {
            case LevelRank.S:
                return StatCategory.Great;
            case LevelRank.A:
            case LevelRank.B:
                return StatCategory.Good;
            case LevelRank.C:
            case LevelRank.D:
                return StatCategory.Average;
            default:
                return StatCategory.Bad;
        }
    }
}
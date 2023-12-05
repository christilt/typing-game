using System;
using UnityEngine;

// TODO: Maybe show text for mini achievements
// TODO: Achievement for burst of high speed, say 10 letters > x speed?
// TODO: Achievement for high accuracy over period?
public class LevelStats : IStat
{
    private LevelStats(AccuracyStat accuracy, SpeedStat speed, LevelRank rank, float rating, int score, StatCategory category, StreakStat bestStreak, BurstStat topSpeed)
    {
        if (accuracy is null) throw new ArgumentException(nameof(accuracy));
        if (speed is null) throw new ArgumentException(nameof(speed));

        Accuracy = accuracy;
        Speed = speed;
        Rank = rank;
        Rating = rating;
        Score = score;
        Category = category;
        BestStreak = bestStreak;
        TopSpeed = topSpeed;
    }

    public LevelRank Rank { get; }
    public AccuracyStat Accuracy { get; }
    public SpeedStat Speed { get; }
    public StreakStat BestStreak { get; }
    public BurstStat TopSpeed { get; }
    public float Rating { get; }
    public int Score { get; }
    public StatCategory Category { get; }

    public static LevelStats Calculate(TypingRecorder typingRecorder, SpeedRecorder speedRecorder)
    {
        var accuracy = typingRecorder.CalculateAccuracy();
        var speed = speedRecorder.CalculateSpeed(SettingsManager.Instance.BenchmarkDurationSeconds.Value);
        var rating = CalculateRating(accuracy, speed);
        var score = CalculateScore(rating);
        var rank = CalculateRank(rating);
        var category = CalculateCategory(rank);
        var bestStreak = typingRecorder.CalculateBestStreak();
        var topSpeed = typingRecorder.CalculateTopSpeed();

        return new LevelStats(accuracy, speed, rank, rating, score, category, bestStreak, topSpeed);
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

    private static int CalculateScore(float rating)
    {
        var unrounded = rating * 100;
        return (int)Math.Floor(unrounded);
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
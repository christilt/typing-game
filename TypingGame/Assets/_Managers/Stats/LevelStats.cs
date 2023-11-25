﻿using System;
using UnityEngine;

public class LevelStats : IStat
{
    private LevelStats(AccuracyStat accuracy, SpeedStat speed, LevelRank rank, float rating, StatCategory category)
    {
        if (accuracy is null) throw new ArgumentException(nameof(accuracy));
        if (speed is null) throw new ArgumentException(nameof(speed));

        Accuracy = accuracy;
        Speed = speed;
        Rank = rank;
        Rating = rating;
        Category = category;
    }

    public LevelRank Rank { get; }
    public AccuracyStat Accuracy { get; }
    public SpeedStat Speed { get; }
    public float Rating { get;}
    public StatCategory Category { get; }

    public static LevelStats Calculate(AccuracyRecorder accuracyRecorder, SpeedRecorder speedRecorder, LevelSettings settings)
    {
        var accuracy = accuracyRecorder.CalculateAccuracy();
        var speed = speedRecorder.CalculateSpeed(settings.BenchmarkDurationSeconds);
        var rating = CalculateRating(accuracy, speed);
        var rank = CalculateRank(rating);
        var category = CalculateCategory(rank);

        return new LevelStats(accuracy, speed, rank, rating, category);
    }

    private static float CalculateRating(AccuracyStat accuracy, SpeedStat speed)
    {
        const float AccuracyRatingMultiplier = 1.5f;
        const float SpeedRatingMultiplier = 1.0f;

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
        else if (rating >= 0.85f)
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
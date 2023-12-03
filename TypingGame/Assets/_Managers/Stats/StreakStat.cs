public class StreakStat : IStat
{
    private StreakStat(int count, StatCategory category)
    {
        Count = count;
        Category = category;
    }

    public int Count { get; }
    public StatCategory Category { get; }

    public static StreakStat Calculate(int streakCount)
    {
        var category = CalculateCategory(streakCount);
        return new StreakStat(streakCount, category);
    }

    private static StatCategory CalculateCategory(int streakCount)
    {
        if (streakCount >= 30)
            return StatCategory.Great;
        else if (streakCount >= 20)
            return StatCategory.Good;
        else
            return StatCategory.Bad;
    }
}
public struct BurstStat : IStat
{
    private BurstStat(int keys, float duration, float keysPerSecond, float wordsPerMinute, StatCategory category)
    {
        Keys = keys;
        Duration = duration;
        KeysPerSecond = keysPerSecond;
        WordsPerMinute = wordsPerMinute;
        Category = category;
    }

    public int Keys { get; }
    public float Duration { get; }
    public float KeysPerSecond { get; }
    public float WordsPerMinute { get; }

    public StatCategory Category { get; }

    public static BurstStat Calculate(int keys, float duration)
    {
        var keysPerSecond = keys / duration;
        var wordsPerMinute = ToWordsPerMinute(keysPerSecond);
        var category = CalculateCategory(wordsPerMinute);
        var burst = new BurstStat(keys, duration, keysPerSecond, wordsPerMinute, category);
        return burst;
    }

    public bool IsBetterThan(BurstStat other)
    {
        // TODO: Does this need to be more clever?

        return KeysPerSecond > other.KeysPerSecond;
    }

    public override string ToString()
    {
        return $"{WordsPerMinute:n1} wpm ({Keys}keys / {Duration:n0}s)";
    }

    private static StatCategory CalculateCategory(float wordsPerMinute)
    {
        // Based on fast medium and slow groups described in https://en.wikipedia.org/wiki/Words_per_minute
        if (wordsPerMinute >= 40)
            return StatCategory.Great;
        else if (wordsPerMinute >= 35)
            return StatCategory.Good;
        else if (wordsPerMinute >= 23)
            return StatCategory.Average;
        else
            return StatCategory.Bad;
    }

    private static float ToWordsPerMinute(float keysPerSecond)
    {
        // Consensus whole number based on various sources, and based on usage rather than dictionary
        // e.g. https://en.wikipedia.org/wiki/Words_per_minute, https://www.ilovelanguages.com/how-many-letters-does-the-average-english-word-have/, https://en.ans.wiki/6074/what-is-the-average-number-of-letters-per-word/
        const int AverageLettersPerWord = 5;

        var lettersPerMinute = keysPerSecond * 60;
        var wordsPerMinute = lettersPerMinute / AverageLettersPerWord;
        return wordsPerMinute;
    }
}
public class AccuracyStat : IStat
{
    private AccuracyStat(int keysCorrect, int keysTyped, StatCategory statCategory, float proportion, float rating)
    {
        KeysCorrect = keysCorrect;
        KeysTyped = keysTyped;
        Category = statCategory;
        Proportion = proportion;
        Rating = rating;
    }

    public int KeysCorrect { get; }
    public int KeysTyped { get; }
    public StatCategory Category { get; }
    public float Proportion { get; }
    // 1-0
    public float Rating { get; }



    public static AccuracyStat Calculate(int keysCorrect, int keysTyped)
    {
        var proportion = CalculateProportion(keysCorrect, keysTyped);
        var rating = CalculateRating(proportion);
        var category = CalculateCategory(rating);
        return new AccuracyStat(keysCorrect, keysTyped, category, proportion, rating);
    }

    private static float CalculateProportion(int keysCorrect, int keysTyped)
    {
        if (keysCorrect == 0)
            return 0;

        return (float)keysCorrect / (float)keysTyped;
    }

    private static StatCategory CalculateCategory(float rating)
    {
        if (rating >= 0.95f)
            return StatCategory.Great;
        else if (rating >= 0.85f)
            return StatCategory.Good;
        else if (rating >= 0.60f)
            return StatCategory.Average;
        else
            return StatCategory.Bad;
    }

    private static float CalculateRating(float? proportion)
    {
        if (proportion == null)
            return 0.9f;

        return proportion.Value;
    }

    public override string ToString()
    {
        return $"Accuracy: {Proportion:P2} ({KeysCorrect}/{KeysTyped}) - {Category}";
    }
}
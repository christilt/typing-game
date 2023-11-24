using UnityEngine;

public class AccuracyRecorder
{
    public int KeysCorrect { get; private set; }
    public int KeysTyped { get; private set; }

    public void LogCorrectKey()
    {
        KeysCorrect++;
        KeysTyped++;
    }

    public void LogIncorrectKey()
    {
        KeysTyped++;
    }

    public AccuracyStat CalculateAccuracy() => new AccuracyStat(KeysCorrect, KeysTyped);
}

public class AccuracyStat
{
    public AccuracyStat(int keysCorrect, int keysTyped)
    {
        KeysCorrect = keysCorrect;
        KeysTyped = keysTyped;
    }

    public int KeysCorrect { get; }
    public int KeysTyped { get; }
    public float? Proportion
    {
        get
        {
            if (KeysTyped == 0)
                return null;

            return (float)KeysCorrect / (float)KeysTyped;
        }
    }

    public override string ToString()
    {
        if (!Proportion.HasValue)
            return "-";

        return $"Accuracy: {Proportion:P2} ({KeysCorrect}/{KeysTyped})";
    }
}
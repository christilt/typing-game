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

    public AccuracyStat CalculateAccuracy() => AccuracyStat.Calculate(KeysCorrect, KeysTyped);
}

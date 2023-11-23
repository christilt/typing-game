using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StatsManager : Singleton<StatsManager>
{
    public Accuracy Accuracy { get; } = new();
}

public class Accuracy
{
    public int KeysCorrect { get; private set; }
    public int KeysTyped { get; private set; }
    public float? Proportion
    {
        get
        {
            if (KeysTyped == 0)
                return null;

            return (float)KeysCorrect/(float)KeysTyped;
        }
    }

    public void LogCorrectKey()
    {
        KeysCorrect++;
        KeysTyped++;

        // TODO remove
        Debug.Log($"Correct: {this}");
    }

    public void LogIncorrectKey()
    {
        KeysTyped++;

        // TODO remove
        Debug.Log($"Incorrect: {this}");
    }

    public override string ToString()
    {
        if (!Proportion.HasValue)
            return "-";

        return $"Accuracy: {Proportion:P2} ({KeysCorrect}/{KeysTyped})";
    }
}
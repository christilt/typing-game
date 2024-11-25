using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class GameProgress
{
    public Dictionary<string, int> DifficultyHighestCompleteLevels = new();
    public override string ToString()
    {
        return $"{Environment.NewLine}{string.Join(Environment.NewLine, DifficultyHighestCompleteLevels.Select(kvp => $"{kvp.Key}:{Environment.NewLine}{string.Join(Environment.NewLine, kvp.Value)}"))}";
    }
}

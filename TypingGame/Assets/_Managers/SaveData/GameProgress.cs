using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class GameProgress
{
    public Dictionary<string, int> DifficultyHighestReachedLevels = new();
    public override string ToString()
    {
        return $"{Environment.NewLine}{string.Join(Environment.NewLine, DifficultyHighestReachedLevels.Select(kvp => $"{kvp.Key}:{Environment.NewLine}{string.Join(Environment.NewLine, kvp.Value)}"))}";
    }
}

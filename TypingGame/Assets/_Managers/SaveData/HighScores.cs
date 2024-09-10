using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class HighScores
{
    public Dictionary<string, List<HighScore>> LevelHighScores = new();

    public override string ToString()
    {
        return $"{Environment.NewLine}{string.Join(Environment.NewLine, LevelHighScores.Select(kvp => $"{kvp.Key}:{Environment.NewLine}{string.Join(Environment.NewLine, kvp.Value)}"))}";
    }
}

[Serializable]
public class HighScore
{
    public string Initials;
    public int Value;
    public string ColourHtmlString; // Reconsider serialising this if categorisation might change

    public override string ToString()
    {
        return $"{Initials}...{Value}";
    }
}
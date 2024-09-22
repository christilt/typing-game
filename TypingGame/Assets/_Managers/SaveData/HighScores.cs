using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class HighScores
{
    public Dictionary<string, List<HighScore>> LevelDifficultyHighScores = new();

    public override string ToString()
    {
        return $"{Environment.NewLine}{string.Join(Environment.NewLine, LevelDifficultyHighScores.Select(kvp => $"{kvp.Key}:{Environment.NewLine}{string.Join(Environment.NewLine, kvp.Value)}"))}";
    }
}

[Serializable]
public class HighScore
{
    public string Initials;
    public int Score;
    public int Minutes;
    public int Seconds;
    public string RankColourHtmlString; // Reconsider serialising this if categorisation might change
    public string TimeColourHtmlString; // Reconsider serialising this if categorisation might change

    public override string ToString()
    {
        return $"{Initials}...{Score}";
    }
}

public class EditableHighScores
{
    private readonly string _levelDifficultyKey;

    public EditableHighScores(HighScores highScores, string levelDifficultyKey, int? levelDifficultyNewEntryIndex)
    {
        HighScores = highScores;
        _levelDifficultyKey = levelDifficultyKey;
        LevelDifficultyNewEntryIndex = levelDifficultyNewEntryIndex;
    }

    public HighScores HighScores { get; }
    public List<HighScore> LevelDifficultyHighScores => HighScores.LevelDifficultyHighScores[_levelDifficultyKey];
    public int? LevelDifficultyNewEntryIndex { get; private set; }
    public void UpdateInitials(string playerInitials)
    {
        if (LevelDifficultyNewEntryIndex == null) throw new InvalidOperationException("Cannot update initials for highscores that do not have a new entry");

        HighScores.LevelDifficultyHighScores[_levelDifficultyKey][LevelDifficultyNewEntryIndex.Value].Initials = playerInitials;
    }
}
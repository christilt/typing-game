using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SaveDataManager : PersistentSingleton<SaveDataManager>
{
    [SerializeField] private int _maxHighScoreEntriesPerLevel;

    private BinaryFormatter _formatter;
    private string _highScoresPath;


    protected override void Awake()
    {
        base.Awake();

        _formatter = new BinaryFormatter();
        _highScoresPath = Path.Combine(Application.persistentDataPath, "highscores.bin");
    }

    public List<HighScore> SaveLevelHighScore(string levelId, string playerInitials, LevelStats stats)
    {
        var highScore = new HighScore
        {
            Initials = playerInitials,
            Score = stats.Score,
            RankColourHtmlString = ColorUtility.ToHtmlStringRGB(stats.Category.GetColour()),
            Minutes = stats.Speed.TimeTaken.Minutes,
            Seconds = stats.Speed.TimeTaken.Seconds,
            TimeColourHtmlString = ColorUtility.ToHtmlStringRGB(stats.Speed.Category.GetColour()),
        };

        var highScores = LoadHighScoresOrDefault() ?? new();

        if (highScores.LevelHighScores.TryGetValue(levelId, out var levelHighScores))
        {
            levelHighScores.Add(highScore);
            highScores.LevelHighScores[levelId] = levelHighScores
                .OrderByDescending(ps => ps.Score)
                .ThenBy(ps => ps.Initials)
                .Take(_maxHighScoreEntriesPerLevel)
                .ToList();
            Save(highScores);
        }
        else
        {
            highScores.LevelHighScores.Add(levelId, new List<HighScore>
            {
                highScore
            });
            Save(highScores);
        }

        return highScores.LevelHighScores[levelId];
    }

    public List<HighScore> LoadLevelHighScores(string levelId)
    {
        var highScores = LoadHighScoresOrDefault();
        if (highScores == null)
            return new();

        if (highScores.LevelHighScores.TryGetValue(levelId, out var levelHighScores))
        {
            return levelHighScores;
        }
        else
        {
            return new();
        }
    }

    private void Save(HighScores highScores)
    {
        using var stream = new FileStream(_highScoresPath, FileMode.Create);
        _formatter.Serialize(stream, highScores);

        // TODO: Remove
        Debug.Log($"HighScores saved at {_highScoresPath}: {highScores}");
    }


    private HighScores LoadHighScoresOrDefault()
    {
        if (!File.Exists(_highScoresPath))
            return null;

        using var stream = new FileStream(_highScoresPath, FileMode.Open);
        var highScores = (HighScores)_formatter.Deserialize(stream);

        // TODO: Remove
        Debug.Log($"HighScores loaded from {_highScoresPath}: {highScores}");

        return highScores;
    }
}
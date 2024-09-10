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
    private BinaryFormatter _formatter;
    private string _highScoresPath;

    protected override void Awake()
    {
        base.Awake();

        _formatter = new BinaryFormatter();
        _highScoresPath = Path.Combine(Application.persistentDataPath, "typinggame.highscores.bin");
    }

    public List<HighScore> SaveLevelHighScore(string levelId, string playerInitials, LevelStats stats)
    {
        var highScore = new HighScore
        {
            Initials = playerInitials,
            ColourHtmlString = ColorUtility.ToHtmlStringRGB(stats.Category.GetColour()),
            Value = stats.Score
        };

        HighScores highScores;
        if (!TryLoadHighScores(out highScores))
            highScores = new HighScores();

        if (highScores.LevelHighScores.TryGetValue(levelId, out var levelHighScores))
        {
            levelHighScores.Add(highScore);
            levelHighScores = levelHighScores
                .OrderByDescending(ps => ps.Value)
                .ThenBy(ps => ps.Initials)
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

    private void Save(HighScores highScores)
    {
        using var stream = new FileStream(_highScoresPath, FileMode.Create);
        _formatter.Serialize(stream, highScores);

        // TODO: Remove
        Debug.Log($"HighScores saved at {_highScoresPath}: {highScores}");
    }


    private bool TryLoadHighScores(out HighScores highScores)
    {
        highScores = null;

        if (!File.Exists(_highScoresPath))
            return false;

        using var stream = new FileStream(_highScoresPath, FileMode.Open);
        highScores = (HighScores)_formatter.Deserialize(stream);

        // TODO: Remove
        Debug.Log($"HighScores loaded from {_highScoresPath}: {highScores}");

        return true;
    }
}
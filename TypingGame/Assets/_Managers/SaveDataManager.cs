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
    private string _gameProgressPath;

    private GameProgress _cachedGameProgress;

    protected override void Awake()
    {
        base.Awake();

        _formatter = new BinaryFormatter();
        _highScoresPath = Path.Combine(Application.persistentDataPath, "highscores.bin");
        _gameProgressPath = Path.Combine(Application.persistentDataPath, "progress.bin");
    }

    public void SaveLastPlayerInitials(string playerInitials) => PlayerPrefs.SetString("player_last_initials", playerInitials);

    public string LoadLastPlayerInitials() => PlayerPrefs.GetString("player_last_initials", string.Empty);

    public EditableHighScores LoadHighScoresWithNewEntry(string levelId, Difficulty difficulty, LevelStats stats)
    {
        var levelDifficultyKey = LevelDifficultyKey(levelId, difficulty);

        var highScore = new HighScore
        {
            Initials = string.Empty,
            Score = stats.Score,
            RankColourHtmlString = ColorUtility.ToHtmlStringRGBA(stats.Category.GetColour()),
            Minutes = stats.Speed.TimeTaken.Minutes,
            Seconds = stats.Speed.TimeTaken.Seconds,
            TimeColourHtmlString = ColorUtility.ToHtmlStringRGBA(stats.Speed.Category.GetColour()),
        };

        var highScores = LoadHighScoresOrDefault() ?? new();
        int? newIndex = null;

        if (highScores.LevelDifficultyHighScores.TryGetValue(levelDifficultyKey, out var levelHighScores))
        {
            levelHighScores.Add(highScore); 
            levelHighScores = levelHighScores
                .OrderByDescending(ps => ps.Score)
                .ThenByDescending(ps => ps.Minutes)
                .ThenByDescending(ps => ps.Seconds)
                .ThenBy(ps => ps.Initials)
                .Take(_maxHighScoreEntriesPerLevel)
                .ToList();
            highScores.LevelDifficultyHighScores[levelDifficultyKey] = levelHighScores;
            var emptyIndicies = levelHighScores.Select((Score, Index) => (Score, Index)).Where(x => x.Score.Initials == string.Empty).ToList();
            if (emptyIndicies.Count > 1)
            {
                // TODO: Remove
                Debug.LogWarning($"More than 1 set of empty initials found in highscores: {highScores}");
            }
            else if (emptyIndicies.Count == 1)
            {
                newIndex = emptyIndicies.Single().Index;
            }
        }
        else
        {
            highScores.LevelDifficultyHighScores.Add(levelDifficultyKey, new List<HighScore>
            {
                highScore
            });
            newIndex = 0;
        }

        return new EditableHighScores(highScores, levelDifficultyKey, newIndex);
    }

    public void Save(HighScores highScores)
    {
        using var stream = new FileStream(_highScoresPath, FileMode.Create);
        _formatter.Serialize(stream, highScores);

        // TODO: Remove
        Debug.Log($"HighScores saved at {_highScoresPath}: {highScores}");
    }

    public GameProgress LoadGameProgress()
    {
        if (!File.Exists(_gameProgressPath))
            return new();

        if (_cachedGameProgress != null)
            return _cachedGameProgress;

        using var stream = new FileStream(_gameProgressPath, FileMode.Open);
        _cachedGameProgress = (GameProgress)_formatter.Deserialize(stream);

        // TODO: Remove
        Debug.Log($"GameProgress loaded from {_gameProgressPath}: {_cachedGameProgress}");

        return _cachedGameProgress;
    }

    public void Save(GameProgress gameProgress)
    {
        _cachedGameProgress = gameProgress;

        using var stream = new FileStream(_gameProgressPath, FileMode.Create);
        _formatter.Serialize(stream, gameProgress);

        // TODO: Remove
        Debug.Log($"GameProgress saved at {_gameProgressPath}: {gameProgress}");
    }

    private static string LevelDifficultyKey(string levelId, Difficulty difficulty) => $"{levelId}__{difficulty}";


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
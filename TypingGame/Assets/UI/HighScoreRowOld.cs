using UnityEngine;

public class HighScoreRowOld : MonoBehaviour
{
    [SerializeField] private TextInterpolatorLevelHighScoresRowOld _textInterpolator;

    public TextInterpolatorLevelHighScoresRowOld TextInterpolator => _textInterpolator;
}

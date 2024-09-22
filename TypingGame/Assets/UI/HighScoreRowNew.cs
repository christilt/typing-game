using TMPro;
using UnityEngine;

public class HighScoreRowNew : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TextInterpolatorLevelHighScoresRowNew _textInterpolator;

    public TextInterpolatorLevelHighScoresRowNew TextInterpolator => _textInterpolator;
    public TMP_InputField InitialsInput => _inputField;
}
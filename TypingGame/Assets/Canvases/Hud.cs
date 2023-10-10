using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        UpdateTextForState(GameManager.Instance.State);
        GameManager.Instance.OnStateChanging += UpdateTextForState;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStateChanging -= UpdateTextForState;
    }

    private void UpdateTextForState(GameState state)
    {
        switch (state)
        {
            case GameState.LevelStarting:
                _text.text = "Get ready";
                break;
            case GameState.LevelPlaying:
                _text.text = string.Empty;
                break;
            case GameState.LevelComplete:
                _text.text = "Level complete";
                break;
            case GameState.PlayerDying:
                _text.text = "Level failed";
                break;
            default:
                _text.text = string.Empty;
                break;
        }
    }
}
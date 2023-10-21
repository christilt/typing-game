﻿using TMPro;
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
        if (GameManager.Instance != null)
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
            case GameState.LevelWon:
                _text.text = "WIN";
                break;
            case GameState.LevelLost:
                _text.text = "LOSE";
                break;
            default:
                _text.text = string.Empty;
                break;
        }
    }
}
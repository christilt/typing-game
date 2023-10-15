using System;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PauseManager : Singleton<PauseManager>
{
    public event Action<PauseState> OnPauseChanging;
    public event Action<PauseState> OnPauseChanged;

    private PauseState _state = PauseState.Unpaused;

    public void Pause()
    {
        ChangeState(PauseState.Paused);
    }

    public void Unpause()
    {
        ChangeState(PauseState.Unpaused);
    }

    public void Slow()
    {
        ChangeState(PauseState.Slowed);
    }

    private void ChangeState(PauseState state)
    {
        OnPauseChanging?.Invoke(state);

        switch (state)
        {
            case PauseState.Paused:
                Debug.Log("Pausing");
                Time.timeScale = 0;
                break;
            case PauseState.Unpaused:
                Debug.Log("Unpausing");
                Time.timeScale = 1;
                break;
            case PauseState.Slowed:
                Debug.Log("Slowing");
                Time.timeScale = 0.25f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        _state = state;

        OnPauseChanged?.Invoke(state);
    }
}

public enum PauseState
{
    Paused,
    Unpaused,
    Slowed
}
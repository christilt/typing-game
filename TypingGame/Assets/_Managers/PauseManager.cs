﻿using System;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PauseManager : Singleton<PauseManager>
{
    public event Action<bool> OnPauseChanging;
    public event Action<bool> OnPauseChanged;

    public void Pause()
    {
        OnPauseChanging?.Invoke(true);
        Time.timeScale = 0;
        OnPauseChanged?.Invoke(true);
    }

    public void Unpause()
    {
        OnPauseChanging?.Invoke(false);
        Time.timeScale = 1;
        OnPauseChanged?.Invoke(false);
    }
}
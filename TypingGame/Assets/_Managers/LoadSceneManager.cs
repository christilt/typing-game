﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO do things like load level tiles in loading screen?
// Based on https://www.youtube.com/watch?v=3I5d2rUJ0pE
public class LoadSceneManager : PersistentSingleton<LoadSceneManager>
{
    private string _loadSceneName;
    private int? _loadBuildIndex;
    private AsyncOperation _loadingAsyncOperation;

    public float LoadingProgress => _loadingAsyncOperation?.progress ?? 0;

    public void ReloadLevel()
    {
        var currentSceneName = SceneManager.GetActiveScene().name;
        StartLoad(currentSceneName);
    }

    public void LoadingSceneLoad()
    {
        if (!LoadingSceneIsCurrent)
            throw new InvalidOperationException($"Not valid outside of Loading scene: {nameof(LoadingSceneLoad)}");

        if (_loadSceneName == null && _loadBuildIndex == null)
            throw new InvalidOperationException($"Neither {nameof(_loadSceneName)} nor {nameof(_loadBuildIndex)} have been set");

        if (_loadSceneName != null)
        {
            _loadingAsyncOperation = SceneManager.LoadSceneAsync(_loadSceneName);
            _loadSceneName = null;
        }
        else if (_loadBuildIndex != null)
        {
            _loadingAsyncOperation = SceneManager.LoadSceneAsync(_loadBuildIndex.Value);
            _loadBuildIndex = null;
        }
        // See https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.html
        _loadingAsyncOperation.allowSceneActivation = false;
        StartCoroutine(AwaitSceneLoadedCoroutine());


        IEnumerator AwaitSceneLoadedCoroutine()
        {
            while (_loadingAsyncOperation.progress < 0.9f)
                yield return null;

            SceneHider.Instance.EndOfSceneFadeOut(() =>
            {
                _loadingAsyncOperation.allowSceneActivation = true;
            });
        }
    }

    public void StartLoad(string sceneName) => StartLoad(() =>
    {
        _loadSceneName = sceneName;
        _loadBuildIndex = null;
    });
    public void StartLoad(int sceneBuildIndex) => StartLoad(() =>
    {
        _loadSceneName = null;
        _loadBuildIndex = sceneBuildIndex;
    });

    private void StartLoad(Action setSceneNameOrSceneBuildIndex)
    {
        if (LoadingSceneIsCurrent)
            throw new InvalidOperationException($"Not valid from Loading scene: {nameof(StartLoad)}");

        setSceneNameOrSceneBuildIndex();

        var isExitingGameplay = _loadSceneName == SceneNames.MainMenu;
        if (SceneHider.Instance != null)
        {
            SceneHider.Instance.EndOfSceneFadeOut(() =>
            {
                GameplayManager.Instance?.SceneEnding(isExitingGameplay);
                SceneManager.LoadScene(SceneNames.Loading);
            });
        }
        else
        {
            GameplayManager.Instance?.SceneEnding(isExitingGameplay);
            SceneManager.LoadScene(SceneNames.Loading);
        }
    }

    private bool LoadingSceneIsCurrent => SceneManager.GetActiveScene().name == SceneNames.Loading;
}
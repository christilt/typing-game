using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Based on https://www.youtube.com/watch?v=3I5d2rUJ0pE
public class LoadingManager : PersistentSingleton<LoadingManager>
{
    private string _loadSceneName;
    private AsyncOperation _loadingAsyncOperation;

    public float? LoadingProgress => _loadingAsyncOperation?.progress;

    public void ReloadLevel()
    {
        var currentSceneName = SceneManager.GetActiveScene().name;
        StartLoad(currentSceneName);
    }

    public void LoadingSceneLoad()
    {
        if (!LoadingSceneIsCurrent)
            throw new InvalidOperationException($"Not valid outside of Loading scene: {nameof(LoadingSceneLoad)}");

        if (_loadSceneName == null)
            throw new InvalidOperationException($"{nameof(_loadSceneName)} has not been set");

        StartCoroutine(LoaderLoadCoroutine());


        IEnumerator LoaderLoadCoroutine()
        {
            _loadingAsyncOperation = SceneManager.LoadSceneAsync(_loadSceneName);

            while (!_loadingAsyncOperation.isDone)
                yield return null;

            _loadSceneName = null;
        }
    }

    private void StartLoad(string sceneName)
    {
        if (LoadingSceneIsCurrent)
            throw new InvalidOperationException($"Not valid from Loading scene: {nameof(StartLoad)}");

        _loadSceneName = sceneName;

        SceneManager.LoadScene(SceneNames.Loading);
    }

    private bool LoadingSceneIsCurrent => SceneManager.GetActiveScene().name == SceneNames.Loading;

    private class SceneNames
    {
        public const string Loading = "Loading";
    }
}
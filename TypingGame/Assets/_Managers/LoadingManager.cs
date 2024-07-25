using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO do things like load level tiles in loading screen?
// Based on https://www.youtube.com/watch?v=3I5d2rUJ0pE
public class LoadingManager : PersistentSingleton<LoadingManager>
{
    private string _loadSceneName;
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

        if (_loadSceneName == null)
            throw new InvalidOperationException($"{nameof(_loadSceneName)} has not been set");


        _loadingAsyncOperation = SceneManager.LoadSceneAsync(_loadSceneName);
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

    public void StartLoad(string sceneName)
    {
        if (LoadingSceneIsCurrent)
            throw new InvalidOperationException($"Not valid from Loading scene: {nameof(StartLoad)}");

        _loadSceneName = sceneName;

        SceneHider.Instance.EndOfSceneFadeOut(() =>
        {
            SceneManager.LoadScene(SceneNames.Loading);
        });
    }

    private bool LoadingSceneIsCurrent => SceneManager.GetActiveScene().name == SceneNames.Loading;
}
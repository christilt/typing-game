using System;
using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO do things like load level tiles in loading screen?
// Based on https://www.youtube.com/watch?v=3I5d2rUJ0pE
public class LoadSceneManager : PersistentSingleton<LoadSceneManager>
{
    private string _sceneNameToLoad;
    private int? _sceneBuildIndexToLoad;
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

        if (_sceneNameToLoad == null && _sceneBuildIndexToLoad == null)
            throw new InvalidOperationException($"Neither {nameof(_sceneNameToLoad)} nor {nameof(_sceneBuildIndexToLoad)} have been set");

        if (_sceneNameToLoad != null)
        {
            _loadingAsyncOperation = SceneManager.LoadSceneAsync(_sceneNameToLoad, LoadSceneMode.Additive);
        }
        else if (_sceneBuildIndexToLoad != null)
        {
            _loadingAsyncOperation = SceneManager.LoadSceneAsync(_sceneBuildIndexToLoad.Value, LoadSceneMode.Additive);
        }
        // See https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.html
        _loadingAsyncOperation.allowSceneActivation = false;
        StartCoroutine(AwaitSceneLoadedCoroutine());

        IEnumerator AwaitSceneLoadedCoroutine()
        {
            // Apparently .progress doesn't reach 1.0 until .allowSceneActivation is set
            // So begin fading out shortly before 1.0 and set .allowSceneActivation after
            while (_loadingAsyncOperation.progress < 0.9f)
                yield return null;

            SceneHider.Instance.EndOfSceneFadeOut(() =>
            {
                _loadingAsyncOperation.allowSceneActivation = true;
            });

            // Unloading the previous scene requires the previous scene no longer being the active one
            // Scene object to get root game objects but also cannot .GetScene before .isDone
            // So wait for .isDone, then .GetScene
            while (!_loadingAsyncOperation.isDone)
                yield return null;

            var sceneToLoad = _sceneNameToLoad != null ? SceneManager.GetSceneByName(_sceneNameToLoad)
                : SceneManager.GetSceneByBuildIndex(_sceneBuildIndexToLoad.Value);

            var sceneToLoadRootObjects = sceneToLoad.GetRootGameObjects();
            foreach (GameObject obj in sceneToLoadRootObjects)
                obj.SetActive(true);

            var unloadingAsyncOperation = SceneManager.UnloadSceneAsync(SceneNames.Loading);

            while (!unloadingAsyncOperation.isDone)
                yield return null;

            // Docs state this is not called as part of .LoadScene when mode is Additive, we must call it ourselves
            Resources.UnloadUnusedAssets();
            _sceneNameToLoad = null;
            _sceneBuildIndexToLoad = null;
        }
    }

    public void StartLoad(string sceneName) => StartLoad(() =>
    {
        _sceneNameToLoad = sceneName;
        _sceneBuildIndexToLoad = null;
    });
    public void StartLoad(int sceneBuildIndex) => StartLoad(() =>
    {
        _sceneNameToLoad = null;
        _sceneBuildIndexToLoad = sceneBuildIndex;
    });

    private void StartLoad(Action setSceneNameOrSceneBuildIndex)
    {
        if (LoadingSceneIsCurrent)
            throw new InvalidOperationException($"Not valid from Loading scene: {nameof(StartLoad)}");

        setSceneNameOrSceneBuildIndex();

        var isExitingGameplay = _sceneNameToLoad == SceneNames.MainMenu;
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
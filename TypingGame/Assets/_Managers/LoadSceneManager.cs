using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : PersistentSingleton<LoadSceneManager>
{
    private string _sceneNameToLoad;
    private int? _sceneBuildIndexToLoad;
    private AsyncOperation _loadingAsyncOperation;

    public event Action OnLoadComplete;

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

        StartCoroutine(AwaitSceneLoadedCoroutine());

        IEnumerator AwaitSceneLoadedCoroutine()
        {
            // See https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.html - we cannot get the scene until the async operation is complete
            // However, objects in the scene need additional loading before the scene is usable
            // So we allow the scene to be activated, and prevent additive scene objects from appearing until all additional loading in the scene is complete.
            // We do this by:
            //   Using a culling mask on the Loading scene camera, for Loading objects only
            //   Only activating the scene camera once loading is complete (signalling completion on the GameplayManager

            while (!_loadingAsyncOperation.isDone)
                yield return null;

            var sceneToLoad = _sceneNameToLoad != null ? SceneManager.GetSceneByName(_sceneNameToLoad)
                : SceneManager.GetSceneByBuildIndex(_sceneBuildIndexToLoad.Value);

            var sceneToLoadRootObjects = sceneToLoad.GetRootGameObjects();
            var sceneSlowLoadingObjects = sceneToLoadRootObjects.SelectMany(o => o.GetComponentsInChildren<ILoadsSlowly>(includeInactive: true));

            var toLoadCount = sceneSlowLoadingObjects.Count(o => !o.IsLoaded);
            while (toLoadCount > 0)
            {
                yield return new WaitForEndOfFrame();
                toLoadCount = sceneSlowLoadingObjects.Count(o => !o.IsLoaded);
            }

            // Avoid fading out until all loading is complete.  Otherwise frames are dropped during the fade
            var info = new LoadingSceneInfo();
            SceneHiderLoadingScreen.Instance.EndOfSceneFadeOut(() =>
            {
                info.LoadingFadeOutComplete = true;
            });

            while (!info.LoadingFadeOutComplete)
                yield return null;

            var unloadingAsyncOperation = SceneManager.UnloadSceneAsync(SceneNames.Loading);

            while (!unloadingAsyncOperation.isDone)
                yield return null;

            OnLoadComplete?.Invoke();

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
    private class LoadingSceneInfo
    {
        public bool LoadingFadeOutComplete { get; set; }
    }
}
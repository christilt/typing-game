using UnityEngine.SceneManagement;

public static class SceneHelper
{
    public static bool IsSceneLoadedAdditively(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        var activeScene = SceneManager.GetActiveScene();
        return scene.isLoaded && scene.buildIndex != activeScene.buildIndex;
    }
}
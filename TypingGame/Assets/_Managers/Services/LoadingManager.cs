using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : PersistentSingleton<LoadingManager>
{
    public void ReloadLevel()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
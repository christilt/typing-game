using UnityEngine;

public class MenuStart : MonoBehaviour
{
    private void Start()
    {
        // TODO: Do this in a manager with states
        SceneHider.Instance.StartOfSceneFadeIn();
    }

    public void OnNewGameButton()
    {
        LoadingManager.Instance.StartLoad(LoadingManager.SceneNames.NewGameLevel);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}

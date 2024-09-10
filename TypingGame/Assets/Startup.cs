using UnityEngine;

public class Startup : MonoBehaviour
{
    private void Start()
    {
        LoadSceneManager.Instance.StartLoad(SceneNames.MainMenu);
    }
}

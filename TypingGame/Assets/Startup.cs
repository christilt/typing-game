using UnityEngine;

public class Startup : MonoBehaviour
{
    private void Start()
    {
        LoadingManager.Instance.StartLoad(SceneNames.MainMenu);
    }
}

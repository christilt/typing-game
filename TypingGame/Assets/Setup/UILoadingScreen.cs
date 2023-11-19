using UnityEngine;
using UnityEngine.UI;

public class UILoadingScreen : MonoBehaviour
{
    [SerializeField] private Image _image;

    private void Start()
    {
        LoadingManager.Instance.LoadingSceneLoad();
    }

    private void Update()
    {
        var progress = LoadingManager.Instance.LoadingProgress;
        if (progress == null)
            return;

        UpdateProgress(progress.Value);
    }

    private void UpdateProgress(float progress)
    {
        _image.fillAmount = progress;
    }
}
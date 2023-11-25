using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingScreen : MonoBehaviour
{
    [SerializeField] private Image _image;

    private void Start()
    {
        SceneHider.Instance.StartOfSceneFadeIn(() =>
        {
            LoadingManager.Instance.LoadingSceneLoad();
        });
    }

    private void Update()
    {
        var progress = LoadingManager.Instance.LoadingProgress;
        UpdateProgress(progress);
    }

    private void UpdateProgress(float progress)
    {
        _image.DOFillAmount(progress, 0.25f);
    }
}
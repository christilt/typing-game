using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingScreen : MonoBehaviour
{
    [SerializeField] private Image _image;

    private Tween _tween;

    private void Start()
    {
        SceneHider.Instance.StartOfSceneFadeIn(() =>
        {
            LoadSceneManager.Instance.LoadingSceneLoad();
        });
    }

    private void OnDestroy()
    {
        _tween?.Kill();
    }

    private void Update()
    {
        var progress = LoadSceneManager.Instance.LoadingProgress;
        UpdateProgress(progress);
    }

    private void UpdateProgress(float progress)
    {
        if (progress <= _image.fillAmount)
            return;

        _tween?.Kill();
        _tween = _image.DOFillAmount(progress, 0.1f).SetUpdate(true);
    }
}
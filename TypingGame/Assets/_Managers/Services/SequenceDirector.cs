using System;
using UnityEngine;

public class SequenceDirector : MonoBehaviour
{
    [SerializeField] private VCameraWithShake _sequenceCamera;
    [SerializeField] private Hider _levelHider;

    public void LevelWinning(Action onComplete)
    {
        PromoteCamera();
        HideLevel(2, onComplete);
    }

    public void PlayerDying(Action onComplete)
    {
        PromoteCamera();
        HideLevel(2.5f, onComplete);
    }

    public void PlayerExploding()
    {
        _sequenceCamera.VeryQuickShake(unscaledTime: true);
    }

    private void PromoteCamera() => _sequenceCamera.Camera.Priority = 110;

    private void HideLevel(float duration, Action onComplete) => _levelHider.HideCompletely(duration, onComplete, unscaled: true);
}
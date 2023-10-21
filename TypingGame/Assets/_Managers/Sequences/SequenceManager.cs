using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    [SerializeField] private VCameraWithShake _sequenceCamera;
    [SerializeField] private Hider _levelHider;

    public void LevelWinning()
    {
        PromoteCamera();
        HideLevel(2);
    }

    public void PlayerDying()
    {
        PromoteCamera();
        HideLevel();
    }

    public void PlayerExploding()
    {
        _sequenceCamera.QuickShake();
    }

    private void PromoteCamera() => _sequenceCamera.Camera.Priority = 110;

    private void HideLevel(int duration = 4) => _levelHider.Hide(duration, unscaled: true);
}
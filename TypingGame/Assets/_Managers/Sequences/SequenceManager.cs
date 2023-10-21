using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    [SerializeField] private VCameraWithShake _sequenceCamera;
    [SerializeField] private Hider _levelHider;

    private void Start()
    {
        _sequenceCamera.Camera.Follow = Player.Instance.VisualTransform;
    }

    public void LevelCompleting()
    {
        PromoteCamera();
        HideLevel();
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

    private void HideLevel() => _levelHider.Hide(4);
}
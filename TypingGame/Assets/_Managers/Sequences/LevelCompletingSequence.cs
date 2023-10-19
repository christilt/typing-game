using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletingSequence : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _sequenceCamera;
    [SerializeField] private Hider _levelHider;

    private void Start()
    {
        _sequenceCamera.Follow = Player.Instance.VisualTransform;
    }

    public void Play()
    {
        _sequenceCamera.Priority = 110;
        _levelHider.Hide(2);
    }
}
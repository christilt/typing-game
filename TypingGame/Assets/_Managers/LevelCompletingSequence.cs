using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletingSequence : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _sequenceCamera;
    [SerializeField] private Hider _hider;

    private void Start()
    {
        _sequenceCamera.Follow = Player.Instance.VisualTransform;
    }

    public void Play()
    {
        _sequenceCamera.gameObject.SetActive(true);
        _hider.Hide(2);
    }
}
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class VCameraFollowPlayer : MonoBehaviour
{
    public CinemachineVirtualCamera Camera { get; private set; }

    private void Start()
    {
        Camera.Follow = Player.Instance.VisualTransform;
    }

    private void Awake()
    {
        Camera = GetComponent<CinemachineVirtualCamera>();
    }
}
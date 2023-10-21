using UnityEngine;

[RequireComponent(typeof(VCameraFollowPlayer))]
public class PlayingVCamera : Singleton<PlayingVCamera>
{
    [SerializeField] private VCameraWithShake _vCameraWithShake;

    public VCameraWithShake Camera => _vCameraWithShake;
}
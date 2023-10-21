using UnityEngine;

[RequireComponent(typeof(VCameraFollowPlayer))]
public class SequenceVCamera : Singleton<SequenceVCamera>
{
    [SerializeField] private VCameraWithShake _vCameraWithShake;

    public VCameraWithShake Shake => _vCameraWithShake;
}
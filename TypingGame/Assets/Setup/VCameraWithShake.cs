using Cinemachine;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class VCameraWithShake : MonoBehaviour
{
    public CinemachineVirtualCamera Camera { get; private set; }

    public void QuickShake() => Shake(5, 0.1f);

    public void Shake(float intensity, float time)
    {
        StartCoroutine(ShakeCoroutine());
        
        IEnumerator ShakeCoroutine()
        {
            var perlin = Camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            perlin.m_AmplitudeGain = intensity;
            while (time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            perlin.m_AmplitudeGain = 0;
        }
    }

    private void Awake()
    {
        Camera = GetComponent<CinemachineVirtualCamera>();
    }
}
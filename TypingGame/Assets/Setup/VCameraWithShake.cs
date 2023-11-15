using Cinemachine;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class VCameraWithShake : MonoBehaviour
{
    public CinemachineVirtualCamera Camera { get; private set; }

    public void VeryQuickShake(bool unscaledTime = false) => Shake(4, 0.3f, unscaledTime);
    public void QuickShake(bool unscaledTime = false) => Shake(5, 0.5f, unscaledTime);

    public void Shake(float intensity, float time, bool unscaledTime = false)
    {
        StartCoroutine(ShakeCoroutine());
        
        IEnumerator ShakeCoroutine()
        {
            var perlin = Camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            perlin.m_AmplitudeGain = intensity;
            while (time > 0)
            {
                time -= unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
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
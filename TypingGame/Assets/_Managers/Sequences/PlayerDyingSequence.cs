﻿using UnityEngine;

public class PlayerDyingSequence : MonoBehaviour
{
    [SerializeField] private VCameraWithShake _sequenceCamera;
    [SerializeField] private Hider _levelHider;

    private void Start()
    {
        _sequenceCamera.Camera.Follow = Player.Instance.VisualTransform;
    }

    public void Play()
    {
        _sequenceCamera.Camera.Priority = 110;
        _levelHider.Hide(2);
        this.DoAfterSecondsRealtime(2, () => _sequenceCamera.QuickShake());
    }
}
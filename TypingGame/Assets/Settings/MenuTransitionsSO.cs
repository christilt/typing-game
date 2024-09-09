using DG.Tweening;
using UnityEngine;

[CreateAssetMenu]
public class MenuTransitionsSO : ScriptableObject
{
    [SerializeField] public Ease _moveInIntroEase;
    public Ease MoveInIntroEase => _moveInIntroEase;

    [SerializeField] public Ease _moveInPositiveEase;
    public Ease MoveInPositiveEase => _moveInPositiveEase;

    [SerializeField] public Ease _moveInNegativeEase;
    public Ease MoveInNegativeEase => _moveInNegativeEase;

    [SerializeField] public Ease _moveOutEase;
    public Ease MoveOutEase => _moveOutEase;

    [SerializeField] public Ease _fadeInEase;
    public Ease FadeInEase => _fadeInEase;

    [SerializeField] public Ease _fadeOutEase;
    public Ease FadeOutEase => _fadeOutEase;

    [SerializeField] private float _moveInIntroDuration;
    public float MoveInIntroDuration => _moveInIntroDuration;

    [SerializeField] private float _moveInPositiveDuration;
    public float MoveInPositiveDuration => _moveInPositiveDuration;

    [SerializeField] private float _moveInNegativeDuration;
    public float MoveInNegativeDuration => _moveInNegativeDuration;

    [SerializeField] private float _moveOutDuration;
    public float MoveOutDuration => _moveOutDuration;

    [SerializeField] private float _intervalDuration;
    public float IntervalDuration => _intervalDuration;

    [SerializeField] private float _fadeOutDuration;
    public float FadeOutDuration => _fadeOutDuration;

    [SerializeField] private float _fadeInDuration;
    public float FadeInDuration => _fadeInDuration;
}

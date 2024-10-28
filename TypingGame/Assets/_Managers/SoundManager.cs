using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SoundManager : PersistentSingleton<SoundManager>
{
    [SerializeField] private AudioSource _sfxSourceGlobal;
    [SerializeField] private AudioSource _musicSourceGlobal;

    [SerializeField] private AudioClip _soundEffectAchievement;
    [SerializeField] private AudioClip _soundEffectCollectGoal;
    [SerializeField] private AudioClip _soundEffectGameStart;
    [SerializeField] private AudioClip _soundEffectMenuMove;
    [SerializeField] private AudioClip _soundEffectPlayerAttack;
    [SerializeField] private AudioClip _soundEffectStatusEffectKillsEnemies;
    [SerializeField] private AudioClip _soundEffectStatusEffectNegative;
    [SerializeField] private AudioClip _soundEffectStatusEffectPositive;
    [SerializeField] private AudioClip _soundEffectTypeHit;
    [SerializeField] private AudioClip _soundEffectTypeMiss;

    [SerializeField] private AudioClip _musicInGame;

    [SerializeField] private float _musicFadeDuration;

    private float _musicOriginalVolume;


    public void PlayAchievement() => _sfxSourceGlobal.PlayOneShot(_soundEffectAchievement);
    public void PlayCollectGoal() => _sfxSourceGlobal.PlayOneShot(_soundEffectCollectGoal);
    public void PlayGameStart() => _sfxSourceGlobal.PlayOneShot(_soundEffectGameStart);
    public void PlayMenuMove() => _sfxSourceGlobal.PlayOneShot(_soundEffectMenuMove);
    public void PlayPlayerAttack() => _sfxSourceGlobal.PlayOneShot(_soundEffectPlayerAttack);
    public void PlayStatusEffectKillsEnemies() => _sfxSourceGlobal.PlayOneShot(_soundEffectStatusEffectKillsEnemies);
    public void PlayStatusEffectNegative() => _sfxSourceGlobal.PlayOneShot(_soundEffectStatusEffectNegative);
    public void PlayStatusEffectPositive() => _sfxSourceGlobal.PlayOneShot(_soundEffectStatusEffectPositive);
    public void PlayTypeHit() => _sfxSourceGlobal.PlayOneShot(_soundEffectTypeHit);
    public void PlayTypeMiss() => _sfxSourceGlobal.PlayOneShot(_soundEffectTypeMiss);


    public void StartMusicInGame()
    {
        if (_musicSourceGlobal.clip == _musicInGame && _musicSourceGlobal.isPlaying)
            return;

        _musicSourceGlobal.clip = _musicInGame;
        _musicSourceGlobal.Play();
    }

    public void StopMusicInGame()
    {
        _musicOriginalVolume = _musicSourceGlobal.volume;
        _musicSourceGlobal
            .DOFade(0, _musicFadeDuration)
            .OnComplete(() =>
            {
                _musicSourceGlobal.Stop();
                _musicSourceGlobal.volume = _musicOriginalVolume;
            });
    }

}
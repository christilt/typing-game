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

    [SerializeField] private AudioClip _soundEffectGameStart;
    [SerializeField] private AudioClip _soundEffectMenuMove;
    [SerializeField] private AudioClip _soundEffectPlayerAttack;
    [SerializeField] private AudioClip _soundEffectType;
    [SerializeField] private AudioClip _soundEffectTypeMiss;

    [SerializeField] private AudioClip _musicInGame;


    public void PlayGameStart() => _sfxSourceGlobal.PlayOneShot(_soundEffectGameStart);
    public void PlayMenuMove() => _sfxSourceGlobal.PlayOneShot(_soundEffectMenuMove);
    public void PlayPlayerAttack() => _sfxSourceGlobal.PlayOneShot(_soundEffectPlayerAttack);
    public void PlayType() => _sfxSourceGlobal.PlayOneShot(_soundEffectType);
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
        _musicSourceGlobal.Stop();
    }

    // TODO: Pause music / fade in out music?

}
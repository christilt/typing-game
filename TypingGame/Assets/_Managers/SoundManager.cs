using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SoundManager : PersistentSingleton<SoundManager>
{
    [SerializeField] private AudioSource _audioSourceGlobal;

    [SerializeField] private AudioClip _soundEffectGameStart;
    [SerializeField] private AudioClip _soundEffectPlayerAttack;
    [SerializeField] private AudioClip _soundEffectType;
    [SerializeField] private AudioClip _soundEffectTypeMiss;

    public void PlayGameStart() => _audioSourceGlobal.PlayOneShot(_soundEffectGameStart);
    public void PlayPlayerAttack() => _audioSourceGlobal.PlayOneShot(_soundEffectPlayerAttack);
    public void PlayType() => _audioSourceGlobal.PlayOneShot(_soundEffectType);
    public void PlayTypeMiss() => _audioSourceGlobal.PlayOneShot(_soundEffectTypeMiss);

}
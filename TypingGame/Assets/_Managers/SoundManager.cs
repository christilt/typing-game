using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SoundManager : PersistentSingleton<SoundManager>
{
    [SerializeField] private AudioSource _audioSourceGlobal;

    [SerializeField] private AudioClip _soundEffectType;

    public void PlayType()
    {
        _audioSourceGlobal.PlayOneShot(_soundEffectType);
    }
}
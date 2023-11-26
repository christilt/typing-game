using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SettingsManager : Singleton<SettingsManager>
{
    [SerializeField] private LevelSettings _levelSettings;
    public LevelSettings LevelSettings => _levelSettings;

    [SerializeField] private Palette _palette;
    public Palette Palette => _palette;
}
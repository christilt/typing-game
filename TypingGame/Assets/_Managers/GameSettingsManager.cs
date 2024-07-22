using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameSettingsManager : PersistentSingleton<GameSettingsManager>
{
    public DifficultySO Difficulty { get; set; }

    [SerializeField] private PaletteSO _palette;
    public PaletteSO Palette => _palette;
}
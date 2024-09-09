using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TextInterpolatorLevelIntro : TextInterpolator
{
    protected override object[] GetTextArgs()
    {
        return new object[]
        {
            LevelSettingsManager.Instance.LevelSettings.LevelName
        };
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TextInterpolatorLevelStats : TextInterpolator
{
    protected override object[] GetTextArgs()
    {
        return new object[]
        {
            ColorUtility.ToHtmlStringRGB(new(255, 0, 0)),
            ColorUtility.ToHtmlStringRGB(new(255, 255, 0)),
            ColorUtility.ToHtmlStringRGB(new(255, 0, 255)),
            0.986f,
            new TimeSpan(0, 2, 15),
            35,
            40
        };
    }
}
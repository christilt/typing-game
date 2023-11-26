using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class TextHelper
{
    public static string WithPixelatedMonospaceText(string text, int mspace = 56)
    {
        return $"<mspace={mspace}>{text}";
    }

    public static string WithColour(string text, Color color)
    {
        var colorString = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{colorString}>{text}</color>";
    }
}
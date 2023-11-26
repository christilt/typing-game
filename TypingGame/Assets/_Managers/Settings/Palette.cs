using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu]
public class Palette : ScriptableObject
{
    [SerializeField] private Color _greatColour;
    public Color GreatColour => _greatColour;
    [SerializeField] private Color _goodColour;
    public Color GoodColour => _goodColour;
    [SerializeField] private Color _averageColour;
    public Color AverageColour => _averageColour;
    [SerializeField] private Color _badColour;
    public Color BadColour => _badColour;
}
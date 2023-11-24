using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu]
public class LevelSettings : ScriptableObject
{
    [SerializeField] private string _namePart1;
    [SerializeField] private string _namePart2;
    [SerializeField] private string _namePartSeparator;
    public string LevelName => $"{_namePart1}{_namePartSeparator}{_namePart2}";

    [SerializeField] private float _benchmarkDurationSeconds;
    public float BenchmarkDurationSeconds => _benchmarkDurationSeconds;
}
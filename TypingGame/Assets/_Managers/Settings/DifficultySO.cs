﻿using UnityEngine;

[CreateAssetMenu]
public class DifficultySO : ScriptableObject
{
    [SerializeField] private Difficulty _difficulty;
    public Difficulty Difficulty => _difficulty;

    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private float _defaultSpeedModifier;
    public float DefaultSpeedModifier => _defaultSpeedModifier;
}

public enum Difficulty
{
    Normal,
    Challenge,
    Extreme
}
using UnityEngine;

[CreateAssetMenu]
public class CharacterSetSO : ScriptableObject
{
    [SerializeField] private string _characters;
    public string Characters => _characters;

    [SerializeField] private string _name;
    public string Name => _name;
}
using UnityEngine;

// TODO not used - remove?
public class CollectableSO : ScriptableObject
{
    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;

    [SerializeField] private string _name;
    public string Name => _name;    
}
using System;
using UnityEngine;

public class UnitBoundary : MonoBehaviour
{
    [SerializeField] private Renderer[] _areas;

    private void Awake()
    {
        if (_areas.Length == 0)
        {
            throw new ArgumentException($"{gameObject.name}: {nameof(_areas)} should not be empty");
        }
    }

    private void Start()
    {
        foreach(var area in _areas)
        {
            area.transform.parent = this.transform.parent;
        }
    }

    public bool Contains(Vector3 position)
    {
        foreach(var area in _areas)
        {
            if (area.bounds.Contains(position)) 
                return true;
        }
        return false;
    }
}
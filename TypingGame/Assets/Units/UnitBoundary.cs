using System;
using System.Linq;
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
        if (!Contains(transform.position))
        {
            // CONSIDER: If this misfires could be because we're not checking the centre
            throw new ArgumentException($"{gameObject.name}: {nameof(_areas)} should contain object"); 
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

    public bool CopyTo(UnitBoundary other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        other._areas = _areas.ToArray();
        return true;
    }
}
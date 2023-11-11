using UnityEngine;

public struct PositionWithCentre
{
    public PositionWithCentre(Vector3 position, Vector3 centre)
    {
        Position = position;
        Centre = centre;
    }

    public Vector3 Position { get; }
    public Vector3 Centre { get; }
}
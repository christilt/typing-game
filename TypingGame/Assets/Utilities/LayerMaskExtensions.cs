using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool HasLayer(this LayerMask layerMask, int layer)
    {
        // See https://forum.unity.com/threads/checking-if-a-layer-is-in-a-layer-mask.1190230/
        return (layerMask.value & (1 << layer)) != 0;
    }
}

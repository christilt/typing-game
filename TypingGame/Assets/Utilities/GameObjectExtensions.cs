using System.Collections;
using UnityEngine;

public static class GameObjectExtensions
{
    public static void DestroyWithChildren(this MonoBehaviour monoBehaviour, GameObject target)
    {
        monoBehaviour.StartCoroutine(DestroyWithChildrenCoroutine());

        IEnumerator DestroyWithChildrenCoroutine()
        {
            while (target.transform.childCount > 0)
            {
                GameObject.Destroy(target.transform.GetChild(0).gameObject);
                yield return null;
            }

            GameObject.Destroy(target.gameObject);
        }
    }
}
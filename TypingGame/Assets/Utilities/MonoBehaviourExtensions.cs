using System;
using System.Collections;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static void DoAfterSeconds(this MonoBehaviour monoBehaviour, float seconds, Action action)
    {
        monoBehaviour.StartCoroutine(DoAfterSecondsCoroutine()); 
        
        IEnumerator DoAfterSecondsCoroutine()
        {
            yield return new WaitForSeconds(seconds);

            action();
        }
    }
    public static void DoAfterSecondsRealtime(this MonoBehaviour monoBehaviour, float seconds, Action action)
    {
        monoBehaviour.StartCoroutine(DoAfterSecondsRealtimeCoroutine());

        IEnumerator DoAfterSecondsRealtimeCoroutine()
        {
            yield return new WaitForSecondsRealtime(seconds);

            action();
        }
    }

    public static void DoNextFrame(this MonoBehaviour monoBehaviour, Action action)
    {
        monoBehaviour.StartCoroutine(DoNextFrameCoroutine());

        IEnumerator DoNextFrameCoroutine()
        {
            yield return null;

            action();
        }
    }

    public static void DestroyChildren(this MonoBehaviour monoBehaviour, GameObject target)
    {
        monoBehaviour.StartCoroutine(DestroyChildrenCoroutine());

        IEnumerator DestroyChildrenCoroutine()
        {
            while (target.transform.childCount > 0)
            {
                GameObject.Destroy(target.transform.GetChild(0).gameObject);
                yield return null;
            }
        }
    }
}
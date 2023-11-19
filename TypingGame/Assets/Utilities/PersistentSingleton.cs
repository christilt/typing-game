using UnityEngine;

public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        transform.parent = null; // DontDestroyOnLoad only works for root game objects (see https://docs.unity3d.com/ScriptReference/Object.DontDestroyOnLoad.html)
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}
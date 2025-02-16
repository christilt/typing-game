using System;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class Pool<T> : ObjectPool<T> where T : class
{
    protected int _defaultCapacity;
    protected Scene _scene;

    public Pool(
        Func<T> createFunc,
        Scene scene,
        Action<T> actionOnGet = null,
        Action<T> actionOnRelease = null,
        Action<T> actionOnDestroy = null,
        bool collectionCheck = true,
        int defaultCapacity = 10,
        int maxSize = 10000) : base(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize)
    {
        _defaultCapacity = defaultCapacity;
        _scene = scene;
    }

    // Ensure objects are ready.  ObjectPool<T> doesn't do this
    public void Load()
    {
        for (var i = 0; i < _defaultCapacity; i++)
        {
            var obj = Get();

            // Ensure objects were created in the intended scene
            if (obj is MonoBehaviour monoBehaviour && monoBehaviour.gameObject.scene.name != _scene.name)
            {
                SceneManager.MoveGameObjectToScene(monoBehaviour.gameObject, _scene);
            }

            Release(obj);
        }
    }
}
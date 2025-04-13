using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;


public class PlayerGoalPointer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _fadeDuration;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    public static PlayerGoalPointer Instantiate(PlayerGoalPointer prefab, Vector3 position, Quaternion rotation, Transform transform, Color color)
    {
        prefab.gameObject.SetActive(false);
        var obj = Instantiate(prefab, position, rotation, transform);
        obj.SpriteRenderer.color = color;
        obj.gameObject.SetActive(true);

        return obj;
    }

    public void BeDestroyed()
    {
        _spriteRenderer
            .DOFade(0, _fadeDuration)
            .OnComplete(() => Destroy(gameObject));
    }
}

using System.Collections;
using UnityEngine;

public class Hider : MonoBehaviour
{
    [SerializeField] SpriteRenderer _hiderSprite;

    public void Hide(float duration)
    {
        StartCoroutine(HideCoroutine());

        IEnumerator HideCoroutine()
        {
            var time = 0f;
            var color = _hiderSprite.color;
            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                var lerp = time / duration;
                color.a = Mathf.Lerp(0, 1, lerp);
                _hiderSprite.color = color;
                yield return null;
            }
        }
    }
}
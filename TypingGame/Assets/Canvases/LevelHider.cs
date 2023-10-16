using System.Collections;
using UnityEngine;

public class LevelHider : MonoBehaviour
{
    [SerializeField] SpriteRenderer _opaqueSprite;

    // TODO maybe do with DoTween and just expose alpha?
    public void Hide(float duration)
    {
        StartCoroutine(HideCoroutine());

        IEnumerator HideCoroutine()
        {
            var time = 0f;
            var color = _opaqueSprite.color;
            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                var lerp = time / duration;
                color.a = Mathf.Lerp(0, 1, lerp);
                _opaqueSprite.color = color;
                yield return null;
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class UIStatusIcon : MonoBehaviour
{
    private Animator _animator;

    private string _animatorState;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public static UIStatusIcon Instantiate(UIStatusIcon prefab, Vector3 position, Transform parent, Sprite prefabSprite)
    {
        prefab.gameObject.SetActive(false);
        var added = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
        var image = added.GetComponent<Image>();
        image.sprite = prefabSprite;
        added.gameObject.SetActive(true);
        return added;
    }

    public bool TryStartBlinking() => TryChangeAnimatorState(AnimatorStates.Blinking);
    public bool TryStopBlinking() => TryChangeAnimatorState(AnimatorStates.NotBlinking);

    private bool TryChangeAnimatorState(string state)
    {
        if (_animatorState == state)
            return false;

        _animatorState = state;
        _animator.Play(state);
        return true;
    }

    private class AnimatorStates
    {
        public const string Blinking = "Blink";
        public const string NotBlinking = "Initial";
    }
}
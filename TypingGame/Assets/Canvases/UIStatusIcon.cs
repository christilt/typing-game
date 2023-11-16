using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// TODO: Centre icon images
// TODO: Make icon background stand out more
[RequireComponent(typeof(Animator))]
public class UIStatusIcon : MonoBehaviour
{
    private const string IconImageGameObjectName = "Image"; // Bad but necessary

    private string _animatorState;
    private Animator _animator;

    public static UIStatusIcon Instantiate(UIStatusIcon prefab, Vector3 position, Transform parent, Sprite prefabSprite)
    {
        prefab.gameObject.SetActive(false);
        var added = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
        var image = added.GetComponentsInChildren<Image>().Where(c => c.gameObject.name == IconImageGameObjectName).FirstOrDefault();
        if (image == null)
        {
            Debug.LogWarning($"Did not find image to attach sprite to, with name {IconImageGameObjectName}!");
        }
        else
        {
            image.sprite = prefabSprite;
        }
        added.gameObject.SetActive(true);
        return added;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
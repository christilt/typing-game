using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI), typeof(Animator))]
public class UINotificationPopUp : MonoBehaviour
{
    [SerializeField] private int _hideAfterSeconds;
    private TextMeshProUGUI _text;
    private Animator _animator;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
    }

    public void ShowText(string text)
    {
        CancelInvoke(nameof(HideText));

        _text.text = TextHelper.WithPixelatedMonospaceText(text, 28);
        ResetAnimator();
        _animator.Play("NotificationAppear", -1, 0);

        Invoke(nameof(HideText), _hideAfterSeconds);
    }

    public void HideText()
    {
        CancelInvoke(nameof(HideText));
        // TODO
        _text.text = "";
        ResetAnimator();
    }

    private void ResetAnimator()
    {
        _animator.Play("Initial", -1, 0);
    }
}
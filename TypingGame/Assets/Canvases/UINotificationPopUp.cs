using TMPro;
using UnityEngine;

public class UINotificationPopUp : MonoBehaviour
{
    [SerializeField] private int _hideAfterSeconds;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Animator _animator;

    public void ShowText(string text)
    {
        CancelInvoke(nameof(HideText));

        _text.text = text;
        ResetAnimator();
        _animator.Play("NotificationAppear", -1, 0);

        Invoke(nameof(HideText), _hideAfterSeconds);
    }

    public void HideText()
    {
        CancelInvoke(nameof(HideText));
        _text.text = "";
        ResetAnimator();
    }

    private void ResetAnimator()
    {
        _animator.Play("Initial", -1, 0);
    }
}
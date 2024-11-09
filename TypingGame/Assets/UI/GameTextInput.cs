using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameTextInput : MonoBehaviour, IDeselectHandler
{
    [SerializeField] private TMP_InputField _inputField;

    public void ForceCaretAtEnd()
    {
        if (!gameObject.activeInHierarchy) return;

        _inputField.caretPosition = _inputField.text.Length;
    }

    public void PlayTypeSound() => SoundManager.Instance.PlayTypeHit();

    public void OnDeselect(BaseEventData eventData)
    {
        SoundManager.Instance?.PlayMenuMove();
    }
}
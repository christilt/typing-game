using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameTextInput : MonoBehaviour, IDeselectHandler, ISelectHandler
{
    [SerializeField] private TMP_InputField _inputField;

    public void ForceCaretAtEnd()
    {
        if (!gameObject.activeInHierarchy) return;

        _inputField.caretPosition = _inputField.text.Length;
    }

    public void PlayTypeSound()
    {
        // Don't play type sound on value changed if nothing was pressed
        if (Input.anyKeyDown)
        {
            SoundManager.Instance.PlayTypeHit();
        }
    }
    public void OnSelect(BaseEventData eventData)
    {
        // Only play if selection was made by navigation
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)
            || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            SoundManager.Instance?.PlayMenuMove();
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SoundManager.Instance.PlayMenuComplete();
    }
}
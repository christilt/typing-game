using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public abstract class TextInterpolator : MonoBehaviour   
{
    protected TextMeshProUGUI _text;

    protected object[] _args;

    protected void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _args = GetTextArgs();
    }

    protected void OnEnable()
    {
        if (_args == null)
            throw new ArgumentNullException(nameof(_args));

        var format = _text.text;
        _text.text = string.Format(format, _args); 
    }

    protected abstract object[] GetTextArgs();
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class DTTextTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Ease _ease;
    // out circ good for UI text entry

    private Vector3 _startPosition;
    private Vector3 _destinationPosition;

    private void Awake()
    {
        _destinationPosition = _text.transform.position;
        _startPosition = _text.transform.position + new Vector3(0, _canvas.pixelRect.yMax, 0);
        Test();
    }

    private void FixedUpdate()
    {
        if (Input.anyKey)
        {
            Test();
        }
    }

    private void Test()
    {
        _text.transform.position = _startPosition;
        _text.transform.DOMove(_destinationPosition, 0.5f)
            .SetEase(_ease);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Boundary : MonoBehaviour
{
    private Collider2D _collider;

    private bool _maybeEntering;
    private bool _maybeExiting;

    public Collider2D Collider => _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetRigidbodyComponent<Player>(out Player player))
        {
            _maybeEntering = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetRigidbodyComponent<Player>(out Player player))
        {
            _maybeExiting = true;
        }
    }

    // Ensure this only runs after OnTrigger events,
    // and after Update events in other objects that may have disabled objects due to game state (e.g. pause)
    private void LateUpdate()
    {
        if (_maybeEntering)
        {
            Player.Instance.EnterBoundary(this);
            _maybeEntering = false;
        }

        if (_maybeExiting)
        {
            Player.Instance.ExitBoundary(this);
            _maybeExiting = false;
        }
    }
}

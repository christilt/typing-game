using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO
        Debug.Log($"Player Hit {collision.gameObject.name}");
    }
}

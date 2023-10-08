using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Collectable : MonoBehaviour
{
    public void DestroySelf()
    {
        GameObject.Destroy(gameObject);
    }
}
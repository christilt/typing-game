using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerAttackController : MonoBehaviour
{
    private PlayerInput _input;

    private void Awake()
    {
        _input = new();
        _input.Main.Trigger.performed += Attack;
    }

    private void OnDestroy()
    {
        _input.Main.Trigger.performed -= Attack;
    }

    public void EnableComponent()
    {
        _input.Main.Enable();
    }

    public void DisableComponent()
    {
        _input.Main.Disable();
    }

    private void Attack(CallbackContext context)
    {
        Debug.Log("Attack"); // TODO: remove
    }
}

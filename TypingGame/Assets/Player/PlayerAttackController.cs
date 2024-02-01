using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private PlayerAttack _playerAttackPrefab;

    private PlayerInput _input;
    private PlayerAttack _currentAttack;

    private void Awake()
    {
        _input = new();
        _input.Main.Trigger.performed += MaybeAttack;
    }

    private void OnDestroy()
    {
        _input.Main.Trigger.performed -= MaybeAttack;
    }

    public void EnableComponent()
    {
        _input.Main.Enable();
    }

    public void DisableComponent()
    {
        _input.Main.Disable();

        if (_currentAttack != null)
        {
            GameObject.Destroy(_currentAttack.gameObject);
        }
    }

    private void MaybeAttack(CallbackContext context)
    {
        if (PlayerAttackManager.Instance.TryPlayerAttack())
        {
            Attack();
        }
    }

    private void Attack()
    {
        _currentAttack = GameObject.Instantiate(_playerAttackPrefab, transform);
    }
}

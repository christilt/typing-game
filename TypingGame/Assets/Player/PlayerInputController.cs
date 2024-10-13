using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private PlayerAttack _playerAttackPrefab;

    private PlayerInput _input;
    private PlayerAttack _currentAttack;

    public event Action OnAttack;
    public event Action OnPauseOrUnpause;

    private void Awake()
    {
        _input = new();
        _input.Main.AttackTrigger.performed += MaybeAttack;
        _input.Main.PauseTrigger.performed += PauseOrUnpause;
    }

    private void OnDestroy()
    {
        _input.Main.AttackTrigger.performed -= MaybeAttack;
        _input.Main.PauseTrigger.performed -= PauseOrUnpause;
    }

    public void EnableAttack()
    {
        _input.Main.AttackTrigger.Enable();
    }

    public void DisableAttack()
    {
        _input.Main.AttackTrigger.Disable();

        if (_currentAttack != null)
        {
            GameObject.Destroy(_currentAttack.gameObject);
        }
    }

    public void EnablePauseAndUnpause() => _input.Main.PauseTrigger.Enable();
    public void DisablePauseAndUnpause() => _input.Main.PauseTrigger.Disable();

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
        OnAttack?.Invoke();
    }

    private void PauseOrUnpause(CallbackContext context)
    {
        if (GameplayManager.Instance.State == GameState.LevelPausing)
            GameplayManager.Instance.LevelUnpausing();
        else
            GameplayManager.Instance.LevelPausing();
        OnPauseOrUnpause?.Invoke();
    }
}

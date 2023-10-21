using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Singleton<Player>
{
    [SerializeField] private PlayerVisual _visual;
    [SerializeField] private PlayerTypingMovement _typingMovement;
    [SerializeField] private Collider2D _collider;

    public void SetAsFollow(CinemachineVirtualCamera camera) => camera.Follow = _visual.transform;

    public void Celebrate() => _visual.PacmanCelebrate();

    private void Start()
    {
        GameManager.Instance.OnStateChanging += HandleGameStateChanging;
        _visual.OnPacmanExploding += HandlePacmanExploding;
        _visual.OnPacmanExploded += HandlePacmanExploded;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.TryGetRigidbodyComponent<Collectable>(out var collectable))
        {
            collectable.DestroySelf();
        }

        if (collision.TryGetRigidbodyComponent<Enemy>(out var enemy))
        {
            _visual.PacmanDie();
            GameManager.Instance.PlayerDying();
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanging -= HandleGameStateChanging;
        if (_visual != null)
            _visual.OnPacmanExploding -= HandlePacmanExploding;
    }

    private void HandleGameStateChanging(GameState state)
    {
        if (state == GameState.LevelPlaying)
        {
            _typingMovement.EnableComponent();
            _collider.enabled = true;
        }
        else
        {
            _typingMovement.DisableComponent();
            _collider.enabled = false;
        }

        if (state == GameState.LevelWinning)
        {
            _visual.PacmanCelebrate();
        }
    }

    private void HandlePacmanExploding()
    {
        GameManager.Instance.PlayerExploding();
    }

    private void HandlePacmanExploded()
    {
        GameManager.Instance.PlayerExploded();
        gameObject.SetActive(false);
    }
}

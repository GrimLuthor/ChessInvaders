using System;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyStats _stats;
    [SerializeField] private SpriteRenderer _spriteRenderer;  // Visual child's SpriteRenderer

    public event Action OnDeath;
    public event Action<float, float> OnHealthChanged;  // current, max

    public float      CurrentHp    { get; private set; }
    public float      MaxHp        => _stats.MaxHp;
    public EnemyStats Stats        => _stats;
    public Vector2Int TilePosition { get; private set; }

    private void Awake()
    {
        CurrentHp = _stats.MaxHp;
    }

    public void TakeDamage(float amount)
    {
        if (CurrentHp <= 0f) return;

        CurrentHp = Mathf.Max(0f, CurrentHp - amount);
        OnHealthChanged?.Invoke(CurrentHp, _stats.MaxHp);

        if (CurrentHp <= 0f)
            Die();
    }

    // Called by WaveManager on spawn and each step tick
    public void SetTilePosition(Vector2Int tile)
    {
        TilePosition = tile;
        transform.position = GameManager.Board.GetTileCenter(tile);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        gameObject.SetActive(false);
    }
}

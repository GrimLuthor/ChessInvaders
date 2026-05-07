using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public static readonly HashSet<EnemyBase> ActiveEnemies = new();

    private void OnEnable()  => ActiveEnemies.Add(this);
    private void OnDisable() => ActiveEnemies.Remove(this);
    [SerializeField] private EnemyStats _stats;
    [SerializeField] private SpriteRenderer _spriteRenderer;  // Visual child's SpriteRenderer

    public event Action OnDeath;
    public event Action<float, float> OnHealthChanged;  // current, max

    public float      CurrentHp    { get; private set; }
    public float      MaxHp        => _stats.MaxHp;
    public EnemyStats Stats        => _stats;
    public Vector2Int TilePosition { get; private set; }

    [SerializeField] private Vector2Int _startingTile = new(4, 6);

    private void Start()
    {
        CurrentHp = _stats.MaxHp;
        SetTilePosition(_startingTile);
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public static readonly HashSet<EnemyBase> ActiveEnemies = new();

    private void OnEnable()  => ActiveEnemies.Add(this);
    private void OnDisable() => ActiveEnemies.Remove(this);

    [SerializeField] private EnemyStats    _stats;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public event Action           OnDeath;
    public event Action<float, float> OnHealthChanged;

    public float      CurrentHp    { get; private set; }
    public float      MaxHp        => _stats.MaxHp;
    public EnemyStats Stats        => _stats;
    public Vector2Int TilePosition { get; private set; }

    // Fallback for scene-placed test enemies. Ignored when WaveManager calls SetTilePosition first.
    [SerializeField] private Vector2Int _startingTile = new(4, 6);
    [SerializeField] private float      _verticalOffset = 0.2f;
    private bool _positionSet;

    private void Awake()
    {
        CurrentHp = _stats.MaxHp;
    }

    private void Start()
    {
        if (!_positionSet)
            SetTilePosition(_startingTile);
    }

    public void ResetHealth()
    {
        CurrentHp = _stats.MaxHp;
        OnHealthChanged?.Invoke(CurrentHp, _stats.MaxHp);
    }

    public void TakeDamage(float amount)
    {
        if (CurrentHp <= 0f) return;

        CurrentHp = Mathf.Max(0f, CurrentHp - amount);
        OnHealthChanged?.Invoke(CurrentHp, _stats.MaxHp);

        if (CurrentHp <= 0f)
            Die();
    }

    public void SetTilePosition(Vector2Int tile)
    {
        _positionSet = true;
        TilePosition = tile;
        transform.position = TileCenter(tile);
    }

    public void SmoothStep(Vector2Int targetTile, float duration)
    {
        TilePosition = targetTile;
        StartCoroutine(SmoothMoveRoutine(TileCenter(targetTile), duration));
    }

    private Vector3 TileCenter(Vector2Int tile) =>
        GameManager.Board.GetTileCenter(tile) + Vector3.up * (_verticalOffset * GameManager.Board.TileSize);

    private IEnumerator SmoothMoveRoutine(Vector3 target, float duration)
    {
        Vector3 start   = transform.position;
        float   elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed           += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        transform.position = target;
    }

    private void Die()
    {
        OnDeath?.Invoke();
        gameObject.SetActive(false);
    }
}

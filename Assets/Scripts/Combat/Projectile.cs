using System;
using UnityEngine;

/// <summary>
/// Generic projectile used by both the player and enemies.
/// Caller provides direction, speed, and a callback to return it to the correct pool —
/// this keeps Projectile decoupled from PoolManager entirely.
/// </summary>
public class Projectile : MonoBehaviour, IPoolable
{
    [SerializeField] private int _damage = 1;

    private Vector2 _direction;
    private float _speed;
    private Action _returnToPool;

    /// <summary>
    /// Called by the pool on retrieval. Activates the object and clears movement state
    /// so a recycled projectile never carries over velocity from a previous use.
    /// </summary>
    public void OnSpawn()
    {
        gameObject.SetActive(true);
        _direction = Vector2.zero;
        _speed = 0f;
        _returnToPool = null;
    }

    public void OnDespawn()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the projectile in motion. Must be called after retrieving from the pool.
    /// speedInTiles is multiplied by TileSize so speed stays proportional if the board scales.
    /// </summary>
    public void Launch(Vector2 direction, float speedInTiles, Action returnToPool)
    {
        _direction = direction.normalized;
        _speed = speedInTiles * GameManager.Board.TileSize;
        _returnToPool = returnToPool;
    }

    private void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);

        if (!GameManager.Board.IsInBounds(GameManager.Board.WorldToGrid(transform.position)))
            Despawn();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // DamageSystem.Apply(other, _damage) will be wired here when enemies exist
        Despawn();
    }

    private void Despawn()
    {
        _returnToPool?.Invoke();
    }
}

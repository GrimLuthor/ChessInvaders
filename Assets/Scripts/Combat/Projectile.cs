using System;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable
{
    [SerializeField] private int       _damage           = 1;
    [SerializeField] private LayerMask _damageLayers     = ~0;
    [SerializeField] private LayerMask _passThroughLayers = 0;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Rigidbody2D _rb;
    private Action      _returnToPool;
    private Vector2     _spawnPosition;
    private float       _maxDistanceSqr  = float.MaxValue;
    private Collider2D  _ignoredCollider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void OnSpawn()
    {
        gameObject.SetActive(true);
        _rb.linearVelocity = Vector2.zero;
        _returnToPool      = null;
        _maxDistanceSqr    = float.MaxValue;
        _ignoredCollider   = null;
        _passThroughLayers = 0;
    }

    public void OnDespawn()
    {
        _rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    public void SetDamage(int damage)               => _damage = damage;
    public void SetPassThrough(LayerMask layers)    => _passThroughLayers = layers;
    public void SetFlipX(bool flip)                 { if (_spriteRenderer != null) _spriteRenderer.flipX = flip; }

    public void Launch(Vector2 direction, float speedInTiles, Action returnToPool,
        float maxDistance = float.MaxValue, Collider2D ignore = null)
    {
        float speed = speedInTiles * GameManager.Board.TileSize;
        _rb.linearVelocity = direction.normalized * speed;
        _returnToPool = returnToPool;
        _spawnPosition   = transform.position;
        _maxDistanceSqr  = maxDistance * maxDistance;
        _ignoredCollider = ignore;
    }

    private void Update()
    {
        if (((Vector2)transform.position - _spawnPosition).sqrMagnitude >= _maxDistanceSqr)
        {
            Despawn();
            return;
        }
        if (!GameManager.Board.IsInBounds(GameManager.Board.WorldToGrid(transform.position)))
            Despawn();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == _ignoredCollider) return;
        if ((_passThroughLayers & (1 << other.gameObject.layer)) != 0) return;
        if ((_damageLayers & (1 << other.gameObject.layer)) != 0)
            other.GetComponent<IDamageable>()?.TakeDamage(_damage);
        Despawn();
    }

    private void Despawn()
    {
        if (_returnToPool == null) return;
        var cb = _returnToPool;
        _returnToPool = null;
        cb.Invoke();
    }
}

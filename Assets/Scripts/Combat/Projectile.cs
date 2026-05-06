using System;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable
{
    [SerializeField] private int _damage = 1;

    private Rigidbody2D _rb;
    private Action _returnToPool;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void OnSpawn()
    {
        gameObject.SetActive(true);
        _rb.linearVelocity = Vector2.zero;
        _returnToPool = null;
    }

    public void OnDespawn()
    {
        _rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    public void Launch(Vector2 direction, float speedInTiles, Action returnToPool)
    {
        float speed = speedInTiles * GameManager.Board.TileSize;
        _rb.linearVelocity = direction.normalized * speed;
        _returnToPool = returnToPool;
    }

    private void Update()
    {
        if (!GameManager.Board.IsInBounds(GameManager.Board.WorldToGrid(transform.position)))
            Despawn();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<IDamageable>()?.TakeDamage(_damage);
        Despawn();
    }

    private void Despawn()
    {
        _returnToPool?.Invoke();
    }
}

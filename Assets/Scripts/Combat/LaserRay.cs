using System;
using System.Collections;
using UnityEngine;

public class LaserRay : MonoBehaviour, IPoolable
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float          _lifetime = 0.5f;
    // Rotate this until the sprite lines up with the SW direction in play mode.
    [SerializeField] private float          _rotationOffset = 0f;
    // Push the laser start point away from the pawn center along the fire direction.
    [SerializeField] private float          _startOffset = 0f;

    private Action    _returnToPool;
    private Coroutine _lifetimeRoutine;

    public void OnSpawn() => gameObject.SetActive(true);

    public void OnDespawn()
    {
        if (_lifetimeRoutine != null) StopCoroutine(_lifetimeRoutine);
        gameObject.SetActive(false);
    }

    public void Fire(Vector2 origin, Vector2 targetCenter, int damage, Action returnToPool)
    {
        _returnToPool = returnToPool;

        Vector2 dir = targetCenter - origin;

        Vector2 start = origin + dir.normalized * _startOffset;
        transform.SetPositionAndRotation(start, Quaternion.Euler(0f, 0f, _rotationOffset));
        _spriteRenderer.flipX  = dir.x > 0f;
        _spriteRenderer.color  = Color.white;

        // Damage player if still on the target tile when the laser fires.
        float hitRadiusSqr = GameManager.Board.TileSize * GameManager.Board.TileSize * 0.5f;
        if (((Vector2)GameManager.Player.position - targetCenter).sqrMagnitude < hitRadiusSqr)
            GameManager.PlayerHealth.TakeDamage(damage);

        _lifetimeRoutine = StartCoroutine(LifetimeRoutine());
    }

    private IEnumerator LifetimeRoutine()
    {
        yield return new WaitForSeconds(_lifetime);
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

using System;
using System.Collections;
using UnityEngine;

public class IceWave : MonoBehaviour, IPoolable
{
    [SerializeField] private Animator      _animator;
    [SerializeField] private BoxCollider2D _collider;
    // Match this to the duration of the Wave animation clip.
    [SerializeField] private float         _lifetime = 1.5f;
    // Must match the state name exactly as shown in the Animator window.
    [SerializeField] private string        _waveStateName = "Wave";

    private static readonly int WaveTrigger = Animator.StringToHash("Wave");

    private Action    _returnToPool;
    private Coroutine _lifetimeRoutine;
    private bool      _hasHit;
    private int       _damage;

    public void OnSpawn()
    {
        gameObject.SetActive(true);
        _hasHit = false;
    }

    public void OnDespawn()
    {
        if (_lifetimeRoutine != null) StopCoroutine(_lifetimeRoutine);
        gameObject.SetActive(false);
    }

    public void Fire(int columnX, int damage, Action returnToPool)
    {
        _damage       = damage;
        _returnToPool = returnToPool;

        var   board    = GameManager.Board;
        float colWorldX = board.GetTileCenter(new Vector2Int(columnX, 0)).x;
        float centerY   = board.transform.position.y;
        transform.position = new Vector2(colWorldX, centerY);

        // Size the collider to cover the full column each run (supports tileSize changes).
        _collider.size = new Vector2(board.TileSize, board.BoardSize * board.TileSize);

        _animator.SetTrigger(WaveTrigger);

        // Damage player immediately if already standing in this column.
        if (GameManager.Board.WorldToGrid(GameManager.Player.position).x == columnX)
            HitPlayer();

        _lifetimeRoutine = StartCoroutine(LifetimeRoutine());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return;
        if (other.gameObject.layer == Layers.Player)
            HitPlayer();
    }

    private void HitPlayer()
    {
        _hasHit = true;
        GameManager.PlayerHealth.TakeDamage(_damage);
        // Jump to the last frame (explosion) immediately.
        _animator.Play(_waveStateName, 0, 0.9f);
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

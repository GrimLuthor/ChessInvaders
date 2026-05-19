using System;
using System.Collections;
using UnityEngine;

public class MeleeSwing : MonoBehaviour, IPoolable
{
    [SerializeField] private Animator _animator;
    // Total duration of the swing animation clip.
    [SerializeField] private float    _lifetime    = 0.5f;
    // When during the animation the hit is checked — tune to the impact frame.
    [SerializeField] private float    _damageDelay = 0.2f;
    // Must match the state name exactly in the Animator window.
    [SerializeField] private string   _swingStateName = "Swing";

    private Action          _returnToPool;
    private Coroutine       _swingRoutine;
    private int             _damage;
    private Vector2Int      _attackerTile;
    private SpriteRenderer  _kingSprite;

    public void OnSpawn() => gameObject.SetActive(true);

    public void OnDespawn()
    {
        if (_swingRoutine != null) StopCoroutine(_swingRoutine);
        gameObject.SetActive(false);
    }

    public void Fire(Vector3 position, Vector2Int attackerTile, int damage,
                     SpriteRenderer kingSprite, Action returnToPool)
    {
        _damage       = damage;
        _attackerTile = attackerTile;
        _returnToPool = returnToPool;
        _kingSprite   = kingSprite;

        transform.position   = position;
        transform.localScale = kingSprite.transform.lossyScale;

        kingSprite.enabled = false;

        _animator.Play(_swingStateName, 0, 0f);
        _swingRoutine = StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        yield return new WaitForSeconds(_damageDelay);

        Vector2Int playerTile = GameManager.Board.WorldToGrid(GameManager.Player.position);
        int dx = Mathf.Abs(playerTile.x - _attackerTile.x);
        int dy = Mathf.Abs(playerTile.y - _attackerTile.y);
        bool isAdjacent = dx <= 1 && dy <= 1 && !(dx == 0 && dy == 0);

        if (isAdjacent)
            GameManager.PlayerHealth.TakeDamage(_damage);

        yield return new WaitForSeconds(_lifetime - _damageDelay);
        Despawn();
    }

    private void Despawn()
    {
        if (_kingSprite != null) _kingSprite.enabled = true;
        if (_returnToPool == null) return;
        var cb = _returnToPool;
        _returnToPool = null;
        cb.Invoke();
    }
}

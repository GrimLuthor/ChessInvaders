using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public class ShootingAI : MonoBehaviour
{
    [SerializeField] private PieceType  _pieceType;
    [SerializeField] private Vector2Int _forwardDir = new(0, -1); // enemies advance toward y=0

    private EnemyBase        _enemy;
    private Collider2D       _collider;
    private Coroutine        _shootRoutine;
    private IntentIndicator  _activeIndicator;

    private void Awake()
    {
        _enemy    = GetComponent<EnemyBase>();
        _collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        _shootRoutine = StartCoroutine(ShootLoop());
    }

    private void OnDisable()
    {
        if (_shootRoutine != null) StopCoroutine(_shootRoutine);
        ReturnActiveIndicator();
    }

    private IEnumerator ShootLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_enemy.Stats.FireInterval);

            List<Vector2Int> threatened = AttackPatterns.GetThreatenedTiles(
                _pieceType, _enemy.TilePosition, GameManager.Board.BoardSize, _forwardDir,
                IsTileBlockedByEnemy);

            threatened.RemoveAll(IsTileBlockedByEnemy);

            if (threatened.Count == 0) continue;

            float prob = CalcShootProbability(threatened);
            if (Random.value >= prob) continue;

            Vector2Int targetTile = PickTarget(threatened);
            yield return TelegraphAndShoot(targetTile);
        }
    }

    private float CalcShootProbability(List<Vector2Int> threatened)
    {
        Vector2 playerPos = GameManager.Player.position;
        float alertSqr = _enemy.Stats.AlertThresholdInTiles * _enemy.Stats.AlertThresholdInTiles
                         * GameManager.Board.TileSize * GameManager.Board.TileSize;

        float minSqrDist = float.MaxValue;
        foreach (var tile in threatened)
        {
            float sqr = (playerPos - (Vector2)GameManager.Board.GetTileCenter(tile)).sqrMagnitude;
            if (sqr < minSqrDist) minSqrDist = sqr;
        }

        float t = 1f - Mathf.Clamp01(minSqrDist / alertSqr);
        return Mathf.Lerp(_enemy.Stats.IdleShotProbability, _enemy.Stats.AlertShotProbability, t);
    }

    private Vector2Int PickTarget(List<Vector2Int> threatened)
    {
        Vector2Int playerTile = GameManager.Board.WorldToGrid(GameManager.Player.position);
        Vector2Int best = threatened[0];
        float bestSqr = float.MaxValue;
        foreach (var tile in threatened)
        {
            float sqr = (tile - playerTile).sqrMagnitude;
            if (sqr < bestSqr) { bestSqr = sqr; best = tile; }
        }
        return best;
    }

    private IEnumerator TelegraphAndShoot(Vector2Int targetTile)
    {
        _activeIndicator = GameManager.Pool.GetIntentIndicator();
        _activeIndicator.transform.position = GameManager.Board.GetTileCenter(targetTile);

        yield return new WaitForSeconds(_enemy.Stats.TelegraphDuration);

        ReturnActiveIndicator();

        Vector2 targetWorld = GameManager.Board.GetTileCenter(targetTile);
        Vector2 dir = targetWorld - (Vector2)transform.position;
        if (dir.sqrMagnitude < 0.001f) yield break;

        Projectile p = GameManager.Pool.GetEnemyProjectile();
        p.SetDamage(_enemy.Stats.ProjectileDamage);
        p.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        float maxDist = AttackPatterns.IsSliding(_pieceType)
            ? float.MaxValue
            : dir.magnitude + GameManager.Board.TileSize * 0.5f;

        p.Launch(dir.normalized, _enemy.Stats.ProjectileSpeedInTiles,
            () => GameManager.Pool.ReturnEnemyProjectile(p), maxDist, _collider);
    }

    private bool IsTileBlockedByEnemy(Vector2Int tile)
    {
        foreach (var enemy in EnemyBase.ActiveEnemies)
            if (enemy != _enemy && enemy.TilePosition == tile)
                return true;
        return false;
    }

    private void ReturnActiveIndicator()
    {
        if (_activeIndicator == null) return;
        GameManager.Pool.ReturnIntentIndicator(_activeIndicator);
        _activeIndicator = null;
    }
}

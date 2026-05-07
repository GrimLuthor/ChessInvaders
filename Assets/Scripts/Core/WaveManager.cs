using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [SerializeField] private List<WaveData> _waves;
    [SerializeField] private float          _stepInterval   = 60f;
    [SerializeField] private float          _restDuration   = 10f;
    [SerializeField] private Transform      _enemyContainer;

    [SerializeField] private float _rankBreachDamage = 20f;

    private int             _currentWaveIndex = -1;
    private int             _stepCount;
    private int             _spawnedEnemies;
    private int             _deadEnemies;
    private List<EnemyBase> _waveEnemies = new();
    private Coroutine       _stepRoutine;
    private float           _phaseStartTime;
    private bool            _isResting;

    public float NormalizedTimeRemaining => _isResting
        ? 1f - Mathf.Clamp01((Time.time - _phaseStartTime) / _restDuration)
        : 1f - Mathf.Clamp01((Time.time - _phaseStartTime) / _stepInterval);

    public float SecondsRemaining => _isResting
        ? Mathf.Max(0f, _restDuration - (Time.time - _phaseStartTime))
        : Mathf.Max(0f, _stepInterval - (Time.time - _phaseStartTime));

    public bool IsResting         => _isResting;
    public int  CurrentWaveNumber => _currentWaveIndex + 1;
    public int  WaveCount         => _waves.Count;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        StartNextWave();
    }

    // ── Wave lifecycle ───────────────────────────────────────────────────────

    private void StartNextWave()
    {
        _currentWaveIndex++;
        if (_currentWaveIndex >= _waves.Count) return;

        _stepCount      = 0;
        _deadEnemies    = 0;
        _spawnedEnemies = 0;
        _waveEnemies.Clear();

        WaveData data = _waves[_currentWaveIndex];

        foreach (var spawn in data.InitialSpawns)
            SpawnEnemy(spawn.Prefab, spawn.Tile);

        _stepRoutine = StartCoroutine(StepLoop());
    }

    private IEnumerator StepLoop()
    {
        while (true)
        {
            _phaseStartTime = Time.time;
            yield return new WaitForSeconds(_stepInterval);
            StepAllEnemies();
            SpawnReinforcements();
            _stepCount++;
        }
    }

    private IEnumerator RestPeriod()
    {
        if (_stepRoutine != null) StopCoroutine(_stepRoutine);
        _isResting      = true;
        _phaseStartTime = Time.time;
        yield return new WaitForSeconds(_restDuration);
        _isResting = false;
        StartNextWave();
    }

    // ── Enemy step ───────────────────────────────────────────────────────────

    private void StepAllEnemies()
    {
        foreach (var enemy in _waveEnemies)
        {
            if (!enemy.gameObject.activeSelf) continue;
            var next = new Vector2Int(enemy.TilePosition.x, enemy.TilePosition.y - 1);
            if (GameManager.Board.IsInBounds(next))
            {
                enemy.SetTilePosition(next);
            }
            else
            {
                GameManager.PlayerHealth.TakeDamage(_rankBreachDamage);
                enemy.ResetHealth();

                var ai = enemy.GetComponent<ShootingAI>();
                if (ai != null && ai.PieceType == PieceType.Pawn)
                    ai.Promote(PieceType.Queen);

                enemy.SetTilePosition(new Vector2Int(enemy.TilePosition.x, GameManager.Board.BoardSize - 1));
            }
        }
    }

    // ── Spawning ─────────────────────────────────────────────────────────────

    private void SpawnReinforcements()
    {
        WaveData data = _waves[_currentWaveIndex];
        if (_stepCount >= data.Reinforcements.Count) return;

        int backRank = GameManager.Board.BoardSize - 1;
        foreach (var rs in data.Reinforcements[_stepCount].Spawns)
            SpawnEnemy(rs.Prefab, new Vector2Int(rs.Column, backRank));
    }

    private void SpawnEnemy(GameObject prefab, Vector2Int tile)
    {
        var go    = Instantiate(prefab, _enemyContainer);
        var enemy = go.GetComponent<EnemyBase>();
        enemy.SetTilePosition(tile);
        enemy.OnDeath += () => OnEnemyDied(enemy);
        _waveEnemies.Add(enemy);
        _spawnedEnemies++;
    }

    // ── Death & win/loss ─────────────────────────────────────────────────────

    private void OnEnemyDied(EnemyBase enemy)
    {
        _deadEnemies++;

        // Killing the enemy King always ends the final wave immediately
        if (_currentWaveIndex == _waves.Count - 1)
        {
            var ai = enemy.GetComponent<ShootingAI>();
            if (ai != null && ai.PieceType == PieceType.King)
            {
                GameManager.Instance.TriggerWin();
                return;
            }
        }

        if (_deadEnemies >= _spawnedEnemies)
        {
            if (_currentWaveIndex >= _waves.Count - 1)
                GameManager.Instance.TriggerWin();
            else
                StartCoroutine(RestPeriod());
        }
    }
}

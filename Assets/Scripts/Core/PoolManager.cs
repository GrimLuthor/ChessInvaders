using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Owns object pools for all high-frequency spawn/despawn types.
/// Add a new pool here when a new pooled prefab type is introduced (enemy projectiles, intent indicators).
/// Pool sizes are serialized so they can be tuned without recompilation.
/// </summary>
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [Header("Player Projectile")]
    [SerializeField] private GameObject _playerProjectilePrefab;
    [SerializeField] private int _playerProjectilePoolSize = 20;

    [Header("Enemy Projectile")]
    [SerializeField] private GameObject _enemyProjectilePrefab;
    [SerializeField] private int _enemyProjectilePoolSize = 30;

    [Header("Intent Indicator")]
    [SerializeField] private GameObject _intentIndicatorPrefab;
    [SerializeField] private int _intentIndicatorPoolSize = 20;

    private ObjectPool<Projectile>       _playerProjectilePool;
    private ObjectPool<Projectile>       _enemyProjectilePool;
    private ObjectPool<IntentIndicator>  _intentIndicatorPool;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        _playerProjectilePool = new ObjectPool<Projectile>(
            createFunc:      () => Instantiate(_playerProjectilePrefab).GetComponent<Projectile>(),
            actionOnGet:     p  => p.OnSpawn(),
            actionOnRelease: p  => p.OnDespawn(),
            actionOnDestroy: p  => Destroy(p.gameObject),
            defaultCapacity: _playerProjectilePoolSize
        );

        _enemyProjectilePool = new ObjectPool<Projectile>(
            createFunc:      () => Instantiate(_enemyProjectilePrefab).GetComponent<Projectile>(),
            actionOnGet:     p  => p.OnSpawn(),
            actionOnRelease: p  => p.OnDespawn(),
            actionOnDestroy: p  => Destroy(p.gameObject),
            defaultCapacity: _enemyProjectilePoolSize
        );

        _intentIndicatorPool = new ObjectPool<IntentIndicator>(
            createFunc:      () => Instantiate(_intentIndicatorPrefab).GetComponent<IntentIndicator>(),
            actionOnGet:     i  => i.OnSpawn(),
            actionOnRelease: i  => i.OnDespawn(),
            actionOnDestroy: i  => Destroy(i.gameObject),
            defaultCapacity: _intentIndicatorPoolSize
        );
    }

    public Projectile      GetPlayerProjectile()                      => _playerProjectilePool.Get();
    public void            ReturnPlayerProjectile(Projectile p)       => _playerProjectilePool.Release(p);

    public Projectile      GetEnemyProjectile()                       => _enemyProjectilePool.Get();
    public void            ReturnEnemyProjectile(Projectile p)        => _enemyProjectilePool.Release(p);

    public IntentIndicator GetIntentIndicator()                       => _intentIndicatorPool.Get();
    public void            ReturnIntentIndicator(IntentIndicator i)   => _intentIndicatorPool.Release(i);
}

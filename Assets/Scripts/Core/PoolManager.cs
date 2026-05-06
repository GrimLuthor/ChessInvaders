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

    private ObjectPool<Projectile> _playerProjectilePool;

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
    }

    public Projectile GetPlayerProjectile()          => _playerProjectilePool.Get();
    public void       ReturnPlayerProjectile(Projectile p) => _playerProjectilePool.Release(p);
}

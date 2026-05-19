using System;
using UnityEngine;
using UnityEngine.Pool;

// Add to any enemy root that needs its own projectile type.
// ShootingAI checks for this component and uses it instead of the shared pool.
public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int        _poolSize = 10;
    [SerializeField] private bool       _rotateToDirection  = true;
    // Sprite is right-facing — flip horizontally when firing leftward.
    [SerializeField] private bool       _flipXToDirection   = false;

    public bool RotateToDirection  => _rotateToDirection;
    public bool FlipXToDirection   => _flipXToDirection;

    private ObjectPool<Projectile> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Projectile>(
            createFunc:      () => Instantiate(_prefab).GetComponent<Projectile>(),
            actionOnGet:     p  => p.OnSpawn(),
            actionOnRelease: p  => p.OnDespawn(),
            actionOnDestroy: p  => Destroy(p.gameObject),
            defaultCapacity: _poolSize
        );
    }

    public Projectile Get()               => _pool.Get();
    public void       Return(Projectile p) => _pool.Release(p);
}

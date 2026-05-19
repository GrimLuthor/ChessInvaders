using UnityEngine;
using UnityEngine.Pool;

// Add to any enemy root that fires a laser instead of a projectile.
// ShootingAI checks for this component and uses it in place of the projectile path.
public class LaserPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int        _poolSize = 5;

    private ObjectPool<LaserRay> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<LaserRay>(
            createFunc:      () => Instantiate(_prefab).GetComponent<LaserRay>(),
            actionOnGet:     r  => r.OnSpawn(),
            actionOnRelease: r  => r.OnDespawn(),
            actionOnDestroy: r  => Destroy(r.gameObject),
            defaultCapacity: _poolSize
        );
    }

    public LaserRay Get()              => _pool.Get();
    public void     Return(LaserRay r) => _pool.Release(r);
}

using UnityEngine;
using UnityEngine.Pool;

// Add to any enemy root that fires an IceWave instead of a projectile.
public class IceWavePool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int        _poolSize = 3;

    private ObjectPool<IceWave> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<IceWave>(
            createFunc:      () => Instantiate(_prefab).GetComponent<IceWave>(),
            actionOnGet:     w  => w.OnSpawn(),
            actionOnRelease: w  => w.OnDespawn(),
            actionOnDestroy: w  => Destroy(w.gameObject),
            defaultCapacity: _poolSize
        );
    }

    public IceWave Get()             => _pool.Get();
    public void    Return(IceWave w) => _pool.Release(w);
}

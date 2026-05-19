using UnityEngine;
using UnityEngine.Pool;

public class MeleeSwingPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int        _poolSize = 3;

    private ObjectPool<MeleeSwing> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<MeleeSwing>(
            createFunc:      () => Instantiate(_prefab).GetComponent<MeleeSwing>(),
            actionOnGet:     s  => s.OnSpawn(),
            actionOnRelease: s  => s.OnDespawn(),
            actionOnDestroy: s  => Destroy(s.gameObject),
            defaultCapacity: _poolSize
        );
    }

    public MeleeSwing Get()              => _pool.Get();
    public void       Return(MeleeSwing s) => _pool.Release(s);
}

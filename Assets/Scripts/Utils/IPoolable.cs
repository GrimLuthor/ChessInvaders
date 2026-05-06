/// <summary>
/// Implemented by any object managed by a pool.
/// OnSpawn is called when retrieved, OnDespawn when returned.
/// </summary>
public interface IPoolable
{
    void OnSpawn();
    void OnDespawn();
}

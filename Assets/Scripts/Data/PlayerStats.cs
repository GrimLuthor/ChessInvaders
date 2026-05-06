using UnityEngine;

[CreateAssetMenu(menuName = "Chess Invaders/Player Stats", fileName = "PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [SerializeField] private float _maxHp            = 100f;
    [SerializeField] private float _moveSpeedInTiles  = 5f;
    [SerializeField] private float _fireRatePerSecond = 5f;
    [SerializeField] private float _bulletSpeedInTiles = 10f;
    [SerializeField] private float _iFrameDuration    = 0.3f;
    [SerializeField] private float _burstCooldown     = 8f;   // UI only — ability deferred

    public float MaxHp             => _maxHp;
    public float MoveSpeedInTiles  => _moveSpeedInTiles;
    public float FireRatePerSecond => _fireRatePerSecond;
    public float BulletSpeedInTiles => _bulletSpeedInTiles;
    public float IFrameDuration    => _iFrameDuration;
    public float BurstCooldown     => _burstCooldown;
}

using UnityEngine;

[CreateAssetMenu(menuName = "Chess Invaders/Enemy Stats", fileName = "EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("Health")]
    [SerializeField] private float _maxHp = 20f;

    [Header("Movement")]
    [SerializeField] private float _moveSpeedInTiles = 1f;

    [Header("Shooting")]
    [SerializeField] private float _fireInterval           = 3f;
    [SerializeField] private float _projectileSpeedInTiles = 4f;
    [SerializeField] private float _telegraphDuration      = 1.2f;
    [SerializeField] private int   _projectileDamage       = 20;

    [Header("Alert")]
    [SerializeField] private float _alertThresholdInTiles = 3f;
    [SerializeField] private float _idleShotProbability   = 0.15f;
    [SerializeField] private float _alertShotProbability  = 0.75f;

    public float MaxHp                  => _maxHp;
    public float MoveSpeedInTiles       => _moveSpeedInTiles;
    public float FireInterval           => _fireInterval;
    public float ProjectileSpeedInTiles => _projectileSpeedInTiles;
    public float TelegraphDuration      => _telegraphDuration;
    public int   ProjectileDamage       => _projectileDamage;
    public float AlertThresholdInTiles  => _alertThresholdInTiles;
    public float IdleShotProbability    => _idleShotProbability;
    public float AlertShotProbability   => _alertShotProbability;
}

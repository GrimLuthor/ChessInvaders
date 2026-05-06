using UnityEngine;

public static class Layers
{
    public static readonly int Player           = LayerMask.NameToLayer("Player");
    public static readonly int Enemy            = LayerMask.NameToLayer("Enemy");
    public static readonly int PlayerProjectile = LayerMask.NameToLayer("PlayerProjectile");
    public static readonly int EnemyProjectile  = LayerMask.NameToLayer("EnemyProjectile");
}

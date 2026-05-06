using UnityEngine;

// Marks this prefab as a Pawn. Pawn-specific behaviour (shooting pattern) is
// handled by ShootingAI in Phase 3 using PieceType.Pawn.
public class EnemyPawn : MonoBehaviour
{
    public PieceType PieceType => PieceType.Pawn;
}

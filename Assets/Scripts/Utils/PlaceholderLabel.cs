using UnityEngine;

// Remove this component when final art is delivered.
public class PlaceholderLabel : MonoBehaviour
{
    [SerializeField] private PieceType      _pieceType;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Awake() => Apply(_pieceType);

    public void Refresh(PieceType newType)
    {
        _pieceType = newType;
        Apply(newType);
    }

    private void Apply(PieceType type)
    {
        _spriteRenderer.color = type switch
        {
            PieceType.Pawn   => new Color(0.6f, 0.6f, 0.6f),
            PieceType.Rook   => new Color(0.2f, 0.6f, 1.0f),
            PieceType.Bishop => new Color(0.2f, 0.9f, 0.4f),
            PieceType.Knight => new Color(1.0f, 0.8f, 0.2f),
            PieceType.Queen  => new Color(0.9f, 0.2f, 0.9f),
            PieceType.King   => new Color(1.0f, 0.3f, 0.3f),
            _                => Color.white
        };
    }
}

using UnityEngine;

// Remove this component when final art is delivered.
public class PlaceholderLabel : MonoBehaviour
{
    [SerializeField] private PieceType      _pieceType;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    // Assign in prefab when art is ready. Leave null to keep placeholder color.
    [SerializeField] private Sprite _idleSprite;
    // Pawn only: assign the Queen sprite here so promotion swaps correctly.
    [SerializeField] private Sprite   _promotedSprite;
    // Pawn only: local position offset applied to the SpriteRenderer on promotion (e.g. 0, 0.2, 0).
    [SerializeField] private Vector3  _promotedOffset;

    private void Awake()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Apply(_pieceType, _idleSprite);
    }

    public void Refresh(PieceType newType)
    {
        _pieceType = newType;
        Apply(newType, _promotedSprite != null ? _promotedSprite : null);
        _spriteRenderer.transform.localPosition = _promotedOffset;
    }

    private void Apply(PieceType type, Sprite sprite)
    {
        if (sprite != null)
        {
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.color  = Color.white;
            return;
        }

        // SpriteRenderer already has a sprite assigned in the prefab — don't tint it.
        if (_spriteRenderer.sprite != null)
        {
            _spriteRenderer.color = Color.white;
            return;
        }

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

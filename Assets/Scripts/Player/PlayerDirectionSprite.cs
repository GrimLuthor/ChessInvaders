using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDirectionSprite : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite _north;
    [SerializeField] private Sprite _south;
    [SerializeField] private Sprite _west;      // flipped for East
    [SerializeField] private Sprite _northWest; // flipped for NorthEast
    [SerializeField] private Sprite _southWest; // flipped for SouthEast

    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mouseWorld = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = mouseWorld - (Vector2)transform.position;

        if (dir.sqrMagnitude < 0.001f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // Remap to 0–360 clockwise from north
        float a = (90f - angle + 360f) % 360f;

        bool flip;
        Sprite next;

        // Divide 360° into 8 sectors of 45° each, centred on each direction
        if      (a <  22.5f || a >= 337.5f) { next = _north;     flip = false; } // N
        else if (a <  67.5f)                { next = _northWest; flip = true;  } // NE
        else if (a < 112.5f)                { next = _west;      flip = true;  } // E
        else if (a < 157.5f)                { next = _southWest; flip = true;  } // SE
        else if (a < 202.5f)                { next = _south;     flip = false; } // S
        else if (a < 247.5f)                { next = _southWest; flip = false; } // SW
        else if (a < 292.5f)                { next = _west;      flip = false; } // W
        else                                { next = _northWest; flip = false; } // NW

        _spriteRenderer.sprite = next;
        _spriteRenderer.flipX  = flip;
    }
}

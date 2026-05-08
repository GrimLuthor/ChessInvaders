using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;

    [SerializeField] private float _knockbackDecay = 20f;

    private Rigidbody2D _rb;
    private float   _speed;
    private Vector2 _minBounds;
    private Vector2 _maxBounds;
    private Vector2 _knockback;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        float tileSize  = GameManager.Board.TileSize;
        int   boardSize = GameManager.Board.BoardSize;

        _speed = _stats.MoveSpeedInTiles * tileSize;

        float halfExtent    = boardSize * tileSize * 0.5f;
        Vector2 boardCenter = GameManager.Board.transform.position;
        _minBounds = boardCenter - Vector2.one * halfExtent;
        _maxBounds = boardCenter + Vector2.one * halfExtent;
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        _knockback = direction * force;
    }

    private void FixedUpdate()
    {
        Vector2 input  = ReadInput();
        Vector2 move   = (input != Vector2.zero ? input.normalized * _speed : Vector2.zero) + _knockback;

        _knockback = Vector2.MoveTowards(_knockback, Vector2.zero, _knockbackDecay * Time.fixedDeltaTime);

        Vector2 newPos = _rb.position + move * Time.fixedDeltaTime;
        newPos.x = Mathf.Clamp(newPos.x, _minBounds.x, _maxBounds.x);
        newPos.y = Mathf.Clamp(newPos.y, _minBounds.y, _maxBounds.y);
        _rb.MovePosition(newPos);
    }

    private Vector2 ReadInput()
    {
        var kb = Keyboard.current;
        if (kb == null) return Vector2.zero;

        float x = 0f;
        float y = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  x -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  y -= 1f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    y += 1f;

        return new Vector2(x, y);
    }
}

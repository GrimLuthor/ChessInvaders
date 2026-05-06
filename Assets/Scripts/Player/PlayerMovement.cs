using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Moves the player with WASD, clamped to the board boundary.
/// Speed is defined in tiles/sec so it stays proportional when tileSize changes.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speedInTiles = 5f;

    private float _speed;
    private Vector2 _minBounds;
    private Vector2 _maxBounds;

    // Start (not Awake) so BoardManager's Awake has already run
    private void Start()
    {
        float tileSize = GameManager.Board.TileSize;
        int boardSize = GameManager.Board.BoardSize;

        _speed = _speedInTiles * tileSize;

        // Outer edge of the corner tiles — player is clamped here, not to tile centers
        float halfExtent = boardSize * tileSize * 0.5f;
        Vector2 boardCenter = GameManager.Board.transform.position;
        _minBounds = boardCenter - Vector2.one * halfExtent;
        _maxBounds = boardCenter + Vector2.one * halfExtent;
    }

    private void Update()
    {
        Vector2 input = ReadInput();
        if (input == Vector2.zero) return;

        // Normalize so diagonal movement isn't faster than cardinal
        Vector2 newPos = (Vector2)transform.position + input.normalized * _speed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, _minBounds.x, _maxBounds.x);
        newPos.y = Mathf.Clamp(newPos.y, _minBounds.y, _maxBounds.y);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
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

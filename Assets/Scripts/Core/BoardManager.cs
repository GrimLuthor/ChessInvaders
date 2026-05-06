using UnityEngine;

/// <summary>
/// Owns the 8x8 grid. Responsibilities:
///   1. Generate tile GameObjects at startup.
///   2. Be the single source of truth for grid <-> world-space conversion.
///
/// Every system that needs to know "where is tile (3,5) in the world?" goes
/// through the helpers here. No other script should do its own grid math.
/// Changing _tileSize alone rescales the entire board and all dependent systems.
/// </summary>
public class BoardManager : MonoBehaviour
{
    [SerializeField] private int _boardSize = 8;

    // All grid math derives from this single value.
    // 1f = 1 Unity unit per tile during gray box. Designers change this, nothing else.
    [SerializeField] private float _tileSize = 1f;

    [SerializeField] private Transform _tileRoot;
    [SerializeField] private GameObject _lightTilePrefab;
    [SerializeField] private GameObject _darkTilePrefab;

    public float TileSize => _tileSize;
    public int BoardSize => _boardSize;

    private void Awake()
    {
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        for (int x = 0; x < _boardSize; x++)
        {
            for (int y = 0; y < _boardSize; y++)
            {
                bool isLight = (x + y) % 2 == 0;
                GameObject prefab = isLight ? _lightTilePrefab : _darkTilePrefab;
                Vector3 worldPos = GridToWorld(new Vector2Int(x, y));
                GameObject tile = Instantiate(prefab, worldPos, Quaternion.identity, _tileRoot);
                tile.name = $"Tile_{x}_{y}";

                // Scale drives tile visual size — must match _tileSize so tiles
                // fill the grid without gaps when tileSize changes.
                tile.transform.localScale = Vector3.one * _tileSize;
            }
        }
    }

    /// <summary>
    /// Returns the world-space center of a tile.
    /// The board is centered on this transform's position.
    /// </summary>
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        // Offset centers the board on this transform rather than having (0,0) at the corner
        float halfExtent = (_boardSize - 1) * _tileSize * 0.5f;
        return transform.position + new Vector3(
            gridPos.x * _tileSize - halfExtent,
            gridPos.y * _tileSize - halfExtent,
            0f
        );
    }

    /// <summary>
    /// Returns the tile coordinate nearest to a world position.
    /// Does NOT clamp to board bounds — call IsInBounds() to validate the result.
    /// </summary>
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        float halfExtent = (_boardSize - 1) * _tileSize * 0.5f;
        Vector3 local = worldPos - transform.position;
        int x = Mathf.RoundToInt((local.x + halfExtent) / _tileSize);
        int y = Mathf.RoundToInt((local.y + halfExtent) / _tileSize);
        return new Vector2Int(x, y);
    }

    public bool IsInBounds(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < _boardSize &&
               gridPos.y >= 0 && gridPos.y < _boardSize;
    }

    // Alias used by gameplay systems that want explicit intent over raw GridToWorld
    public Vector3 GetTileCenter(Vector2Int gridPos) => GridToWorld(gridPos);

#if UNITY_EDITOR
    // Draws the grid outline in the Scene view so you can verify alignment without entering Play mode
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int x = 0; x < _boardSize; x++)
        {
            for (int y = 0; y < _boardSize; y++)
            {
                Gizmos.DrawWireCube(GridToWorld(new Vector2Int(x, y)), Vector3.one * _tileSize);
            }
        }
    }
#endif
}

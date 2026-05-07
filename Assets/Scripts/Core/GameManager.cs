using UnityEngine;

/// <summary>
/// Central access point for top-level systems. Other scripts reach BoardManager,
/// WaveManager, and PlayerHealth through here rather than using FindObjectOfType.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private BoardManager _boardManager;
    [SerializeField] private PoolManager  _poolManager;
    [SerializeField] private GamePalette  _gamePalette;
    [SerializeField] private Transform    _playerTransform;

    public static BoardManager Board   => Instance._boardManager;
    public static PoolManager  Pool    => Instance._poolManager;
    public static GamePalette  Palette => Instance._gamePalette;
    public static Transform    Player  => Instance._playerTransform;

    private void Awake()
    {
        // Destroy duplicate if scene is reloaded, keeping the first instance alive
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void TriggerGameOver()
    {                                                                                                                                                                                 
        // Phase 4: load game over screen here                                                                                                                                      
        Debug.Log("Game Over");
    }
}

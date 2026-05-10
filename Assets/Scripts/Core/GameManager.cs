using UnityEngine;

/// <summary>
/// Central access point for top-level systems. Other scripts reach BoardManager,
/// WaveManager, and PlayerHealth through here rather than using FindObjectOfType.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private BoardManager  _boardManager;
    [SerializeField] private PoolManager   _poolManager;
    [SerializeField] private GamePalette   _gamePalette;
    [SerializeField] private Transform     _playerTransform;
    [SerializeField] private WaveManager   _waveManager;
    [SerializeField] private PlayerHealth  _playerHealth;
    [SerializeField] private EndScreen     _gameOverScreen;
    [SerializeField] private EndScreen     _winScreen;
    [SerializeField] private PauseMenu     _pauseMenu;

    public static BoardManager  Board        => Instance._boardManager;
    public static PoolManager   Pool         => Instance._poolManager;
    public static GamePalette   Palette      => Instance._gamePalette;
    public static Transform     Player       => Instance._playerTransform;
    public static WaveManager   Waves        => Instance._waveManager;
    public static PlayerHealth  PlayerHealth => Instance._playerHealth;

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
        _pauseMenu.Disable();
        _gameOverScreen.Show();
    }

    public void TriggerWin()
    {
        _pauseMenu.Disable();
        _winScreen.Show();
    }
}

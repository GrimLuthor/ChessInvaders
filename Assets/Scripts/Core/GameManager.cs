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

    public static BoardManager Board => Instance._boardManager;
    public static PoolManager  Pool  => Instance._poolManager;

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

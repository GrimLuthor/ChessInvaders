using UnityEngine;

/// <summary>
/// Central access point for top-level systems. Other scripts reach BoardManager,
/// WaveManager, and PlayerHealth through here rather than using FindObjectOfType.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private BoardManager _boardManager;

    // Shorthand accessors so callsites read GameManager.Board.GridToWorld(...)
    // rather than GameManager.Instance._boardManager.GridToWorld(...)
    public static BoardManager Board => Instance._boardManager;

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
}

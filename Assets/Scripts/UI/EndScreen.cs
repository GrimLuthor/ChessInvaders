using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private Button _replayButton;
    [SerializeField] private Button _exitToMenuButton;
    [SerializeField] private string _menuSceneName = "MainMenu";

    private void Awake()
    {
        _replayButton.onClick.AddListener(Replay);
        _exitToMenuButton.onClick.AddListener(ExitToMenu);
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Replay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_menuSceneName);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _exitToMenuButton;
    [SerializeField] private string _menuSceneName = "MainMenu";

    private bool _isPaused;
    private bool _disabled;

    private void Awake()
    {
        _continueButton.onClick.AddListener(Continue);
        _exitToMenuButton.onClick.AddListener(ExitToMenu);
        _panel.SetActive(false);
    }

    private void Update()
    {
        if (_disabled) return;
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_isPaused) Continue();
            else Pause();
        }
    }

    public void Disable()
    {
        _disabled = true;
        if (_isPaused) Continue();
    }

    private void Pause()
    {
        _isPaused = true;
        _panel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Continue()
    {
        _isPaused = false;
        _panel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_menuSceneName);
    }
}

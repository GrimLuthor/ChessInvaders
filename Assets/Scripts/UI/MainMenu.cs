using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private string _gameSceneName = "Game";

    private void Awake()
    {
        _playButton.onClick.AddListener(Play);
    }

    private void Play()
    {
        SceneManager.LoadScene(_gameSceneName);
    }
}

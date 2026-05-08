using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private Button _replayButton;

    private void Awake()
    {
        _replayButton.onClick.AddListener(Replay);
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
}

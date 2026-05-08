using UnityEngine;
using UnityEngine.InputSystem;

public class SkipRestButton : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
        WaveManager.Instance.OnRestStarted += Show;
        WaveManager.Instance.OnWaveStarted += Hide;
    }

    private void OnDestroy()
    {
        if (WaveManager.Instance == null) return;
        WaveManager.Instance.OnRestStarted -= Show;
        WaveManager.Instance.OnWaveStarted -= Hide;
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
            WaveManager.Instance.SkipRest();
    }

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}

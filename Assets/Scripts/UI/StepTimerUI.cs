using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StepTimerUI : MonoBehaviour
{
    [SerializeField] private Image    _fillImage;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private Color    _normalColor     = Color.white;
    [SerializeField] private Color    _urgentColor     = new Color(1f, 0.3f, 0.1f);
    [SerializeField] private float    _urgentThreshold = 0.15f;
    [SerializeField] private float    _restScale       = 2f;
    [SerializeField] private float    _scaleSpeed      = 4f;

    private void Update()
    {
        if (WaveManager.Instance == null) return;

        float t = WaveManager.Instance.NormalizedTimeRemaining;
        _fillImage.fillAmount = t;
        _fillImage.color      = t < _urgentThreshold ? _urgentColor : _normalColor;

        float targetScale = WaveManager.Instance.IsResting ? _restScale : 1f;
        float current     = transform.localScale.x;
        float next        = Mathf.Lerp(current, targetScale, Time.deltaTime * _scaleSpeed);
        transform.localScale = new Vector3(next, next, 1f);

        if (_label == null) return;
        int secs = Mathf.CeilToInt(WaveManager.Instance.SecondsRemaining);
        _label.text = WaveManager.Instance.IsResting
            ? $"Wave {WaveManager.Instance.CurrentWaveNumber}\n{secs}s"
            : $"{secs}s";
    }
}

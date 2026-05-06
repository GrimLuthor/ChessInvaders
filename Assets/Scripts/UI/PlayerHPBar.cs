using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private Image _fill;

    // Migrates to GamePalette in Phase 2
    [SerializeField] private Color _fullColor  = Color.green;
    [SerializeField] private Color _emptyColor = Color.red;

    private void Start()
    {
        _playerHealth.OnHealthChanged += UpdateBar;
        UpdateBar(_playerHealth.CurrentHp, _playerHealth.MaxHp);
    }

    private void OnDestroy()
    {
        _playerHealth.OnHealthChanged -= UpdateBar;
    }

    private void UpdateBar(float current, float max)
    {
        float fraction = max > 0f ? current / max : 0f;
        _fill.fillAmount = fraction;
        _fill.color = Color.Lerp(_emptyColor, _fullColor, fraction);
    }
}

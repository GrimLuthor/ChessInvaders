using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    [SerializeField] private EnemyBase _enemy;
    [SerializeField] private Image _fill;

    private void Start()
    {
        _enemy.OnHealthChanged += UpdateBar;
        UpdateBar(_enemy.CurrentHp, _enemy.MaxHp);
    }

    private void OnDestroy()
    {
        _enemy.OnHealthChanged -= UpdateBar;
    }

    private void UpdateBar(float current, float max)
    {
        _fill.fillAmount = max > 0f ? current / max : 0f;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    [SerializeField] private EnemyBase _enemy;
    [SerializeField] private Image     _fill;
    [SerializeField] private float     _hideDelay = 2f;

    private CanvasGroup _canvasGroup;
    private Coroutine   _hideRoutine;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        _canvasGroup.alpha = 0f;
    }

    private void Start()
    {
        _enemy.OnHealthChanged += OnHealthChanged;
        UpdateFill(_enemy.CurrentHp, _enemy.MaxHp);
    }

    private void OnDestroy()
    {
        _enemy.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float current, float max)
    {
        UpdateFill(current, max);
        Show();
    }

    private void UpdateFill(float current, float max)
    {
        _fill.fillAmount = max > 0f ? current / max : 0f;
    }

    private void Show()
    {
        _canvasGroup.alpha = 1f;

        if (_hideRoutine != null) StopCoroutine(_hideRoutine);
        _hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(_hideDelay);
        _canvasGroup.alpha = 0f;
        _hideRoutine = null;
    }
}

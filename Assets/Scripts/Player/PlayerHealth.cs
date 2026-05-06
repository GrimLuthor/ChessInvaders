using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private SpriteRenderer _spriteRenderer;  // Visual child's SpriteRenderer
    [SerializeField] private float _flashInterval = 0.05f;

    public event Action OnDeath;
    public event Action<float, float> OnHealthChanged;  // current, max

    public float CurrentHp { get; private set; }
    public float MaxHp     => _stats.MaxHp;

    private bool _isInvincible;

    private void Awake()
    {
        CurrentHp = _stats.MaxHp;
    }

    public void TakeDamage(float amount)
    {
        if (_isInvincible || CurrentHp <= 0f) return;

        CurrentHp = Mathf.Max(0f, CurrentHp - amount);
        OnHealthChanged?.Invoke(CurrentHp, _stats.MaxHp);

        if (CurrentHp <= 0f)
        {
            Die();
            return;
        }

        StartCoroutine(IFrameRoutine());
    }

    private void Die()
    {
        OnDeath?.Invoke();
        GameManager.Instance.TriggerGameOver();
        gameObject.SetActive(false);
    }

    private IEnumerator IFrameRoutine()
    {
        _isInvincible = true;

        float elapsed = 0f;
        while (elapsed < _stats.IFrameDuration)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
            yield return new WaitForSeconds(_flashInterval);
            elapsed += _flashInterval;
        }

        _spriteRenderer.enabled = true;
        _isInvincible = false;
    }

#if UNITY_EDITOR
    [ContextMenu("Debug: Take 10 Damage")]
    private void DebugTakeDamage() => TakeDamage(10f);
#endif
}

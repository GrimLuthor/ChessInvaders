using System.Collections;
using UnityEngine;

public class IntentIndicator : MonoBehaviour, IPoolable
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _pulseSpeed = 3f;
    [SerializeField] private float _minAlpha   = 0.2f;
    [SerializeField] private float _maxAlpha   = 0.7f;

    private Coroutine _pulseRoutine;

    public void OnSpawn()
    {
        gameObject.SetActive(true);
        Color baseColor = GameManager.Palette.IntentHighlightColor;
        baseColor.a = _maxAlpha;
        _spriteRenderer.color = baseColor;
        _pulseRoutine = StartCoroutine(Pulse());
    }

    public void OnDespawn()
    {
        if (_pulseRoutine != null)
        {
            StopCoroutine(_pulseRoutine);
            _pulseRoutine = null;
        }
        gameObject.SetActive(false);
    }

    private IEnumerator Pulse()
    {
        Color baseColor = GameManager.Palette.IntentHighlightColor;
        while (true)
        {
            float t = (Mathf.Sin(Time.time * _pulseSpeed) + 1f) * 0.5f;
            baseColor.a = Mathf.Lerp(_minAlpha, _maxAlpha, t);
            _spriteRenderer.color = baseColor;
            yield return null;
        }
    }
}

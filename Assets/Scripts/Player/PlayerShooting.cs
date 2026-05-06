using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;

    private float _cooldown;
    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        _cooldown -= Time.deltaTime;

        if (Mouse.current.leftButton.isPressed && _cooldown <= 0f)
        {
            Fire();
            _cooldown = 1f / _stats.FireRatePerSecond;
        }
    }

    private void Fire()
    {
        Vector2 mouseWorld = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction  = mouseWorld - (Vector2)transform.position;

        if (direction.sqrMagnitude < 0.001f) return;

        Projectile p = PoolManager.Instance.GetPlayerProjectile();
        p.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        p.Launch(direction, _stats.BulletSpeedInTiles, () => PoolManager.Instance.ReturnPlayerProjectile(p));
    }
}

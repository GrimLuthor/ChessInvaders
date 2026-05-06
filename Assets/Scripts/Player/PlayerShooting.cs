using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Fires a projectile toward the mouse cursor on left-click/hold.
/// Fire rate is a cooldown timer rather than a per-frame check — shoot once per interval.
/// </summary>
public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private float _fireRatePerSecond = 3f;
    [SerializeField] private float _projectileSpeedInTiles = 8f;

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
            _cooldown = 1f / _fireRatePerSecond;
        }
    }

    private void Fire()
    {
        Vector2 mouseWorld = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction  = (mouseWorld - (Vector2)transform.position);

        // Don't fire if the cursor is on top of the player
        if (direction.sqrMagnitude < 0.001f) return;

        Projectile p = PoolManager.Instance.GetPlayerProjectile();
        p.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        p.Launch(direction, _projectileSpeedInTiles, () => PoolManager.Instance.ReturnPlayerProjectile(p));
    }
}

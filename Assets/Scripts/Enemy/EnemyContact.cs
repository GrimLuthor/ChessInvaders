using UnityEngine;

public class EnemyContact : MonoBehaviour
{
    [SerializeField] private float _damage    = 5f;
    [SerializeField] private float _pushForce = 8f;

    private PlayerHealth  _playerHealth;
    private PlayerMovement _playerMovement;

    private void OnTriggerEnter2D(Collider2D other)
    {
        _playerHealth = other.GetComponent<PlayerHealth>();
        if (_playerHealth == null) return;

        _playerMovement = other.GetComponent<PlayerMovement>();
        _playerHealth.TakeDamage(_damage);
        Push(other.transform.position);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_playerHealth == null) return;
        _playerHealth.TakeDamage(_damage);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerHealth>() == null) return;
        _playerHealth   = null;
        _playerMovement = null;
    }

    private void Push(Vector3 playerPos)
    {
        if (_playerMovement == null) return;
        Vector2 dir = ((Vector2)playerPos - (Vector2)transform.position).normalized;
        _playerMovement.ApplyKnockback(dir, _pushForce);
    }
}

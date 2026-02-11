using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MissileEnemyMissile : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField]
    private float _moveSpeed = 6f;
    // lower = easier to dodge
    [SerializeField]
    private float _turnSpeedDegreesPerSecond = 120f;
    [SerializeField]
    private float _lifeTime = 5f;

    [Header("Explosion")]
    [SerializeField]
    private float _explosionRadius = 1.5f;
    [SerializeField]
    private int _explosionDamage = 1;
    [SerializeField]
    private GameObject _explosionPrefab;

    private Rigidbody2D _rb;
    private Transform _player;

    private bool _exploded = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            Debug.LogError("MissileEnemyMissile needs a Rigidbody2D.");
        }
    }

    private void Start()
    {
        // Find the player once (good enough for small projects)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }

        // explode automatically after lifetime
        StartCoroutine(LifeTimer());
    }

    private void FixedUpdate()
    {
        if (_exploded)
        {
            return;
        }
        if (_rb == null)
        {
            return;
        }

        // If no player exists, just keep going straight
        if (_player == null)
        {
            _rb.linearVelocity = transform.up * _moveSpeed;
            return;
        }

        // Direction from missile -> player
        Vector2 toPlayer = ((Vector2)_player.position - _rb.position).normalized;

        // Desired angle to face player, assuming missile's "forward" is transform.up
        float targetAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg - 90f;

        // Rotate toward target slowly (turn rate limited)
        float newAngle = Mathf.MoveTowardsAngle(_rb.rotation, targetAngle, _turnSpeedDegreesPerSecond * Time.fixedDeltaTime);
        _rb.MoveRotation(newAngle);

        // Move forward in whatever direction we're currently facing
        _rb.linearVelocity = (Vector2)transform.up * _moveSpeed;
    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(_lifeTime);
        Explode();
    }

    private void Explode()
    {
        if (_exploded)
        {
            return;
        }
        _exploded = true;

        // VFX
        if (_explosionPrefab != null)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        }

        // Damage player if within radius
        Collider2D hit = Physics2D.OverlapCircle(transform.position, _explosionRadius);
        // OverlapCircle finds ONE collider; better to check all:
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);

        foreach (Collider2D c in hits)
        {
            if (c != null && c.CompareTag("Player"))
            {
                PlayerHealth ph = c.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(_explosionDamage);
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_exploded)
        {
            return;
        }

        // Player bullet destroys the missile
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            AudioManager.Instance?.PlayEnemyMissileExplosionSFX();
            Explode(); // or just Destroy(gameObject) if you DON'T want it to explode when shot
        }

        if (other.CompareTag("Player"))
        {
            if (_explosionPrefab != null)
            {
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                AudioManager.Instance?.PlayEnemyMissileExplosionSFX();
            }

            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage();
            }

            Destroy(this.gameObject);
        }
    }

    // Optional: helps you see explosion radius in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}

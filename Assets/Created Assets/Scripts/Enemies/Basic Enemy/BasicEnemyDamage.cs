using UnityEngine;

public class BasicEnemyDamage : MonoBehaviour
{
    // This all works with the new health powerup. 
    // It gives a random chance upon each death for one to drop.
    [Header("Powerup Drop")]
    [SerializeField] 
    private GameObject _healthPowerupPrefab;

    [Tooltip("0 = never drops, 1 = always drops. Example: 0.2 = 20% chance.")]
    [Range(0f, 1f)]
    [SerializeField]
    private float _healthDropChance = 0.2f;

    [SerializeField]
    private GameObject _explosionPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player collision: damage player, kill enemy
        if (other.CompareTag("Player"))
        {
            // This grabs the PlayerHealth script so that it can call on the TakeDamage method to take away one life with each impact. 
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage();
            }

            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            AudioManager.Instance?.PlayEnemyDeathExplosion();

            SpawnManager sm = FindFirstObjectByType<SpawnManager>();
            if (sm != null)
            {
                sm.OnEnemyKilled();
            }

            Destroy(gameObject);
            return;
        }

        // Bullet collision: kill bullet, maybe drop health, kill enemy
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);

            TryDropHealthPowerup();

            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            AudioManager.Instance?.PlayEnemyDeathExplosion();

            SpawnManager sm = FindFirstObjectByType<SpawnManager>();
            if (sm != null)
            {
                sm.OnEnemyKilled();
            }

            Destroy(gameObject);
        }
    }

    private void TryDropHealthPowerup()
    {
        if (_healthPowerupPrefab == null)
        {
            return;
        }

        // Random.value returns a float between 0.0 and 1.0
        if (Random.value <= _healthDropChance)
        {
            Instantiate(_healthPowerupPrefab, transform.position, Quaternion.identity);
        }
    }
}
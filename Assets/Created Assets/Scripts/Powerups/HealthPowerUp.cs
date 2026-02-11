using UnityEngine;

public class HealthPowerUp : MonoBehaviour
{
    [SerializeField] 
    private float _speed = 5f;

    private void Update()
    {
        // Move straight down at 5 units/sec (frame-rate independent)
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        // Optional cleanup if it goes off screen
        if (transform.position.y < -6.5f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only react to the player
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // Call the player's heal method
        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.Heal();
        }

        // Destroy the pickup immediately after use
        Destroy(gameObject);
    }
}
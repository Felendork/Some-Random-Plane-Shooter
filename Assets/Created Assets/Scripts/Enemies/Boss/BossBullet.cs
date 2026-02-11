using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [SerializeField] 
    private float _speed = 10f;
    [SerializeField] 
    private float _lifeTime = 5f;

    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    // Set the velocity direction for the bullet
    public void SetVelocity(Vector2 direction)
    {
        if (_rb != null)
        {
            _rb.linearVelocity = direction * _speed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(1);
            }
            Destroy(gameObject);
        }
    }
}

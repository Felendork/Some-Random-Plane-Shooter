using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    private int _bulletSpeed = 10;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (_rb == null)
        {
            Debug.LogError("Player Bullet needs a RigidBody2D component");
        }
    }

    private void FixedUpdate()
    {
        // This tells the Rigidbody to move upwards in a straight line, at a rate of 10. This moves the model of the bullet with it for a smooth process. 
        _rb.linearVelocity = Vector2.up * _bulletSpeed;
    }

    // This is called when the bullet leaves the camera view. Once it does, the bullet will destroy itself- otherwise it will keep traveling indefinitely and taking up space.
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}

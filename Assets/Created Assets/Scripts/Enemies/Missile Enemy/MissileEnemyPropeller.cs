using UnityEngine;

public class PropellerRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] 
    private float _rotationSpeed = 1000f; 
    [SerializeField] 
    private bool _rotateClockwise = true;

    void Update()
    {
        // Calculate rotation direction
        float direction = _rotateClockwise ? -1f : 1f;

        // Rotate around Z-axis (forward vector) for 2D sprites
        transform.Rotate(Vector3.forward * direction * _rotationSpeed * Time.deltaTime);
    }

    // Optional: Method to change rotation speed at runtime
    public void SetRotationSpeed(float newSpeed)
    {
        _rotationSpeed = Mathf.Max(0f, newSpeed); // Prevent negative speed
    }

    // Optional: Method to reverse rotation direction
    public void ReverseDirection()
    {
        _rotateClockwise = !_rotateClockwise;
    }
}

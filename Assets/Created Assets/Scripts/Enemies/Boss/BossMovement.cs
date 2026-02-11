using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Enter Position")]
    [SerializeField] 
    private float _startY = 7f;
    [SerializeField] 
    private float _targetY = 3.5f;
    [SerializeField] 
    private float _enterSpeed = 3f;

    [Header("Strafe")]
    [SerializeField] 
    private float _horizontalSpeed = 2f;
    [SerializeField] 
    private float _leftX = -8.35f;
    [SerializeField] 
    private float _rightX = 8.35f;

    private Rigidbody2D _rb;
    private bool _entering = true;
    private int _dirX = -1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            Debug.LogError("BossMovement needs a Rigidbody2D.");
        }
    }

    private void OnEnable()
    {
        // Put boss at the top when enabled/spawned
        transform.position = new Vector3(transform.position.x, _startY, transform.position.z);

        _entering = true;
        _dirX = -1;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (_rb == null) return;

        if (_entering)
        {
            float newY = Mathf.MoveTowards(_rb.position.y, _targetY, _enterSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(new Vector2(_rb.position.x, newY));

            if (Mathf.Abs(_rb.position.y - _targetY) < 0.01f)
            {
                _entering = false;
            }

            return;
        }

        // Strafe left/right
        _rb.linearVelocity = new Vector2(_dirX * _horizontalSpeed, 0f);

        // Bounce off edges
        if (_rb.position.x <= _leftX)
        {
            _dirX = 1;
            _rb.position = new Vector2(_leftX, _rb.position.y);
        }
        else if (_rb.position.x >= _rightX)
        {
            _dirX = -1;
            _rb.position = new Vector2(_rightX, _rb.position.y);
        }
    }
}

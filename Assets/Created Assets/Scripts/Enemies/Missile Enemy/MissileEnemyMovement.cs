using UnityEngine;

public class MissileEnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float _horizontalSpeed = 4f;

    // how fast it drops into position
    [SerializeField]
    private float _enterSpeed = 6f;

    // "grazing" top edge (tweak for your camera)
    [SerializeField] 
    private float _targetY = 5.5f;      
    [SerializeField] 
    private float _leftX = -8.35f;
    [SerializeField] 
    private float _rightX = 8.35f;

    private Rigidbody2D _rb;

    // -1 = moving left, +1 = moving right
    private int _dirX = -1;

    // True until it reaches the target Y
    private bool _entering = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            Debug.LogError("SideToSideEnemyMovement needs a Rigidbody2D.");
        }
    }

    private void OnEnable()
    {
        // Start moving left by default
        _dirX = -1;
        _entering = true;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (_rb == null)
        {
            return;
        }


        if (_entering)
        {
            // Drop down until we reach the target Y
            float newY = Mathf.MoveTowards(_rb.position.y, _targetY, _enterSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(new Vector2(_rb.position.x, newY));

            if (Mathf.Abs(_rb.position.y - _targetY) < 0.01f)
            {
                _entering = false;
            }

            return;
        }

        // Move strictly left/right at the "grazing" Y level
        _rb.linearVelocity = new Vector2(_dirX * _horizontalSpeed, 0f);

        // Bounce at edges
        if (_rb.position.x <= _leftX)
        {
            _dirX = 1;
            _rb.position = new Vector2(_leftX, _rb.position.y); // snap inside bounds
        }
        else if (_rb.position.x >= _rightX)
        {
            _dirX = -1;
            _rb.position = new Vector2(_rightX, _rb.position.y);
        }
    }
}

using UnityEngine;

public class PlayerEnter : MonoBehaviour
{
    [Header("Entrance Movement")]
    [SerializeField] 
    private float _startY = -7f;
    [SerializeField] 
    private float _targetY = 0f;
    [SerializeField] 
    private float _enterSpeed = 6f;

    [Header("Control Script To Toggle")]
    // drag PlayerControls here
    [SerializeField] 
    private MonoBehaviour _playerControlScript; 

    private bool _entering = true;

    private void Start()
    {
        // If not assigned, try to find PlayerControls automatically
        if (_playerControlScript == null)
        {
            _playerControlScript = GetComponent<PlayerControls>();
        }

        // Place player below screen and disable controls
        transform.position = new Vector3(transform.position.x, _startY, transform.position.z);

        if (_playerControlScript != null)
        {
            _playerControlScript.enabled = false;
        }

        // Optional: stop physics while entering so nothing interferes
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }
    }

    private void Update()
    {
        if (!_entering)
        {
            return;
        }

        Vector3 pos = transform.position;
        pos.y = Mathf.MoveTowards(pos.y, _targetY, _enterSpeed * Time.deltaTime);
        transform.position = pos;

        if (Mathf.Approximately(pos.y, _targetY))
        {
            _entering = false;

            // Re-enable physics + controls
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.simulated = true;
            }

            if (_playerControlScript != null)
            {
                _playerControlScript.enabled = true;
            }
        }
    }
}

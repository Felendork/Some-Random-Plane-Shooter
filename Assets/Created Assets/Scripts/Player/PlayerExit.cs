using UnityEngine;

public class PlayerExit : MonoBehaviour
{
    [SerializeField] 
    private float _exitSpeed = 6f;
    [SerializeField] 
    private float _offscreenY = 7f; // set above top of camera view

    private bool _exiting = false;

    private void OnEnable()
    {
        UIManager.LevelCompleteEvent += StartExit;
    }

    private void OnDisable()
    {
        UIManager.LevelCompleteEvent -= StartExit;
    }

    private void StartExit()
    {
        _exiting = true;

        // Optional: if you have Rigidbody2D movement, stop it:
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false; // prevents physics from fighting your movement
        }

        // Optional: disable your PlayerControls so input can't move them during exit
        PlayerControls pc = GetComponent<PlayerControls>();
        if (pc != null)
        {
            pc.enabled = false;
        }
    }

    private void Update()
    {
        if (!_exiting)
        {
            return;
        }

        transform.Translate(Vector3.up * _exitSpeed * Time.deltaTime);

        if (transform.position.y > _offscreenY)
        {
            // At this point you can load the next scene, show menu, etc.
            // For now just stop moving.
            _exiting = false;
        }
    }
}

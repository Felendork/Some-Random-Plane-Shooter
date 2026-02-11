using UnityEngine;

// Need this line here. Without the InputSystem, it can't call on the PlayerInput I created. 
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [Header("Movement")]
    // This is the speed at which the Player will move. 
    private int _moveSpeed = 10;

    // Using the RigidBody is good for smooth, physics based movement.
    private Rigidbody2D _rb;

    [Header("Input Controls")]
    // This references the Input created in the project view.
    private PlayerInput _input;

    // This is the Vector2 input. It'll help with controlling the Player in a 2D space. If it was Vector3, it would be in 3D space. But I'm only focusing on the X and Y axis.
    private Vector2 _moveInput;

    [Header("Shooting Settings")]
    // This adds a reference to the bullet, so I may place it within the inspector for the Player. 
    // 'SerializeField' just means that, even if it's private (so nothing else can call on it but this script), that I'll be able to see it in the Inspector to mess with.
    [SerializeField]
    private GameObject _bullet;

    // This is the fire point- the transform is just 'location', as well as scale and rotation. The firepoint will be an empty game object made as a child of the Player
    // so that the bullet can fire from it, rather than from inside of the player. 
    [SerializeField]
    private Transform _firePoint;

    // These two help determine the cooldown so the player can't spam the bullet endlessly. 
    [SerializeField]
    private float _shootCooldown = 0.3f;
    private float _lastShotTime;

    // This all holds the information for the edges of which the Player shouldn't move beyond. 
    [Header("Screen Boundaries")]
    private float _MIN_X = -8.35f;
    private float _MAX_X = 8.35f;
    private float _MIN_Y = -4.57f;
    private float _MAX_Y = 0f;

    // void Awake runs exactly once, as soon as the game object is created/the script is loaded, even before void Start. Good for one-time setups like grabbing references,
    // creating objects, etc.
    private void Awake()
    {
        // This is referencing the RigidBody2D that I will attach to the Player. 
        _rb = GetComponent<Rigidbody2D>();

        // This is a safety check. If I forget to, or some bug removes it, the game will tell me. 
        if (_rb == null)
        {
            Debug.LogError("Player needs a RigidBody2D Component!");
        }

        // This one creates an instance of the generated Input Actions class from the PlayerInput. Without this, Unity won't create/call on it.
        _input = new PlayerInput();
    }

    // Every time the script/GameObject becomes active/enabled. It starts to listen, subscribe, turn things on. 
    private void OnEnable()
    {
        _input.Enable();
        // This adds the Shoot Input into the list of things to do once the script is enabled. 
        _input.Player.Shoot.performed += OnShoot;
    }

    // Inversely, this is the opposite. Every time the script/GameObject is turned off or is disabled, it'll stop listening. 
    private void OnDisable()
    {
        // And this tells it to stop doing things when the script is disabled. 
        _input.Player.Shoot.performed -= OnShoot;
        _input.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // It's blank right now, but I will be using it later. 
    void Start()
    {

    }

    // Update is called once per frame, typically 60-144 frames a second, depending on your hardware/settings. It's good for reading input, animations, UI updates, non-physics
    // movement (Transform.Translate).
    void Update()
    {
        // This line reads the current input state from the new Input System and stores it into a variable. It will be used in FixedUpdate to update the movement inputs. 
        _moveInput = _input.Player.Move.ReadValue<Vector2>();
    }

    // FixedUpdate is fixed onto 50 times a second, and does so regardless of the frames. For physics based movement and consistent simulations, this is better to use. 
    private void FixedUpdate()
    {
        // This takes the direction the player is pressing, stored in _moveInput in Update(), multiply it by how fast I want it (_moveSpeed), and tells the Player to move in that
        // direction now. 
        Vector2 velocity = _moveInput * _moveSpeed;
        _rb.linearVelocity = velocity;
    }

    // LateUpdate runs last in the frame, after FixedUpdate's physics and Update's input. It corrects 'overshoot' from velocity without fighting the other code. 
    // It is also frame-rate smooth, unlike FixedUpdate which is physics-timed. 
    private void LateUpdate()
    {
        // This copies the player's current position. 
        Vector3 pos = transform.position;

        // This tells the player, more or less, "Don't move beyond the X and Y axis boundaries."
        pos.x = Mathf.Clamp(pos.x, _MIN_X, _MAX_X);
        pos.y = Mathf.Clamp(pos.y, _MIN_Y, _MAX_Y);

        // This applies the clamped position to the player's position, stopping them from moving beyond it. 
        transform.position = pos;
    }

    // Like OnMove, this is for the Shoot input. 
    private void OnShoot(InputAction.CallbackContext context)
    {
        // If Time.time (current time) is less than the last shot time, + the cooldown, then it won't fire again until it reaches it. This stops it from firing for 0.3 seconds
        // to prevent an endless spam of bullets. 
        if (Time.time < _lastShotTime + _shootCooldown)
        {
            return;
        }

        // This is a safety for the event the bullet doesn't exist. It just tells me to place it in the Inspector. 
        if (_bullet == null)
        {
            Debug.LogWarning("No Bullet Prefab Assigned.");
            return;
        }

        // This tells the game that the fire point (which will be assigned to the player) is the exact point in which the bullet will fire from. 
        Transform spawnPoint = _firePoint != null ? _firePoint : transform;

        // 'Instantiate' is another way of saying 'clone' or 'spawn' or 'create'. Now when this method is called, the player will spawn in a bullet at the fire point, and at
        // it's rotation so it doesn't spin about or anything. 
        Instantiate(_bullet, spawnPoint.position, spawnPoint.rotation);

        // Plays the sound for the Player Bullet after the bullet is fired. 
        AudioManager.Instance?.PlayPlayerBullet();

        // This is recording the time, so that it knows when it should and shouldn't wait between shots. 
        _lastShotTime = Time.time;
    }
}

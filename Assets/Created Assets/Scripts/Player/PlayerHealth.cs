using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    // I made these public so that when I have a UI element to display the lives, these can be called on. 
    public int maxLives = 5;
    public int currentLives;

    [SerializeField]
    private GameObject _explosionPrefab;

    private UIManager _uiManager;

    // This is all for the shake, when the Player takes damage they'll shake for half a second just slightly. 
    [Header("Damage Shake")]
    [SerializeField]
    private float _shakeDuration = 0.5f;
    [SerializeField]
    private float _shakeIntensity = 0.15f;
    private Vector3 _originalPosition;

    // The duration is how long the player can't take damage for after being hit. 
    // The bool is a check for the coroutine that will keep the Player invincible. 
    [Header("Invincibility Frames")]
    [SerializeField]
    private float _iFrameDuration = 1f;
    private bool _isInvincible = false;

    // This is a bool (boolean) variable. It is just a 'true or false' statement, good for checking if specific things should or shouldn't happen based on if it's true or not. 
    private bool _isShaking = false;

    [Header("Damage Visuals (Fire)")]
    // turns on at 3 lives or less
    [SerializeField]
    private GameObject _fireStage1;

    // turns on at 1 life or less
    [SerializeField]
    private GameObject _fireStage2;


    private void Awake()
    {
        // Starts the scene with the max lives.
        currentLives = maxLives;
    }

    private void Start()
    {

        _uiManager = FindFirstObjectByType<UIManager>();

        UpdateFireEffects();

        // This calls on the UIManager script to update its sprites to match the lives. 
        if (_uiManager != null)
        {
            _uiManager.UpdateLives(currentLives);
        }
    }

    // This will be called when the Player takes damage from the enemy scripts.
    public void TakeDamage(int amount = 1)
    {
        // If the Player is invincible, then the code stops there as to not let the player take damage again. 
        if (_isInvincible)
        {
            return;
        }

        // These two determine that if the player is already at 0 lives or less, they are already dead and so nothing can run here. 
        if (amount <= 0)
        {
            return;
        }

        if (currentLives <= 0)
        {
            return;
        }

        // When this script is called, assuming they aren't dead yet, the player's health will decrease by the amount (1). 
        currentLives -= amount;

        // This determines that the player can't reach past 5 lives, or go lower than 0. 
        currentLives = Mathf.Clamp(currentLives, 0, maxLives);

        AudioManager.Instance?.PlayPlayerDamage();

        // Immediately after the life lost is calculated, the I-frames kick in. 
        StartCoroutine(InvincibilityFrames());

        UpdateFireEffects();

        if (_isShaking == false)
        {
            StartCoroutine(DamageShake());
        }

        if (_uiManager != null)
        {
            _uiManager.UpdateLives(currentLives);

            if (currentLives <= 0)
            {
                _uiManager.ShowGameOver();
            }
        }

        // Once the player reaches 0, they dead. 
        if (currentLives <= 0)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            AudioManager.Instance?.PlayPlayerDeathExplosion();
            Destroy(gameObject);
        }
    }

    // This will be called in the Health Power Up script. 
    public void Heal(int amount = 1)
    {
        if (amount <= 0)
        {
            return;
        }

        if (currentLives <= 0)
        {
            return;
        }

        // When interacted with, the player will add amount (1) to their lives. 
        currentLives += amount;
        currentLives = Mathf.Clamp(currentLives, 0, maxLives);

        AudioManager.Instance?.PlayHealUpSound();

        UpdateFireEffects();

        if (_uiManager != null)
        {
            _uiManager.UpdateLives(currentLives);
        }
    }

    private IEnumerator DamageShake()
    {
        // When this is called, it checks that its now true. 
        _isShaking = true;

        // This ensures that when it's done shaking, it will return to its original position before it started shaking. 
        _originalPosition = transform.position;
        float elapsedTime = 0f;

        // While the elapsed time (0) is less than the duration, it will generate the code below until it reaches 0.5 seconds. Once it does, then it stops, returns to its position
        // and this coroutine is no longer active. 
        while (elapsedTime < _shakeDuration)
        {
            // Generate random offset
            float offsetX = Random.Range(-_shakeIntensity, _shakeIntensity);
            float offsetY = Random.Range(-_shakeIntensity, _shakeIntensity);

            // Apply shake
            transform.position = _originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset to original position
        transform.position = _originalPosition;
        _isShaking = false;
    }

    // This turns the bool to true, then turns on the duration for the I-frames, and then turns it off right after. 
    private IEnumerator InvincibilityFrames()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_iFrameDuration);
        _isInvincible = false;
    }

    private void UpdateFireEffects()
    {
        // Stage 1: ON when 3 or fewer lives
        if (_fireStage1 != null)
        {
            _fireStage1.SetActive(currentLives <= 3 && currentLives > 0);
        }

        // Stage 2: ON when 1 or fewer lives (but still alive)
        if (_fireStage2 != null)
        {
            _fireStage2.SetActive(currentLives <= 1 && currentLives > 0);
        }
    }
}

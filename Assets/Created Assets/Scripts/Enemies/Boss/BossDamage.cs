using UnityEngine;

public class BossDamage : MonoBehaviour
{
    [Header("Boss HP")]
    [SerializeField]
    private int _maxHP = 50;
    [SerializeField]
    private int _currentHP;

    [Header("Damage Stages (every 10 HP)")]
    [Tooltip("Index 0 turns on at 40 HP, 1 at 30, 2 at 20, 3 at 10. (Optional: add more)")]
    [SerializeField]
    private GameObject[] _damageStageVFX;

    [SerializeField]
    private AudioClip _damageStageSFX;

    [Header("Health Drop (every 10 HP)")]
    [SerializeField]
    private GameObject _healthPickupPrefab;

    [SerializeField]
    private float _dropMinX = -8.35f;
    [SerializeField]
    private float _dropMaxX = 8.35f;
    [SerializeField]
    private float _dropY = 5.7f;

    [Header("Death")]
    [SerializeField]
    private GameObject _explosionPrefab;

    // If your UI panel is called "Boss Defeated" instead of "Level Complete",
    // make a matching method in UIManager (ShowBossDefeated / ShowLevelComplete etc).
    [SerializeField]
    private bool _useBossDefeatedPanel = true;

    [SerializeField]
    private AudioClip _victoryMusic;

    private UIManager _ui;
    private int _nextThreshold; // next multiple of 10 to trigger at (40,30,20,10,0)

    private void Awake()
    {
        _currentHP = _maxHP;
        _nextThreshold = _maxHP - 10; // 40 first
    }

    private void Start()
    {
        _ui = FindFirstObjectByType<UIManager>();

        // Ensure all stage VFX start off
        if (_damageStageVFX != null)
        {
            for (int i = 0; i < _damageStageVFX.Length; i++)
            {
                if (_damageStageVFX[i] != null)
                {
                    _damageStageVFX[i].SetActive(false);
                }
            }
        }
    }

    // Call this from bullets when boss is hit (or use OnTriggerEnter2D on the boss)
    public void TakeDamage(int amount = 1)
    {
        if (amount <= 0)
        {
            return;
        }
        if (_currentHP <= 0)
        {
            return;
        }

        _currentHP -= amount;
        _currentHP = Mathf.Clamp(_currentHP, 0, _maxHP);

        // Handle crossing 10-HP thresholds (supports big hits too)
        while (_currentHP <= _nextThreshold && _nextThreshold >= 0)
        {
            OnThresholdReached(_nextThreshold);
            _nextThreshold -= 10;
        }

        if (_currentHP <= 0)
        {
            Die();
        }
    }

    private void OnThresholdReached(int thresholdHP)
    {
        // Example: thresholdHP = 40 => stage index 0
        // thresholdHP = 30 => stage index 1, etc
        int stageIndex = (_maxHP - thresholdHP) / 10 - 1;

        if (_damageStageVFX != null && stageIndex >= 0 && stageIndex < _damageStageVFX.Length)
        {
            if (_damageStageVFX[stageIndex] != null)
            {
                _damageStageVFX[stageIndex].SetActive(true);
            }
        }

        // Play SFX for the stage
        if (_damageStageSFX != null)
        {
            // You can route this through AudioManager if you prefer; direct is fine too.
            AudioSource.PlayClipAtPoint(_damageStageSFX, transform.position);
        }

        // Spawn a health pickup each threshold
        if (_healthPickupPrefab != null)
        {
            float x = Random.Range(_dropMinX, _dropMaxX);
            Vector3 pos = new Vector3(x, _dropY, 0f);
            Instantiate(_healthPickupPrefab, pos, Quaternion.identity);
        }
    }

    private void Die()
    {
        if (_explosionPrefab != null)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(_damageStageSFX, transform.position);
        }

        // Show boss defeated panel (you’ll add/rename this UI method)
        if (_ui != null)
        {
            if (_useBossDefeatedPanel)
            {
                _ui.ShowLevelComplete();
            }
        }

        // Victory music
        if (AudioManager.Instance != null && _victoryMusic != null)
        {
            AudioManager.Instance.PlayMusic(_victoryMusic);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage();
            Destroy(other.gameObject);
        }
    }
}

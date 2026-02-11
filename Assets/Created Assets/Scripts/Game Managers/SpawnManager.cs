using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [Header("Level 1 (Scene 1)")]
    [SerializeField] 
    private GameObject _level1EnemyPrefab;
    [SerializeField] 
    private int _level1MaxEnemies = 15;

    [Header("Level 2 (Scene 2)")]
    [SerializeField] 
    private GameObject _level2EnemyPrefabA;
    [SerializeField] 
    private GameObject _level2EnemyPrefabB;
    [SerializeField] 
    private int _level2MaxEnemies = 25;

    [Header("Spawn Timing")]
    [SerializeField] 
    private float _spawnDelay = 3f;

    [Header("Spawn Position")]
    [SerializeField] 
    private float _minX = -8.35f;
    [SerializeField] 
    private float _maxX = 8.35f;
    [SerializeField] 
    private float _spawnY = 5.7f;

    [Header("UI")]
    [SerializeField] 
    private UIManager _uiManager;

    private int _enemiesSpawned = 0;
    private int _enemiesKilled = 0;
    private int _maxEnemiesThisLevel = 0;

    private void Start()
    {
        if (_uiManager == null)
        {
            _uiManager = FindFirstObjectByType<UIManager>();
        }

        SetupForCurrentScene();
        StartCoroutine(SpawnEnemies());
    }

    private void SetupForCurrentScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Assumption: Level 1 is buildIndex 1, Level 2 is buildIndex 2
        if (sceneIndex == 1)
        {
            _maxEnemiesThisLevel = _level1MaxEnemies;
        }
        else if (sceneIndex == 2)
        {
            _maxEnemiesThisLevel = _level2MaxEnemies;
        }
        else
        {
            // Default fallback (won't spawn if not set)
            _maxEnemiesThisLevel = 0;
        }

        _enemiesSpawned = 0;
        _enemiesKilled = 0;
    }

    private IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(_spawnDelay);

        while (_enemiesSpawned < _maxEnemiesThisLevel)
        {
            SpawnEnemyForCurrentLevel();
            _enemiesSpawned++;

            yield return new WaitForSeconds(_spawnDelay);
        }

        Debug.Log("All enemies spawned for this level!");
    }

    private void SpawnEnemyForCurrentLevel()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        GameObject prefabToSpawn = null;

        if (sceneIndex == 1)
        {
            prefabToSpawn = _level1EnemyPrefab;
        }
        else if (sceneIndex == 2)
        {
            // Randomly choose between the two enemy types
            // (50/50). You can change this later to weighted.
            prefabToSpawn = (Random.value < 0.5f) ? _level2EnemyPrefabA : _level2EnemyPrefabB;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogError("SpawnManager: Enemy prefab not assigned for this level.");
            return;
        }

        float randomX = Random.Range(_minX, _maxX);
        Vector3 spawnPosition = new Vector3(randomX, _spawnY, 0f);

        Instantiate(prefabToSpawn, spawnPosition, Quaternion.Euler(0, 0, 180));
    }

    public bool IsSpawningComplete()
    {
        return _enemiesSpawned >= _maxEnemiesThisLevel;
    }

    public void OnEnemyKilled()
    {
        _enemiesKilled++;

        if (IsSpawningComplete() && _enemiesKilled >= _maxEnemiesThisLevel)
        {
            if (_uiManager != null)
            {
                _uiManager.ShowLevelComplete();
            }
            else
            {
                Debug.LogWarning("SpawnManager: UIManager not found, can't show Level Complete panel.");
            }
        }
    }
}


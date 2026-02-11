using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Lives UI")]
    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private Sprite[] _lifeSprites;

    [Header("Game Over UI")]
    [SerializeField]
    private GameObject _gameOverPanel;
    [SerializeField]
    private CanvasGroup _gameOverCanvasGroup;
    [SerializeField]
    private float _gameOverFadeDuration = 1f;

    [Header("Level Complete UI")]
    [SerializeField]
    private GameObject _levelCompletePanel;
    [SerializeField]
    private CanvasGroup _levelCompleteCanvasGroup;
    [SerializeField]
    private float _levelCompleteFadeDuration = 1f;

    public static System.Action LevelCompleteEvent;

    private Coroutine _fadeRoutine;

    private void Start()
    {
        // Validate Lives UI
        if (_livesImage == null)
        {
            Debug.LogWarning("UIManager: Lives Image is not assigned!");
        }

        if (_lifeSprites == null || _lifeSprites.Length < 6)
        {
            Debug.LogWarning("UIManager: Life Sprites array should have 6 sprites (0-5).");
        }

        // Initialize Game Over Panel
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(false);
        }

        if (_gameOverCanvasGroup != null)
        {
            _gameOverCanvasGroup.alpha = 0f;
            _gameOverCanvasGroup.interactable = false;
            _gameOverCanvasGroup.blocksRaycasts = false;
        }

        // Initialize Level Complete Panel
        if (_levelCompletePanel != null)
        {
            _levelCompletePanel.SetActive(false);
        }

        if (_levelCompleteCanvasGroup != null)
        {
            _levelCompleteCanvasGroup.alpha = 0f;
            _levelCompleteCanvasGroup.interactable = false;
            _levelCompleteCanvasGroup.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Load main menu
            SceneManager.LoadScene(0);
        }
    }


    public void UpdateLives(int currentLives)
    {
        if (_livesImage == null)
        {
            return;
        }
        if (_lifeSprites == null || _lifeSprites.Length == 0)
        {
            return;
        }

        currentLives = Mathf.Clamp(currentLives, 0, _lifeSprites.Length - 1);
        _livesImage.sprite = _lifeSprites[currentLives];
    }


    public void ShowGameOver()
    {
        if (_gameOverPanel == null || _gameOverCanvasGroup == null)
        {
            Debug.LogWarning("UIManager: GameOver Panel/CanvasGroup not assigned.");
            return;
        }

        _gameOverPanel.SetActive(true);

        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }

        _fadeRoutine = StartCoroutine(FadeInPanel(_gameOverCanvasGroup, _gameOverFadeDuration));
    }


    // Call this when the last enemy is destroyed to show level complete screen
    public void ShowLevelComplete()
    {
        if (_levelCompletePanel == null || _levelCompleteCanvasGroup == null)
        {
            Debug.LogWarning("UIManager: LevelComplete Panel/CanvasGroup not assigned.");
            return;
        }

        // Activate panel
        _levelCompletePanel.SetActive(true);

        LevelCompleteEvent?.Invoke();

        // Stop any previous fade
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }

        // Start fade-in coroutine
        _fadeRoutine = StartCoroutine(FadeInPanel(_levelCompleteCanvasGroup, _levelCompleteFadeDuration));
    }


    // Generic fade-in coroutine for any CanvasGroup
    private IEnumerator FadeInPanel(CanvasGroup canvasGroup, float duration)
    {
        // Start invisible and non-interactive
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float t = 0f;

        // Fade from 0 to 1 alpha
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // Enable interaction after fade completes
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        _fadeRoutine = null;
    }

    public void RestartScene()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void LoadScene1()
    {
        SceneManager.LoadScene(1);
    }


    // Load the next scene in build order (for Level Complete screen)
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if next scene exists
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("UIManager: No next scene available! Looping to main menu.");

            // Loop back to main menu.
            SceneManager.LoadScene(0);
        }
    }

    public void QuitGame()
    {
        // Works in a built game
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] 
    private AudioSource _musicSource;
    [SerializeField] 
    private AudioSource _sfxSource;

    [Header("SFX Clips")]
    [SerializeField] 
    private AudioClip _playerBulletClip;
    [SerializeField] 
    private AudioClip _playerDamageClip;
    [SerializeField] 
    private AudioClip _playerDeathExplosionClip;

    [SerializeField]
    private AudioClip _healUpClip;

    [SerializeField]
    private AudioClip _enemyBulletClip;
    [SerializeField]
    private AudioClip _enemyDeathExplosionClip;
    [SerializeField]
    private AudioClip _missileSFXClip;
    [SerializeField]
    private AudioClip _missileExplosionClip;
    [SerializeField]
    private AudioClip _bossRapidFireClip;

    // Changed from the one music clip, to three. 
    [Header("Level Music Clips")]
    [SerializeField] 
    private AudioClip _level1Music;
    [SerializeField] 
    private AudioClip _level2Music;
    [SerializeField]
    private AudioClip _level3Music;

    [Header("Volumes")]
    [Range(0f, 1f)][SerializeField]
    private float _musicVolume = 0.6f;
    [Range(0f, 1f)][SerializeField] 
    private float _sfxVolume = 1f;

    private void Awake()
    {
        // Singleton pattern (one AudioManager in the whole game)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // If you forgot to assign sources, try grabbing them
        if (_musicSource == null || _sfxSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length >= 2)
            {
                _musicSource = sources[0];
                _sfxSource = sources[1];
            }
        }

        ApplyVolumes();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Decide which clip should play for this scene.
        AudioClip target = null;

        // Use either scene.buildIndex or scene.name.
        // Example assumes: 1 = Level1, 2 = Level2, 3 = Level3
        switch (scene.buildIndex)
        {
            case 1: target = _level1Music; break;
            case 2: target = _level2Music; break;
            case 3: target = _level3Music; break;
        }

        if (target != null)
        {
            PlayMusicIfNotAlready(target);
        }
    }

    private void PlayMusicIfNotAlready(AudioClip clip)
    {
        if (_musicSource == null || clip == null)
        {
            return;
        }

        // Prevent restarting the same track when reloading UI / objects
        if (_musicSource.isPlaying && _musicSource.clip == clip)
        {
            return;
        }

        PlayMusic(clip);
    }

    private void ApplyVolumes()
    {
        if (_musicSource != null)
        {
            _musicSource.volume = _musicVolume;
        }

        if (_sfxSource != null)
        {
            _sfxSource.volume = _sfxVolume;
        }

    }

    // ---------- Generic helpers ----------
    private void PlaySFX(AudioClip clip)
    {
        if (_sfxSource == null || clip == null)
        {
            return;
        }

        _sfxSource.PlayOneShot(clip, _sfxVolume);
    }

    // ---------- Public SFX methods ----------
    public void PlayPlayerBullet() => PlaySFX(_playerBulletClip);
    public void PlayPlayerDamage() => PlaySFX(_playerDamageClip);
    public void PlayPlayerDeathExplosion() => PlaySFX(_playerDeathExplosionClip);

    public void PlayHealUpSound() => PlaySFX(_healUpClip);

    public void PlayEnemyBullet() => PlaySFX(_enemyBulletClip);
    public void PlayEnemyDeathExplosion() => PlaySFX(_enemyDeathExplosionClip);

    public void PlayEnemyMissileSFX() => PlaySFX(_missileSFXClip);
    public void PlayEnemyMissileExplosionSFX() => PlaySFX(_missileExplosionClip);

    public void PlayBossRapidFireSFX() => PlaySFX(_bossRapidFireClip);

    // ---------- Music methods ----------
    public void PlayMusic(AudioClip music)
    {
        if (_musicSource == null || music == null)
        {
            return;
        }

        _musicSource.clip = music;
        _musicSource.loop = true;
        _musicSource.volume = _musicVolume;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        if (_musicSource == null)
        {
            return;
        }

        _musicSource.Stop();
    }

    public void SetMusicVolume(float v)
    {
        _musicVolume = Mathf.Clamp01(v);
        if (_musicSource != null)
        {
            _musicSource.volume = _musicVolume;
        }
    }

    public void SetSFXVolume(float v)
    {
        _sfxVolume = Mathf.Clamp01(v);
        // PlayOneShot uses the volume passed in, but we also keep the source volume sane
        if (_sfxSource != null)
        {
            _sfxSource.volume = _sfxVolume;
        }
    }
}
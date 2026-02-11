using System.Collections;
using UnityEngine;

public class BasicEnemyShoot : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] 
    private GameObject _enemyBulletPrefab;
    [SerializeField] 
    private Transform _firePoint; 

    // This is all to determine the rate at which the bullets fire. Three in a quick burst, and three seconds in between each one. 
    [Header("Burst Settings")]
    [SerializeField]
    private int _bulletsPerBurst = 3;
    [SerializeField] 
    private float _timeBetweenShotsInBurst = 0.12f;
    [SerializeField] 
    private float _timeBetweenBursts = 3f;

    private Coroutine _shootRoutine;

    private void OnEnable()
    {
        // Start shooting as soon as the enemy spawns/enables.
        _shootRoutine = StartCoroutine(ShootLoop());
    }

    private void OnDisable()
    {
        if (_shootRoutine != null)
        {
            StopCoroutine(_shootRoutine);
            _shootRoutine = null;
        }
    }

    private IEnumerator ShootLoop()
    {
        // Burst immediately on spawn
        yield return StartCoroutine(FireBurst());

        // And it'll infinitely fire off the burst every three seconds until it is destroyed. 
        while (true)
        {
            yield return new WaitForSeconds(_timeBetweenBursts);
            yield return StartCoroutine(FireBurst());
        }
    }

    private IEnumerator FireBurst()
    {
        if (_enemyBulletPrefab == null)
        {
            Debug.LogWarning("BasicEnemyShoot: No bullet prefab assigned.");
            yield break;
        }

        Transform spawn = (_firePoint != null) ? _firePoint : transform;

        // A for loop, I think it's called, is like this: if 0 is less than than the bulletsPerBurst (3), then it will run this code again, and again, until all three have been
        // Instantiated. 
        for (int i = 0; i < _bulletsPerBurst; i++)
        {
            Instantiate(_enemyBulletPrefab, spawn.position, Quaternion.identity);
            AudioManager.Instance?.PlayEnemyBullet();
            yield return new WaitForSeconds(_timeBetweenShotsInBurst);
        }
    }
}

using System.Collections;
using UnityEngine;

public class BossShoot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] 
    private Transform _firePoint;
    [SerializeField] 
    private GameObject _bulletPrefab;

    [Header("Normal Fire")]
    [SerializeField] 
    private float _normalFireRate = 0.5f;

    [Header("Special Sweep")]
    [SerializeField] 
    private float _timeBetweenSweeps = 10f;
    [SerializeField] 
    private float _sweepDuration = 2f;
    [SerializeField] 
    private float _sweepFireRate = 0.08f;
    [SerializeField] 
    private float _rightAngle = 80f;
    [SerializeField] 
    private float _leftAngle = -80f;

    [Header("Grace Period")]
    [SerializeField] 
    private float _initialGracePeriod = 3f;

    private Transform _player;
    private Coroutine _mainRoutine;

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            _player = p.transform;
        }

        if (_firePoint == null)
        {
            _firePoint = transform;
        }

        _mainRoutine = StartCoroutine(BossFireLoop());
    }

    private IEnumerator BossFireLoop()
    {
        yield return new WaitForSeconds(_initialGracePeriod);
        Debug.Log("Boss: Grace period over - starting attacks");

        float sweepTimer = 0f;

        while (true)
        {
            if (sweepTimer < _timeBetweenSweeps)
            {
                FireAtPlayerOnce();
                yield return new WaitForSeconds(_normalFireRate);
                sweepTimer += _normalFireRate;
            }
            else
            {
                yield return StartCoroutine(SweepAttack());
                sweepTimer = 0f;
            }
        }
    }

    private void FireAtPlayerOnce()
    {
        if (_bulletPrefab == null || _firePoint == null)
        {
            return;
        }

        Vector2 direction;

        if (_player != null)
        {
            direction = ((Vector2)_player.position - (Vector2)_firePoint.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            _firePoint.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            direction = Vector2.down;
        }

        FireBullet(direction);
    }

    private IEnumerator SweepAttack()
    {
        yield return StartCoroutine(SweepAndFire(_rightAngle, _leftAngle));
        yield return StartCoroutine(SweepAndFire(_leftAngle, _rightAngle));
    }

    private IEnumerator SweepAndFire(float startAngle, float endAngle)
    {
        float elapsed = 0f;
        float fireTimer = 0f;

        while (elapsed < _sweepDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _sweepDuration);

            float angle = Mathf.Lerp(startAngle, endAngle, t);
            _firePoint.rotation = Quaternion.Euler(0f, 0f, angle);

            fireTimer += Time.deltaTime;
            if (fireTimer >= _sweepFireRate)
            {
                // Calculate direction from angle and fire
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.down;
                FireBullet(direction);

                fireTimer = 0f;
            }

            yield return null;
        }
    }

    private void FireBullet(Vector2 direction)
    {
        if (_bulletPrefab == null)
        {
            return;
        }

        // Instantiate bullet
        GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);

        // CRITICAL: Pass direction to bullet script
        BossBullet bulletScript = bullet.GetComponent<BossBullet>();
        if (bulletScript != null)
        {
            bulletScript.SetVelocity(direction);
        }
        else
        {
            Debug.LogError("BossBullet script missing on bullet prefab!");
            Destroy(bullet);
        }

        // Play SFX
        AudioManager.Instance?.PlayEnemyBullet();
    }

    private void OnDisable()
    {
        if (_mainRoutine != null) StopCoroutine(_mainRoutine);
    }
}
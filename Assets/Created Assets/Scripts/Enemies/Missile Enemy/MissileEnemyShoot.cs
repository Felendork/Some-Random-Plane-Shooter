using System.Collections;
using UnityEngine;

public class MissileEnemyShoot : MonoBehaviour
{
    [SerializeField] 
    private GameObject _missilePrefab;
    [SerializeField] 
    private Transform _firePoint;
    [SerializeField] 
    private float _fireInterval = 5f;

    private Coroutine _routine;

    private void OnEnable()
    {
        _routine = StartCoroutine(FireLoop());
    }

    private void OnDisable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
        }
    }

    private IEnumerator FireLoop()
    {
        FireOnce(); // fire immediately on spawn

        while (true)
        {
            yield return new WaitForSeconds(_fireInterval);
            FireOnce();
        }
    }

    private void FireOnce()
    {
        if (_missilePrefab == null)
        {
            return;
        }

        Transform spawn = (_firePoint != null) ? _firePoint : transform;
        Instantiate(_missilePrefab, spawn.position, spawn.rotation);
        AudioManager.Instance?.PlayEnemyMissileSFX();
    }
}

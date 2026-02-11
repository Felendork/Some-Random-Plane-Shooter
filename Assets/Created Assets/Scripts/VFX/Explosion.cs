using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    [SerializeField] private float _destroyDelay = 1.5f;

    void Start()
    {
        Destroy(gameObject, _destroyDelay);
    }
}
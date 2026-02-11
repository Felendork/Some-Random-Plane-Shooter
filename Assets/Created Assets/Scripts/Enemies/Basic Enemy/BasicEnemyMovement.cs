using System.Runtime.CompilerServices;
using UnityEngine;

public class BasicEnemyMovement : MonoBehaviour
{
    [SerializeField]
    private int _speed = 5;

    void Update()
    {
        // Now I have this changed from Vector3.down to Vector3.up. 
        // Reason being is that it's rotation is personal, not world-based. With its model default to facing up, having it go down would have it fly backwards. But now that it's
        // rotated, it's 'down' axis is facing up, and so it would fly backwards upwards instead. 
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        // Once it reaches the bottom of the screen, or rather just past it, it will be teleported back to the top.
        if (transform.position.y < -5.7f)
        {
            RespawnAtTop();
        }
    }

    private void RespawnAtTop()
    {
        // This determines a random value between the two numbers below, which are the X axis edges. 
        float randomX = Random.Range(-8.35f, 8.35f);

        // And this makes it teleport at a random X position at 5.7 on the Y. 
        transform.position = new Vector3(randomX, 5.7f, 0);
    }
}

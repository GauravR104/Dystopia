using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        FindFirstObjectByType<DetetcionEnemy>().AlertEnemy(transform.position);
    }
}

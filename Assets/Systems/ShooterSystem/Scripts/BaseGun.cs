using UnityEngine;

public abstract class BaseGun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject firePoint;
    public float fireDirection ;
    public abstract void Fire();

    // Optional: shared logic for all guns
    public void Reload()
    {
        Debug.Log("Reloading...");
    }

    public float FireDirection()
    {
        return fireDirection = (firePoint.transform.position.x - transform.position.x) > 0 ? 1 : -1;
    }
}
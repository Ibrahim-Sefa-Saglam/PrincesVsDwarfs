using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.ShooterSystem
{
    public class Shotgun : BaseGun
    {
        [Header("Shotgun Settings")]
        public int bulletCount = 5;
        public float maxSpread = 10f;
        public float bulletSpeed = 10f;
        
        public override void Fire()
        {
            if (!bulletPrefab || !firePoint)
            {
                Debug.LogWarning("Missing bulletPrefab or firePoint.");
                return;
            }

            for (int i = 0; i < bulletCount; i++)
            {
                // Random spread angle within -maxSpread to +maxSpread
                float spreadAngle = Random.Range(-maxSpread, maxSpread);

                // Calculate spread rotation
                Quaternion spreadRotation = firePoint.transform.rotation * Quaternion.Euler(0, 0, spreadAngle * FireDirection());
                                
                // Instantiate bullet
                GameObject bullet = Instantiate(bulletPrefab, firePoint.transform.position, spreadRotation);

                // Make 'this' the parent of the bullet
                bullet.layer = gameObject.layer;
                
                // Apply velocity if the bullet has a Rigidbody
                bullet.TryGetComponent<Rigidbody2D>(out var rb);
                if (rb)
                {
                    rb.velocity = bullet.transform.right * (bulletSpeed * FireDirection());
                }
                

                // Draw a visible debug ray in the direction of the bullet

            }
        }

    }
}
using Unity.VisualScripting;
using UnityEngine;

namespace Systems.ShooterSystem.Scripts
{
    public class Bullet: MonoBehaviour
    {
        void OnCollisionEnter2D(Collision2D collision)
        {
            collision.gameObject.TryGetComponent<IBulletDamagable>(out IBulletDamagable bulletDamagable);
            if (bulletDamagable != null)
            {
                Destroy(gameObject);
                bulletDamagable.TakeDamage();
            }
            else
            {
                Destroy(gameObject, 1.25f);
            }
            
        }    
    }
}
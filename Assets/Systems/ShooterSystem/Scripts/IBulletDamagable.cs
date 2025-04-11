using UnityEngine;

namespace Systems.ShooterSystem.Scripts
{
    public interface IBulletDamagable
    {  
        float health { get; set; }
        ParticleSystem particleSystem { get; set; }
        public void TakeDamage(); // reduces health, and initiates the particle system
    }
}
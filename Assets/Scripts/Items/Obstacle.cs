using UnityEngine;

public abstract class Obstacle : GameItem
{
    public int health;
    
    public GameObject[] ObstacleParticles;
    
    public override void OnTap() {
    }

    public abstract void TakeDamage(int damage);

    protected virtual void StartParticle(int layer) {
        ParticleManager.Instance.StartObstacleParticle(x,y,ObstacleParticles[layer]);
    }
    
}
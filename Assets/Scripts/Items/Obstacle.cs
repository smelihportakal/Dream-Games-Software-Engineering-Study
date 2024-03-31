using UnityEngine;

public abstract class Obstacle : GameItem
{
    public int health;
    
    public GameObject ObstacleParticle;
    
    public override void OnTap() {
        // Obstacles don't react to taps
    }

    public abstract void TakeDamage(int damage);

    protected virtual void StartParticle() {
        ParticleManager.Instance.StartObstacleParticle(x,y,ObstacleParticle);
    }
    

    
}
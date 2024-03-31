
public abstract class Bomb : GameItem
{
    
    public override void OnTap() {
        // Obstacles don't react to taps
    }
    
    protected virtual void Destroy() {
        // Logic to handle destruction of the obstacle
    }
}
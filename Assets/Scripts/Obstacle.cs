public abstract class Obstacle : CellItem
{
    public int health;

    public Obstacle(ItemType type) : base(ItemType.Obstacle) {}

    public override void OnTap() {
        // Obstacles don't react to taps
    }

    public abstract void TakeDamage(int damage);

    protected virtual void Destroyy() {
        // Logic to handle destruction of the obstacle
        
    }
    

    
}
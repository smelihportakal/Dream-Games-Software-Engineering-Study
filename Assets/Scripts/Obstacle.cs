public abstract class Obstacle : CellItem
{
    protected int health;

    public Obstacle(ItemType type) : base(ItemType.Obstacle) {}

    public override void OnTap() {
        // Obstacles don't react to taps
    }

    public virtual void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) {
            Destroy();
        }
    }

    protected virtual void Destroy() {
        // Logic to handle destruction of the obstacle
    }
}
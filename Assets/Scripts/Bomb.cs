
public abstract class Bomb : CellItem
{

    public Bomb(ItemType type) : base(ItemType.Bomb) {}

    public override void OnTap() {
        // Obstacles don't react to taps
    }

    public abstract void StartBomb(GridSystem2D<GridObject<CellItem>> grid, int x , int y);

    protected virtual void Destroy() {
        // Logic to handle destruction of the obstacle
    }
}
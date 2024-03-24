
public class Empty : CellItem
{
    public Empty(ItemType type) : base(ItemType.Empty) {}

    public override void OnTap() {
        // Obstacles don't react to taps
    }
    
    public override void Clear()
    {

    }

}

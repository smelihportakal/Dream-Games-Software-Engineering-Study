using UnityEngine;

public class TNT : CellItem
{
    public TNT(ItemType type) : base(ItemType.TNT) {}

    public override void OnTap() {
        // Logic for TNT tapping
        // Explode in a 5x5 area
    }
}

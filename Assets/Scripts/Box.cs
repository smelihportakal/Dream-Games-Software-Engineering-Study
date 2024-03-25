using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Obstacle
{
    public Box(ItemType type) : base(ItemType.Obstacle) {}
    
    public override void Clear()
    {
        TakeDamage(1);
    }
    
    public override void ClearOnNear()
    {
        TakeDamage(1);
    }

    public override void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) {
            GridBoard.Instance.grid.SetValue(x,y, null);
            Destroy(gameObject);
        }
    }
}

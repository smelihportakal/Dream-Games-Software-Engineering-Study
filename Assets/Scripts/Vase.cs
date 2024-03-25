using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Vase : Obstacle
{
    [SerializeField] Sprite[] states;
    
    public Vase(ItemType type) : base(ItemType.Obstacle) {}
    
    public override void Clear()
    {
        TakeDamage(1);
    }
    
    public override void ClearOnNear()
    {
        Clear();
    }
    
    public override void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) {
            GridBoard.Instance.grid.SetValue(x,y, null);
            Destroy(gameObject);
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = states[states.Length - health];
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Obstacle
{
    
    public void Start()
    {
        moveable = false;
    }

    public override void Clear()
    {
        TakeDamage(1);
    }

    public override void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) {
            GameManager.Instance.grid.SetValue(x,y, null);
            GoalManager.Instance.UpdateGoal("s");
            StartParticle();
            Destroy(gameObject);
            SoundManager.Instance.StoneDestroy();
        }
    }
}
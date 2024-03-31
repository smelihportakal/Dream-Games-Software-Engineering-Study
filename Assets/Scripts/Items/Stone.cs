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
    
    public override void OnTap()
    {
        GetComponent<Animator>().Play("Stone Shake",0);
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
            EndGameManager.Instance.UpdateGoal("s");
            StartParticle(0);
            Recycle();
            SoundManager.Instance.StoneDestroy();
        }
    }
    
    public void Recycle()
    {
        ObjectPooler.Instance.ReturnObjectToPool("s", gameObject);
    }
}
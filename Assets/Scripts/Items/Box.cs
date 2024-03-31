using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Obstacle
{
    public override void OnTap()
    {
        GetComponent<Animator>().Play("Box Shake",0);
    }

    public void Start()
    {
        moveable = false;
    }
    
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
            SoundManager.Instance.PlayAudio("box");
            GameManager.Instance.grid.SetValue(x,y, null);
            EndGameManager.Instance.UpdateGoal("bo");
            StartParticle(0);
            Recycle();
        }
    }

    public void Recycle()
    {
        ObjectPooler.Instance.ReturnObjectToPool("bo", gameObject);
    }
}

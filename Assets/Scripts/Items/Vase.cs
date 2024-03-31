using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Vase : Obstacle
{
    [SerializeField] Sprite[] states;

    public override void OnTap()
    {
        GetComponent<Animator>().Play("Vase Shake",0);
    }

    public override void Clear()
    {
        TakeDamage(1);
        SoundManager.Instance.PlayAudio("vase");
    }
    
    public override void ClearOnNear()
    {
        Clear();
    }
    
    public override void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) {
            GameManager.Instance.clearGridCell(x,y);
            EndGameManager.Instance.UpdateGoal("v");

            StartParticle(0);
            Recycle();
        }
        else
        {
            StartParticle(health);
            GetComponent<SpriteRenderer>().sprite = states[states.Length - health];
            GetComponent<Animator>().Play("Vase Shake",0);
        }
    }
    
    public override void Recycle()
    {
        health = 2;
        GetComponent<SpriteRenderer>().sprite = states[0];
        ObjectPooler.Instance.ReturnObjectToPool("v", gameObject);
    }

}
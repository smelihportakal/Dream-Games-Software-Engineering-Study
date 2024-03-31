using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Vase : Obstacle
{
    [SerializeField] Sprite[] states;
    public bool destroyed;
    
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
            Debug.Log("Vase health " + health);
            GoalManager.Instance.UpdateGoal("v");
            if (!destroyed)
            {
                StartParticle();
                GameManager.Instance.grid.SetValue(x,y, null);
                Destroy(gameObject);
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = states[states.Length - health];
        }
    }
}
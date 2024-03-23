using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : CellItem
{
    public Sprite normalState;
    public Sprite bombState;
    public CubeType cubeType;
    private int state = 0;
   
    public Cube(ItemType type, Sprite normalState, Sprite bombState, CubeType cubeType) : base(ItemType.Cube)
    {
        GetComponent<SpriteRenderer>().sprite = normalState;
    }

    public override void OnTap()
    {
        GetComponent<SpriteRenderer>().sprite = bombState;
    }

    public void changeState(int state)
    {
        this.state = state;
        if (this.state == 0)
        {
            GetComponent<SpriteRenderer>().sprite = normalState;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = bombState;
        }
    }
    void Update()
    {
  
    }
}

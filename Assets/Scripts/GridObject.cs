using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridObject<T>
{

    GridSystem2D<GridObject<T>> grid;
    public int x;
    public int y;
    T cellItem;
        
    public GridObject(GridSystem2D<GridObject<T>> grid, int x, int y) {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void SetValue(T cellItem) {
        this.cellItem = cellItem;
    }

    public void SetValue(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
        
    public T GetValue() => cellItem;

    public bool isEmpty()
    {
        Debug.Log(cellItem.GetType());
        
        return false;
    }
}

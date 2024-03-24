using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : Bomb
{
    public TNT(ItemType type) : base(ItemType.Bomb) {}

    public override void OnTap() {
        // Logic for TNT tapping
        // Explode in a 5x5 area
        List<CellItem> adjacentItems = GridBoard.Instance.getAdjacentItems(x,y);

        bool isComboTnt = false;
        
        Debug.Log("dafsdf");
        
        foreach (CellItem cellItem in adjacentItems)
        {
            Debug.Log(cellItem.GetItemType());
            if (cellItem.GetItemType() == ItemType.Bomb)
            {
                Debug.Log("second bomb");
                GridBoard.Instance.grid.SetValue(cellItem.x,cellItem.y, null);
                StartCoroutine(cellItem.MoveToPositionAndDestroy(GridBoard.Instance.grid.GetWorldPositionCenter(x, y),2f));
                isComboTnt = true;
            }
        }

        if (isComboTnt)
        {
            Debug.Log("combo TNT");
            StartCoroutine(ClearBigTNTCoroutine());
        }
        else
        {
            Clear();
            GridBoard.Instance.IsAnimationContinue = false;
        }
        
    }
    
    private IEnumerator ClearBigTNTCoroutine()
    {
        gameObject.transform.position += new Vector3(0,0,-5f);
        while (gameObject.transform.localScale.x < 4f)
        {
            gameObject.transform.localScale += new Vector3(0.1f,0.1f,0f);
            yield return new WaitForSeconds(0.1f);
        }
        GridBoard.Instance.grid.SetValue(x,y, null);
        Destroy(gameObject);
        GridBoard.Instance.TriggerTnT(x,y, 3);
        GridBoard.Instance.CollapseGrid();
        GridBoard.Instance.IsAnimationContinue = false;
    }

    
    public override void Clear()
    {
        IsBeingCleared = true;
        GridBoard.Instance.grid.SetValue(x,y, null);
        StartCoroutine(ClearCoroutine());
        GridBoard.Instance.TriggerTnT(x,y, 2);
        GridBoard.Instance.CollapseGrid();
    }
    
    private IEnumerator ClearCoroutine()
    {
        while (gameObject.transform.localScale.x < 1.5f)
        {
            gameObject.transform.localScale += new Vector3(0.1f,0.1f,0);
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
    }

    public override void StartBomb(GridSystem2D<GridObject<CellItem>> grid, int posx , int posy)
    {
        for (int x = posx - 2; x <= posx + 2; x++)
        {
            if (x >= 0 && x < grid.width)
            {
                for (int y = posy - 2; y <= posy + 2; y++)
                {
                    if (y >= 0 && y < grid.height)
                    {
                        
                        Debug.Log("bomb");
                        grid.GetValue(x, y).GetValue().Clear();
                        grid.SetValue(x,y,null);
                    }
                }
            }
        }
    }
    
}

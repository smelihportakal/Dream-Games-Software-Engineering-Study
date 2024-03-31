using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : Bomb
{
    public GameObject TntParticle;
    private IEnumerator clearCo;

    public override void OnTap() {
        // Logic for TNT tapping
        // Explode in a 5x5 area
        HashSet<GameItem> adjacentItems = new HashSet<GameItem>();
        
        GameManager.Instance.getAdjacentItems(x,y,ref adjacentItems);

        bool isComboTnt = false;
        Debug.Log("TNT " );

        
        foreach (GameItem cellItem in adjacentItems)
        {
            Debug.Log("TNT " + cellItem.GetItemType());
            if (cellItem.GetItemType() == ItemType.Bomb)
            {
                GameManager.Instance.clearGridCell(cellItem.x,cellItem.y);
                StartCoroutine(cellItem.MoveToPositionAndDestroy(GameManager.Instance.grid.GetWorldPositionCenter(x, y),2f));
                isComboTnt = true;
            }
        }

        if (isComboTnt)
        {
            GameManager.Instance.StopCollapseCo();
            StartCoroutine(ClearBigTNTCoroutine());
        }
        else
        {
            Clear();
            GameManager.Instance.IsAnimationContinue = false;
        }
    }
    
    private IEnumerator ClearBigTNTCoroutine()
    {
        gameObject.transform.position += new Vector3(0,0,-6f);
        while (gameObject.transform.localScale.x < 4f)
        {
            gameObject.transform.localScale += new Vector3(0.2f,0.2f,0f);
            yield return new WaitForSeconds(0.1f);
        }
        GameManager.Instance.clearGridCell(x,y);
        Recycle();
        TriggerTnT(x,y, 3);
        GameManager.Instance.StartCollapseCo();
        GameManager.Instance.IsAnimationContinue = false;
    }

    
    public override void Clear()
    {
        /*
        if (clearCo != null)
        {
            StopCoroutine(clearCo);
            gameObject.transform.localScale = new Vector3(1f,1f,0);
        }
        */
        IsBeingCleared = true;
        GameManager.Instance.StopCollapseCo();
        clearCo = ClearCoroutine();
        StartCoroutine(clearCo);
    }
    
    private IEnumerator ClearCoroutine()
    {
        while (gameObject.transform.localScale.x < 1.5f)
        {
            gameObject.transform.localScale += new Vector3(0.1f,0.1f,0);
            yield return new WaitForSeconds(0.1f);
        }
        ParticleManager.Instance.StartTntParticle(x,y,TntParticle);
        GameManager.Instance.clearGridCell(x,y);
        TriggerTnT(x,y, 2);
        Recycle();
    }
    
    public void TriggerTnT(int posx, int posy, int size)
    {
        (int width, int height) = GameManager.Instance.getGridSize();
        
        for (int x = posx - size; x <= posx + size; x++)
        {
            if (x >= 0 && x < width)
            {
                for (int y = posy - size; y <= posy + size; y++)
                {
                    if (x == posx && y == posy)
                    {
                        continue;
                    }
                    if (y >= 0 && y < height)
                    {
                        if (GameManager.Instance.getItemAtCell(x,y) != null)
                        {
                            GameManager.Instance.getItemAtCell(x,y).Clear();
                        }
                    }
                }
            }
        }
        GameManager.Instance.StartCollapseCo();
    }

    public void Recycle()
    {
        gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 0f);
        ObjectPooler.Instance.ReturnObjectToPool("tnt", gameObject);
    }
}

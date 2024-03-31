using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : Bomb
{
    public GameObject TntParticle;
    private IEnumerator clearCo;
    
    public override void OnTap() {
        HashSet<GameItem> adjacentItems = new HashSet<GameItem>();
        getAdjacentItems(ref adjacentItems);

        bool isComboTnt = false;
        foreach (GameItem cellItem in adjacentItems)
        {
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
        }
        EndGameManager.Instance.decreaseMoveCount(1);
    }
    
    private IEnumerator ClearBigTNTCoroutine()
    {
        transform.position += new Vector3(0,0,-3f);
        GetComponent<Animator>().Play("TNT Explode",0);
        yield return new WaitForSeconds(1.15f);
        SoundManager.Instance.PlayAudio("tnt");
        ParticleManager.Instance.StartTntParticle(x,y,TntParticle);
        GameManager.Instance.clearGridCell(x,y);
        Recycle();
        TriggerTnT(x,y, 3);
        GameManager.Instance.StartCollapseCo();
    }

    
    public override void Clear()
    {
        GameManager.Instance.StopCollapseCo();
        clearCo = ClearCoroutine();
        StartCoroutine(clearCo);
    }
    
    private IEnumerator ClearCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        ParticleManager.Instance.StartTntParticle(x,y,TntParticle);
        GameManager.Instance.clearGridCell(x,y);
        SoundManager.Instance.PlayAudio("tnt");
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

    public override void Recycle()
    {
        gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
        ObjectPooler.Instance.ReturnObjectToPool("tnt", gameObject);
    }
    
    public override IEnumerator  MoveToPositionAndDestroy(Vector3 targetPosition, float speed)
    {
        Vector3 from = transform.position;
        Vector3 to = targetPosition + new Vector3(0,0,-3);
        float howfar = 0;
        do
        {
            howfar += speed * Time.deltaTime;
            if (howfar > 1)
                howfar = 1;
            
            transform.position = Vector3.LerpUnclamped(from, to, Easing(howfar));
            yield return null;
        } while (howfar != 1);

        Recycle();
    }

}

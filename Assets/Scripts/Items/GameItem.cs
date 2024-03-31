using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType {
    Cube,
    Bomb,
    Obstacle
}

[RequireComponent(typeof(SpriteRenderer))] 
public abstract class GameItem : MonoBehaviour
{
    public Sprite defaultSprite;
    public int x;
    public int y;
    public ItemType type;
    public bool moveable;
    
    public void setCoordinate(int x , int y)
    {
        this.x = x;
        this.y = y;
    }
    
    public abstract void OnTap();
    
    public abstract void Clear();

    public ItemType GetItemType()
    {
        return type;
    }
    
    public IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        Vector3 from = transform.position;
        Vector3 to = targetPosition;
        float howfar = 0;
        do
        {
            howfar += Time.deltaTime;
            if (howfar > 1)
                howfar = 1;
            
            transform.position = Vector3.LerpUnclamped(from, to, Easing(howfar));
            yield return null;
        } while (howfar != 1);
    }
    
    public virtual IEnumerator MoveToPositionAndDestroy(Vector3 targetPosition, float speed)
    {
        Vector3 from = transform.position;
        Vector3 to = targetPosition;
        float howfar = 0;
        do
        {
            howfar += speed * Time.deltaTime;
            if (howfar > 1)
                howfar = 1;
            
            transform.position = Vector3.LerpUnclamped(from, to, Easing(howfar));
            yield return null;
        } while (howfar != 1);

        Destroy(gameObject);
    }
    
    public float Easing(float t)
    {
        return t < 0.5f
            ? 16 * t * t * t * t * t
            : 1 - Mathf.Pow(-2 * t + 2, 5) / 2;
    }

    public virtual void ClearOnNear()
    {
        
    }
    
    public void getAdjacentItems(ref HashSet<GameItem> adjacentItems)
    {
        GridBoard<GameItem> grid = GameManager.Instance.grid;
        if (x >= 0 && y - 1 >= 0)
        {
            if (grid.GetValue(x, y - 1) != null )
            {
                adjacentItems.Add(grid.GetValue(x, y - 1));
            }
        }

        if (x >= 0 && y + 1 < grid.height)
        {
            if (grid.GetValue(x, y + 1) != null)
            {
                adjacentItems.Add(grid.GetValue(x, y + 1));
            }
        }

        if (x - 1 >= 0 && y >= 0)
        {
            if (grid.GetValue(x - 1, y) != null)
            {
                adjacentItems.Add(grid.GetValue(x - 1, y));
            }
        }

        if (x + 1 < grid.width && y < grid.height)
        {
            if (grid.GetValue(x + 1, y) != null)
            {
                adjacentItems.Add(grid.GetValue(x + 1, y));
            }
        }
    }

    public abstract void Recycle();


}

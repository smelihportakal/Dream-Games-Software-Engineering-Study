using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType {
    Cube,
    TNT,
    Bomb,
    Obstacle,
    Empty
}

[RequireComponent(typeof(SpriteRenderer))] 
public abstract class CellItem : MonoBehaviour
{
    public string tag;
    public int x;
    public int y;
    public ItemType type;
    private bool idle = true;
    public bool moveable;
    public bool IsBeingCleared { get;  set; }

    public CellItem(ItemType type) {
        this.type = type;
        this.IsBeingCleared = false;
    }

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

    public bool IsAvailable()
    {
        return IsBeingCleared;
    }
    
    public IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        Vector3 from = transform.position;
        Vector3 to = targetPosition;
        float howfar = 0;
        idle = false;
        do
        {
            howfar += Time.deltaTime;
            if (howfar > 1)
                howfar = 1;
            
            transform.position = Vector3.LerpUnclamped(from, to, Easing(howfar));
            yield return null;
        } while (howfar != 1);

        idle = true;
    }
    
    public virtual IEnumerator MoveToPositionAndDestroy(Vector3 targetPosition, float speed)
    {
        Vector3 from = transform.position;
        Vector3 to = targetPosition;
        float howfar = 0;
        idle = false;
        do
        {
            howfar += speed * Time.deltaTime;
            if (howfar > 1)
                howfar = 1;
            
            transform.position = Vector3.LerpUnclamped(from, to, Easing(howfar));
            yield return null;
        } while (howfar != 1);

        idle = true;
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }
    
    public float Easing(float t)
    {
        float c1 = 1.70158f,
            c2 = c1 * 1.525f;

        return t < 0.5f
            ? (Mathf.Pow(t * 2, 2) * ((c2 + 1) * 2 * t - c2)) / 2
            : (Mathf.Pow(t * 2 - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
    }

    public virtual void ClearOnNear()
    {
        
    }
}

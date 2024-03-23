using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType {
    Cube,
    TNT,
    Obstacle,
    Empty
}

[RequireComponent(typeof(SpriteRenderer))] 
public abstract class CellItem : MonoBehaviour
{
    public ItemType type;
    private bool idle = true;
    public CellItem(ItemType type) {
        this.type = type;
    }
    
    public abstract void OnTap();

    public ItemType GetItemType()
    {
        return type;
    }

    public bool GetIdle()
    {
        return idle;
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
    
    private float Easing(float t)
    {
        float c1 = 1.70158f,
            c2 = c1 * 1.525f;

        return t < 0.5f
            ? (Mathf.Pow(t * 2, 2) * ((c2 + 1) * 2 * t - c2)) / 2
            : (Mathf.Pow(t * 2 - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
    }

}

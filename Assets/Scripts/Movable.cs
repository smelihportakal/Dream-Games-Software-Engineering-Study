using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{
    private Vector3 from, to;
    private float howfar;
    private float speed = 1;
    private bool idle = true;

    public bool getIdle()
    {
        return idle;
    }

    public IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        if (speed <= 0)
            Debug.LogWarning("Speed must be a positive number.");
        from = transform.position;
        to = targetPosition;
        howfar = 0;
        idle = false;
        do
        {
            howfar += speed * Time.deltaTime;
            if (howfar > 1)
                howfar = 1;
            
            transform.position = Vector3.Lerp(from, to, howfar);

            yield return null;
        } while (howfar != 1);

        idle = true;
    }

    private float Easing(float t)
    {
        return t * t;
    }
}

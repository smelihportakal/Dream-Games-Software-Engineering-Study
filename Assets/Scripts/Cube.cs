using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : CellItem
{
    public Sprite normalState;
    public Sprite bombState;
    public CubeType cubeType;
    private int state = 0;
    public GameObject particle;
    public CubeColor currentCubeColor;
    
    public List<CubeColor> cubeColors;
    public Dictionary<string, CubeColor> colorDictionary;
    
    public Cube(ItemType type, Sprite normalState, Sprite bombState, CubeType cubeType) : base(ItemType.Cube)
    {
        GetComponent<SpriteRenderer>().sprite = normalState;
    }

    public override void OnTap()
    {
        GetComponent<SpriteRenderer>().sprite = currentCubeColor.bombState;
    }

    public void changeState(int state)
    {
        this.state = state;
        if (this.state == 0)
        {
            GetComponent<SpriteRenderer>().sprite = currentCubeColor.normalState;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = currentCubeColor.bombState;
        }
    }
    void Update()
    {
  
    }

    public void ChangeColor(CubeColor cubeColor)
    {
        currentCubeColor = cubeColor;
        GetComponent<SpriteRenderer>().sprite = currentCubeColor.normalState;
        cubeType = currentCubeColor.cubeType;
    }

    public override void Clear()
    {
        IsBeingCleared = true;
        GridBoard.Instance.grid.SetValue(x,y, null);
        GridBoard.Instance.StartCubeParticle(x,y, currentCubeColor.particleMaterial);
        //StartCoroutine(GridBoard.Instance.CubeParticleCoroutine(x,y));
        ObjectPooler.Instance.ReturnObjectToPool("cube", gameObject);
        //Debug.Log("are you working");
        
        /*
        ObjectPooler.Instance.ReturnObjectToPool("b",gameObject);

        GameObject particles = ObjectPooler.Instance.SpawnFromPool("cube_particle",
            GridBoard.Instance.grid.GetWorldPositionCenter(x, y), Quaternion.identity);
            
        //yield return new WaitForSeconds(4f);
        Debug.Log("Particle deleted");
        ObjectPooler.Instance.ReturnObjectToPool("cube_particle", particles);
        */

    }

    private IEnumerator ClearCoroutine()
    {

        GameObject particles = ObjectPooler.Instance.SpawnFromPool("cube_particle",
            GridBoard.Instance.grid.GetWorldPositionCenter(x, y), Quaternion.identity);
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("Particle deleted");
        ObjectPooler.Instance.ReturnObjectToPool("cube",gameObject);

        ObjectPooler.Instance.ReturnObjectToPool("cube_particle", particles);

    }
    
    public override IEnumerator  MoveToPositionAndDestroy(Vector3 targetPosition, float speed)
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

        ObjectPooler.Instance.ReturnObjectToPool("cube",gameObject);
        Debug.Log("is it working");
        //gameObject.SetActive(false);
        //Destroy(gameObject);
    }

}
